using System;
using System.Collections.Generic;
using System.Linq;
using CSV_To_ScriptableObject.Editor.Services;
using UnityEditor;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Window
{
    /// <summary> Main editor class for CSV to ScriptableObject conversion </summary>
    public class CsvToScriptableObject: EditorWindow
    {
        // Data structures
        private readonly List<List<string>> _csvData = new();
        private readonly Dictionary<string, string> _fieldMappings = new();
        private readonly Dictionary<string, System.Reflection.FieldInfo> _allFieldsCache = new();
        private readonly List<string> _allFieldPaths = new();
        private readonly List<string> _csvHeaders = new();

        // UI state
        private Vector2 _scrollPosition;
        private string _csvFilePath;
        private string _outputFolder = "Assets/";
        private Type _selectedScriptableObjectType;
        private GUIStyle _headerStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _boxStyle;
        private GUIStyle _errorStyle;
        private bool _showPreview;
        private bool _showOptions;
        private string _status = "";
        private bool _hasError;
        private bool _currentThemeIsDark;

        // CSV parsing options
        private char _delimiter = ',';
        private bool _hasHeaderRow = true;
        private char _quoteCharacter = '"';

        // Available ScriptableObject types
        private List<Type> _availableTypes = new();
        private string[] _typeNames;
        private int _selectedTypeIndex;
        private Vector2 _fieldMappingScrollPosition = Vector2.zero;
        private string _fieldSearchQuery = "";

        // Data processing services
        private CsvParser _csvParser;
        private ScriptableObjectProcessor _processor;
        private FieldMappingService _fieldMappingService;
        private string _splitCollectionOrArray = ";";
        [MenuItem("Tools/CSV to ScriptableObject Converter")]
        public static void ShowWindow()
        {
            // Create and display converter window
            var window = GetWindow<CsvToScriptableObject>("CSV to ScriptableObject");
            window.minSize = new(500, 700);
        }
        private void OnEnable()
        {
            // Initialize services
            this._csvParser = new();
            this._processor = new();
            this._fieldMappingService = new();

            // Load all ScriptableObject types
            this.LoadAvailableTypes();

            // Subscribe to editor update to detect theme changes
            EditorApplication.update += this.CheckForThemeChanges;
        }
        private void LoadAvailableTypes()
        {
            // Get all ScriptableObject types from assemblies
            this._availableTypes = AppDomain.CurrentDomain.GetAssemblies()
                                            .SelectMany(assembly => assembly.GetTypes())
                                            .Where(type => typeof(ScriptableObject).IsAssignableFrom(type) && !type.IsAbstract)
                                            .OrderBy(type => type.FullName)
                                            .ToList();

            this._typeNames = this._availableTypes.Select(t => t.FullName).ToArray();
        }
        private void CheckForThemeChanges()
        {
            // Check if theme has changed
            bool newThemeIsDark = EditorGUIUtility.isProSkin;
            if (newThemeIsDark != this._currentThemeIsDark)
            {
                // Update theme state
                this._currentThemeIsDark = newThemeIsDark;

                // Force styles to be recreated
                this._headerStyle = null;
                this._sectionStyle = null;
                this._boxStyle = null;
                this._errorStyle = null;

                // Force repaint to apply new styles
                this.Repaint();
            }
        }
        private void OnDisable()
        {
            // Unsubscribe from editor update when window is closed
            EditorApplication.update -= this.CheckForThemeChanges;
        }
        private void OnGUI()
        {
            // Initialize GUI styles
            EditorUIStyles.InitializeStyles(ref this._headerStyle, ref this._sectionStyle, ref this._boxStyle, ref this._errorStyle);

            // Begin main scroll view
            this._scrollPosition = EditorGUILayout.BeginScrollView(this._scrollPosition);

            // Draw window header
            this.DrawWindowHeader();

            // Draw main UI sections
            EditorUI.DrawCsvFileSelector(ref this._csvFilePath, this.LoadCsvFile, this._boxStyle, this._sectionStyle);
            EditorUI.DrawCsvParsingOptions(ref this._delimiter,
                                           ref this._quoteCharacter,
                                           ref this._hasHeaderRow,
                                           ref this._showOptions,
                                           ref this._splitCollectionOrArray,
                                           this._csvFilePath,
                                           this.LoadCsvFile,
                                           this._boxStyle);

            // Draw ScriptableObject type selector if CSV data is available
            if (!string.IsNullOrEmpty(this._csvFilePath))
            {
                EditorUI.DrawScriptableObjectTypeSelector(ref this._selectedTypeIndex,
                                                          this._typeNames,
                                                          this._availableTypes,
                                                          ref this._selectedScriptableObjectType,
                                                          this._csvHeaders,
                                                          this.GenerateFieldList,
                                                          this.GenerateDefaultFieldMappings,
                                                          this._boxStyle,
                                                          this._sectionStyle);
            }

            // Draw field mapping and data preview if CSV data and selected type are available
            if (!string.IsNullOrEmpty(this._csvFilePath) && this._selectedScriptableObjectType != null && this._csvHeaders.Count > 0)
            {
                EditorUI.DrawFieldMappings(ref this._outputFolder,
                                           this._fieldMappings,
                                           this._allFieldPaths,
                                           this._allFieldsCache,
                                           this._csvHeaders,
                                           ref this._fieldMappingScrollPosition,
                                           ref this._fieldSearchQuery,
                                           this._boxStyle,
                                           this._sectionStyle);
                EditorUI.DrawPreviewAndProcessButtons(ref this._showPreview,
                                                      this._csvData,
                                                      this._csvHeaders,
                                                      this._fieldMappings,
                                                      () => this.ProcessCsvToScriptableObjects(),
                                                      () => this.ProcessCsvToScriptableObjects(true),
                                                      this._boxStyle,
                                                      this._sectionStyle,
                                                      this._outputFolder,
                                                      this._selectedScriptableObjectType);
            }

            // Display status message if exists
            if (!string.IsNullOrEmpty(this._status))
            {
                EditorUI.DrawStatusMessage(this._status, this._hasError, this._errorStyle);
            }

            EditorGUILayout.EndScrollView();
        }
        private void DrawWindowHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            var iconContent = EditorGUIUtility.IconContent("d_ScriptableObject Icon");
            GUILayout.Label(iconContent, GUILayout.Width(32), GUILayout.Height(32));
            EditorGUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label("CSV to ScriptableObject Converter", this._headerStyle);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private void LoadCsvFile(string filePath)
        {
            try
            {
                this._csvHeaders.Clear();
                this._csvData.Clear(); // This is correct - clearing the list, not assigning a new one

                // Load CSV data using the parser
                var result = this._csvParser.ParseCsvFile(filePath, this._delimiter, this._quoteCharacter, this._hasHeaderRow);

                // Copy headers instead of assignment
                this._csvHeaders.Clear();
                this._csvHeaders.AddRange(result.Headers);

                // Copy data instead of assignment
                this._csvData.Clear();
                foreach (var row in result.Data)
                {
                    this._csvData.Add(row);
                }

                this._status = $"Loaded CSV with {this._csvHeaders.Count} columns and {this._csvData.Count} data rows";
                this._hasError = false;

                if (this._selectedScriptableObjectType != null)
                {
                    this.GenerateFieldList();
                    this.GenerateDefaultFieldMappings();
                }
            }
            catch (Exception ex)
            {
                this._status = $"Error loading CSV file: {ex.Message}";
                this._hasError = true;
            }
        }
        private void GenerateFieldList()
        {
            this._allFieldsCache.Clear();
            this._allFieldPaths.Clear();

            // Load all fields using the field mapping service
            this._fieldMappingService.CollectAllFields(this._selectedScriptableObjectType, "", this._allFieldsCache, this._allFieldPaths);
        }
        private void GenerateDefaultFieldMappings()
        {
            this._fieldMappings.Clear();

            // Automatic field mapping by name
            this._fieldMappingService.GenerateDefaultFieldMappings(this._allFieldPaths, this._allFieldsCache, this._csvHeaders, this._fieldMappings);
        }
        private void ProcessCsvToScriptableObjects(bool update = false)
        {
            // Input validation
            if (this._csvData.Count == 0)
            {
                this._status = "No CSV data to process";
                this._hasError = true;
                return;
            }

            if (this._selectedScriptableObjectType == null)
            {
                this._status = "No ScriptableObject type selected";
                this._hasError = true;
                return;
            }

            try
            {
                // Process data using ScriptableObject service
                var result = this._processor.ProcessCsvData(this._csvData,
                                                            this._csvHeaders,
                                                            this._selectedScriptableObjectType,
                                                            this._fieldMappings,
                                                            this._outputFolder,
                                                            this._allFieldsCache,
                                                            this._splitCollectionOrArray,
                                                            this._csvFilePath,
                                                            update);

                // Display results
                string statusBase = $"Processed {this._csvData.Count} rows. Success: {result.SuccessCount}, Errors: {result.ErrorCount}";
                if (result.ErrorCount > 0)
                {
                    this._status = $"{statusBase} - Check console for detailed error information";
                    this._hasError = true;

                    if (result.ErrorMessages.Count > 0)
                    {
                        this._status += $"\nErrors: {string.Join("\n", result.ErrorMessages.Take(5))}";
                        if (result.ErrorMessages.Count > 5)
                        {
                            this._status += $"\n... and {result.ErrorMessages.Count - 5} more errors";
                        }
                    }
                }
                else
                {
                    this._status = statusBase;
                    this._hasError = false;
                }
            }
            catch (Exception ex)
            {
                this._status = $"Error processing CSV: {ex.Message}";
                if (ex.InnerException != null)
                {
                    this._status += $"\nInner exception: {ex.InnerException.Message}";
                }
                this._hasError = true;
                Debug.LogError($"CSV processing error: {ex}");
            }
        }
    }
}
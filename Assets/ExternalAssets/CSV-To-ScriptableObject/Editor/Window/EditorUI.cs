using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSV_To_ScriptableObject.Editor.Services;
using UnityEditor;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Window
{
    /// <summary> UI components for the CSV to ScriptableObject converter editor window </summary>
    public static class EditorUI
    {
        public static bool s_Update;
        public static bool s_RunTest;
        static internal string s_selectedScriptableObjectIdField;
        static internal List<CsvIdFieldConfig> s_SelectedCsvIdFields = new();
        private static int s_lastSelectedIndexDelimiter = -1;
        private static int s_lastSelectedIndexQuote = -1;
        private static bool s_showMappingSection = true;
        private static Vector2 s_previewScrollPosition;
        private static Dictionary<ScriptableObject, int> s_objectsToUpdateFound = new();
        private static bool s_showFoundObjects = true;
        private static int s_rowsPerPage = 25;
        private static int s_currentPage;
        public static void DrawCsvFileSelector(ref string csvFilePath, Action<string> loadCsvFile, GUIStyle boxStyle, GUIStyle sectionStyle)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);

            EditorGUILayout.LabelField("<u>1. Select CSV File</u>", sectionStyle);

            using (new EditorGUILayout.HorizontalScope(sectionStyle))
            {
                EditorGUILayout.LabelField("CSV File:", GUILayout.Width(80));

                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.TextField(csvFilePath ?? "No file selected", EditorStyles.textField);
                }
                if (GUILayout.Button(new GUIContent("Browse...", EditorGUIUtility.IconContent("d_TextAsset Icon").image), GUILayout.Width(80), GUILayout.Height(20)))
                {
                    try
                    {
                        string path = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
                        if (!string.IsNullOrEmpty(path))
                        {
                            csvFilePath = path;
                            loadCsvFile?.Invoke(path);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to reload CSV: {e.Message}");
                        EditorUtility.DisplayDialog("Error", $"Failed to reload CSV: {e.Message}", "OK");
                    }
                }
                if (GUILayout.Button(new GUIContent("Reload CSV", EditorGUIUtility.IconContent("Refresh").image), GUILayout.Width(100), GUILayout.Height(20)))
                {
                    try
                    {
                        loadCsvFile?.Invoke(csvFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to reload CSV: {e.Message}");
                        EditorUtility.DisplayDialog("Error", $"Failed to reload CSV: {e.Message}", "OK");
                    }
                }
            }
        }
        public static void DrawCsvParsingOptions(ref char delimiter,
                                                 ref char quoteCharacter,
                                                 ref bool hasHeaderRow,
                                                 ref bool showOptions,
                                                 ref string splitCollectionOrArray,
                                                 string csvFilePath,
                                                 Action<string> loadCsvFile,
                                                 GUIStyle boxStyle)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);

            try
            {
                // Collapsible header with icon
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUIContent iconContent = EditorGUIUtility.TrIconContent("_Popup", "CSV Parsing Options");
                    GUILayout.Space(5);
                    GUILayout.Label(iconContent, EditorStyles.label, GUILayout.Width(20));
                    showOptions = EditorGUILayout.Foldout(showOptions, "CSV Parsing Options", true, EditorStyles.foldout);
                }

                if (showOptions)
                {
                    EditorGUI.indentLevel++;

                    delimiter = DrawDelimiterSelector(delimiter);
                    quoteCharacter = DrawQuoteSelector(quoteCharacter);

                    EditorGUILayout.Space(5);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(30); // Add left indent
                        splitCollectionOrArray = EditorGUILayout.TextField("Split Collection/Array:", splitCollectionOrArray, GUILayout.Width(200));
                    }
                    EditorGUILayout.Space(5);
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(30); // Add left indent
                        hasHeaderRow = EditorGUILayout.ToggleLeft("First row contains column headers", hasHeaderRow);
                    }

                    EditorGUILayout.Space(5);
                    DrawReloadButton(csvFilePath, loadCsvFile);

                    EditorGUI.indentLevel--;
                }
            }
            catch (Exception ex)
            {
                EditorGUILayout.HelpBox($"Error in CSV options UI: {ex.Message}", MessageType.Error);
            }
        }
        public static void DrawScriptableObjectTypeSelector(ref int selectedTypeIndex,
                                                            string[] typeNames,
                                                            List<Type> availableTypes,
                                                            ref Type selectedScriptableObjectType,
                                                            List<string> csvHeaders,
                                                            Action generateFieldList,
                                                            Action generateDefaultFieldMappings,
                                                            GUIStyle boxStyle,
                                                            GUIStyle sectionStyle)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);

            EditorGUILayout.LabelField("<u>2. Select ScriptableObject Type</u>", sectionStyle);

            using (new EditorGUILayout.HorizontalScope(sectionStyle))
            {
                EditorGUI.BeginChangeCheck();
                selectedTypeIndex = EditorGUILayout.Popup("ScriptableObject Type:", selectedTypeIndex, typeNames);

                // Refresh button
                if (GUILayout.Button(new GUIContent("Refresh", EditorGUIUtility.IconContent("d_Refresh").image), GUILayout.Width(80), GUILayout.Height(20)))
                {
                    if (csvHeaders.Count > 0 && selectedScriptableObjectType != null)
                    {
                        generateFieldList(); // This should populate allFieldPaths and allFieldsCache
                        generateDefaultFieldMappings(); // This should handle creating the field mappings
                    }
                }
                if (EditorGUI.EndChangeCheck() && availableTypes.Count > 0 && selectedTypeIndex >= 0 && selectedTypeIndex < availableTypes.Count)
                {
                    selectedScriptableObjectType = availableTypes[selectedTypeIndex];

                    if (csvHeaders.Count > 0)
                    {
                        generateFieldList();
                        generateDefaultFieldMappings();
                    }
                }
            }
        }
        public static void DrawFieldMappings(ref string outputFolder,
                                             Dictionary<string, string> fieldMappings,
                                             List<string> allFieldPaths,
                                             Dictionary<string, FieldInfo> allFieldsCache,
                                             List<string> csvHeaders,
                                             ref Vector2 fieldMappingScrollPosition,
                                             ref string fieldSearchQuery,
                                             GUIStyle boxStyle,
                                             GUIStyle sectionStyle)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);
            EditorGUILayout.LabelField("<u>3. Field Mapping</u>", sectionStyle);
            EditorGUILayout.Space(10);
            DrawOutputFolderSelection(ref outputFolder);
            EditorGUILayout.Space(10);
            s_showMappingSection = EditorGUILayout.Foldout(s_showMappingSection, "Field Mapping Settings", true);
            if (s_showMappingSection)
            {
                using (new EditorGUILayout.VerticalScope(sectionStyle))
                {
                    // Search field for field mapping
                    DrawSearchForMappingScrollView(ref fieldSearchQuery);
                    string searchLower = fieldSearchQuery.ToLowerInvariant();
                    EditorGUILayout.Space(5);

                    // Button to reset field mappings
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.FlexibleSpace(); // Move buttons to the right

                        // Add Auto Fill button
                        if (GUILayout.Button("Trying Auto Fill", GUILayout.Width(120)))
                        {
                            var fieldMappingService = new FieldMappingService();
                            fieldMappingService.TryAutomateFill(allFieldPaths, allFieldsCache, csvHeaders, fieldMappings);
                        }
                        if (GUILayout.Button(new GUIContent("Reset All", EditorGUIUtility.IconContent("d_Refresh").image), GUILayout.Width(80), GUILayout.Height(20)))
                        {
                            if (EditorUtility.DisplayDialog("Reset All Mappings", "Are you sure you want to reset all field mappings?", "Reset", "Cancel"))
                            {
                                fieldMappings.Clear();
                            }
                        }
                    }

                    // Draw dropdown list for field mapping
                    DrawFieldMappingScrollView(fieldMappings,
                                               allFieldPaths,
                                               allFieldsCache,
                                               csvHeaders,
                                               ref fieldMappingScrollPosition,
                                               searchLower);
                }
            }
        }
        public static void DrawPreviewAndProcessButtons(ref bool showPreview,
                                                        List<List<string>> csvData,
                                                        List<string> csvHeaders,
                                                        Dictionary<string, string> fieldMappings,
                                                        Action processCsvToScriptableObjects,
                                                        Action processCsvToScriptableObjectsUpdate,
                                                        GUIStyle boxStyle,
                                                        GUIStyle sectionStyle,
                                                        string outputFolder,
                                                        Type selectedScriptableObjectType)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);
            EditorGUILayout.LabelField("<u>4. Preview and Process</u>", sectionStyle);
            EditorGUILayout.Space(10);
            showPreview = EditorGUILayout.Foldout(showPreview, "Show Data Preview", true);
            EditorGUILayout.Space(10);
            if (showPreview && csvData.Count > 0)
            {
                DrawDataPreview(csvData, csvHeaders, fieldMappings);
            }
            s_RunTest = EditorGUILayout.ToggleLeft(new GUIContent("Run Test", "When enabled, only processes a few rows from CSV to to ensure works as you need before you process large data."),
                                                   s_RunTest);
            EditorGUILayout.Space(10);
            s_Update = EditorGUILayout.ToggleLeft(new GUIContent("Update Existing", "When enabled, will update existing ScriptableObjects instead of creating new ones."), s_Update);
            EditorGUILayout.Space(10);
            if (s_Update)
            {
                DrawIdFieldSelectorUpdate(csvHeaders,
                                          csvHeaders,
                                          ref s_selectedScriptableObjectIdField,
                                          ref s_SelectedCsvIdFields,
                                          boxStyle,
                                          sectionStyle,
                                          selectedScriptableObjectType);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                string suffix = s_RunTest ? " (Test Mode)" : "";

                var backgroundColor = GUI.backgroundColor;
                if (!s_Update)
                {
                    var createButtonStyle = new GUIStyle(GUI.skin.button)
                    {
                        fontSize = 12,
                        fontStyle = FontStyle.Bold,
                        padding = new(12, 12, 8, 8),
                    };
                    Color buttonColor = EditorGUIUtility.isProSkin ? new(0.35f, 0.75f, 0.45f) : new Color(0.25f, 0.65f, 0.35f);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUI.backgroundColor = buttonColor;
                        if (GUILayout.Button(new GUIContent("  Create ScriptableObjects" + suffix, EditorGUIUtility.IconContent("d_CreateAddNew").image), createButtonStyle, GUILayout.Height(36)))
                        {
                            processCsvToScriptableObjects();
                        }
                        GUI.backgroundColor = backgroundColor; // Reset background color
                    }
                }
                else
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        if (s_objectsToUpdateFound.Count > 0)
                        {
                            EditorGUILayout.Space(5);
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                s_showFoundObjects = EditorGUILayout.Foldout(s_showFoundObjects, $"Found Objects to Update ({s_objectsToUpdateFound.Count})", true);
                                EditorGUI.EndDisabledGroup();
                            }
                            if (s_showFoundObjects)
                            {
                                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                                {
                                    // Pagination variables
                                    int itemsPerPage = Math.Min(s_rowsPerPage, s_objectsToUpdateFound.Count);
                                    int totalPages = Mathf.CeilToInt((float)s_objectsToUpdateFound.Count / itemsPerPage);
                                    int startIndex = s_currentPage * itemsPerPage;
                                    int endIndex = Mathf.Min(startIndex + itemsPerPage, s_objectsToUpdateFound.Count);
                                    // Pagination controls
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        GUI.enabled = s_currentPage > 0;
                                        if (GUILayout.Button("<<", GUILayout.Width(25))) s_currentPage = 0;
                                        if (GUILayout.Button("<", GUILayout.Width(25))) s_currentPage--;
                                        GUI.enabled = true;

                                        GUILayout.Label($"Page {s_currentPage + 1}/{totalPages}", EditorStyles.miniLabel);

                                        GUI.enabled = s_currentPage < totalPages - 1;
                                        if (GUILayout.Button(">", GUILayout.Width(25))) s_currentPage++;
                                        if (GUILayout.Button(">>", GUILayout.Width(25))) s_currentPage = totalPages - 1;
                                        GUI.enabled = true;

                                        GUILayout.Label($"Showing {startIndex + 1}-{endIndex} of {s_objectsToUpdateFound.Count}", EditorStyles.miniLabel);
                                        GUILayout.FlexibleSpace();
                                    }
                                    // Display objects with pagination
                                    var paginatedObjects = s_objectsToUpdateFound.Skip(startIndex).Take(itemsPerPage);
                                    foreach (KeyValuePair<ScriptableObject, int> valuePair in paginatedObjects)
                                    {
                                        using (new EditorGUILayout.HorizontalScope())
                                        {
                                            GUILayout.Space(10);
                                            EditorGUI.BeginDisabledGroup(true);
                                            EditorGUILayout.ObjectField(GUIContent.none,
                                                                        valuePair.Key,
                                                                        valuePair.Key.GetType(),
                                                                        false,
                                                                        GUILayout.Height(16));
                                            EditorGUI.EndDisabledGroup();

                                            // Display CSV row number
                                            GUIStyle rowNumberStyle = new(EditorStyles.miniLabel)
                                            {
                                                alignment = TextAnchor.MiddleRight,
                                            };
                                            EditorGUILayout.LabelField($"CSV Row: {valuePair.Value + 1}", rowNumberStyle, GUILayout.Width(80));

                                            if (GUILayout.Button(new GUIContent("", EditorGUIUtility.IconContent("d_ViewToolOrbit").image, "Locate in Project Window"),
                                                                 GUILayout.Width(24),
                                                                 GUILayout.Height(16)))
                                            {
                                                EditorGUIUtility.PingObject(valuePair.Key);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        var previewButtonStyle = new GUIStyle(GUI.skin.button)
                        {
                            fontSize = 11,
                            padding = new(10, 10, 6, 6),
                        };
                        Color buttonPreview = EditorGUIUtility.isProSkin ? new(0.4f, 0.7f, 1.0f) : new Color(0.35f, 0.6f, 0.95f);
                        GUI.backgroundColor = buttonPreview;
                        if (GUILayout.Button(new GUIContent("  Preview Objects Ready For Update", EditorGUIUtility.IconContent("d_ViewToolZoom").image), previewButtonStyle, GUILayout.Height(28)))
                        {
                            var objectsToUpdate = FindScriptableObjectsForUpdate(outputFolder,
                                                                                 selectedScriptableObjectType,
                                                                                 s_selectedScriptableObjectIdField,
                                                                                 s_SelectedCsvIdFields,
                                                                                 csvData,
                                                                                 csvHeaders);

                            if (objectsToUpdate.Count == 0)
                            {
                                EditorUtility.DisplayDialog("No Objects Found", "No matching ScriptableObjects found for update. Make sure your ID fields are correctly set.", "OK");
                                s_objectsToUpdateFound.Clear();
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Objects Found", $"{objectsToUpdate.Count} ScriptableObjects found for update.", "OK");
                                s_objectsToUpdateFound = objectsToUpdate;
                            }
                        }
                        GUI.backgroundColor = backgroundColor; // Reset background color
                        GUILayout.Space(6);
                        var updateButtonStyle = new GUIStyle(GUI.skin.button)
                        {
                            fontSize = 12,
                            fontStyle = FontStyle.Bold,
                            padding = new(12, 12, 8, 8),
                        };
                        Color updateColor = EditorGUIUtility.isProSkin ? new(0.35f, 0.75f, 0.45f) : new Color(0.25f, 0.65f, 0.35f);
                        GUI.backgroundColor = updateColor;
                        if (GUILayout.Button(new GUIContent("  Update ScriptableObjects" + suffix, EditorGUIUtility.IconContent("d_Refresh").image), updateButtonStyle, GUILayout.Height(36)))
                        {
                            processCsvToScriptableObjectsUpdate();
                        }
                        GUI.backgroundColor = backgroundColor; // Reset background color
                    }
                }
            }
        }
        public static Dictionary<ScriptableObject, int> FindScriptableObjectsForUpdate(string outputFolder,
                                                                                       Type scriptableObjectType,
                                                                                       string scriptableObjectIdField,
                                                                                       List<CsvIdFieldConfig> csvIdFields,
                                                                                       List<List<string>> csvData,
                                                                                       List<string> csvHeaders)
        {
            Dictionary<ScriptableObject, int> objectsToUpdate = new();

            // Validate parameters
            if (string.IsNullOrEmpty(scriptableObjectIdField) || csvIdFields.Count == 0 || csvData.Count == 0 || csvHeaders.Count == 0)
            {
                Debug.LogWarning("Missing required parameters for finding objects to update");
                return objectsToUpdate;
            }

            // Fast path for single ID field - no conditions to evaluate
            if (csvIdFields.Count == 1)
            {
                return FindScriptableObjectsWithSingleIdField(outputFolder,
                                                              scriptableObjectType,
                                                              scriptableObjectIdField,
                                                              csvIdFields[0],
                                                              csvData,
                                                              csvHeaders);
            }

            // Find all ScriptableObject assets in the output folder
            if (!Directory.Exists(outputFolder))
            {
                Debug.LogWarning($"Output directory does not exist: {outputFolder}");
                return objectsToUpdate;
            }

            string[] assetGuids = AssetDatabase.FindAssets($"t:{scriptableObjectType.Name}", new[] {outputFolder});

            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (!so) continue;

                // Get the ID value from the ScriptableObject
                object idValue = GetFieldValue(so, scriptableObjectIdField);
                if (idValue == null) continue;

                string soIdString = idValue.ToString();

                // Find matching row for this ScriptableObject
                int matchingRowIndex = FindMatchingRowForScriptableObject(soIdString, csvIdFields, csvData, csvHeaders);

                if (matchingRowIndex >= 0)
                {
                    objectsToUpdate[so] = matchingRowIndex;
                }
            }

            return objectsToUpdate;
        }
        public static void DrawIdFieldSelectorUpdate(List<string> allFieldPaths,
                                                     List<string> csvHeaders,
                                                     ref string selectedScriptableObjectIdField,
                                                     ref List<CsvIdFieldConfig> selectedCsvIdFields,
                                                     GUIStyle boxStyle,
                                                     GUIStyle sectionStyle,
                                                     Type selectedType)
        {
            using var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);
            EditorGUILayout.LabelField("<u>ID Field Selection for Updates</u>", sectionStyle);
            EditorGUILayout.Space(10);
            var iconContent = EditorGUIUtility.IconContent("FilterByLabel");

            // ScriptableObject ID field selector
            const string DefaultIdField = "Select ID field...";
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(iconContent, GUILayout.Width(20));
                EditorGUILayout.LabelField("ScriptableObject ID Field:", GUILayout.Width(150));

                // Use FieldMappingService to get all fields
                List<string> soFieldPaths = new();
                Dictionary<string, FieldInfo> soFieldsCache = new();
                if (selectedType != null)
                {
                    var fieldMappingService = new FieldMappingService();
                    fieldMappingService.CollectAllFields(selectedType, "", soFieldsCache, soFieldPaths);
                }

                // Get current mapped value
                string currentMapping = selectedScriptableObjectIdField ?? "";
                int currentIndex = soFieldPaths.IndexOf(currentMapping);

                // Create options array with "None" as first option
                string[] options = new string[soFieldPaths.Count + 1];
                options[0] = DefaultIdField;
                for (int i = 0; i < soFieldPaths.Count; i++) options[i + 1] = soFieldPaths[i];
                int selectedIndex = EditorGUILayout.Popup(currentIndex + 1, options, GUILayout.Width(200));

                // Update mapping
                selectedScriptableObjectIdField = selectedIndex <= 0 ? null : soFieldPaths[selectedIndex - 1];
            }
            EditorGUILayout.Space(5);

            // CSV ID fields selector
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(iconContent, GUILayout.Width(20));
                EditorGUILayout.LabelField("CSV ID Fields", GUILayout.Width(100));
                using (new EditorGUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Add CSV ID Field", GUILayout.Width(120)))
                    {
                        selectedCsvIdFields.Add(new(csvHeaders.Count > 0 ? csvHeaders[0] : ""));
                    }
                    for (int i = selectedCsvIdFields.Count - 1; i >= 0; i--)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            int currentIndex = csvHeaders.IndexOf(selectedCsvIdFields[i].FieldName);
                            string[] options = new string[csvHeaders.Count + 1];
                            options[0] = "Select CSV column...";
                            csvHeaders.CopyTo(options, 1);

                            // Field selector
                            int newIndex = EditorGUILayout.Popup(currentIndex + 1, options, GUILayout.Width(200));

                            // Match mode selector
                            selectedCsvIdFields[i].MatchMode = (CompositeKeyMatchMode)EditorGUILayout.EnumPopup(selectedCsvIdFields[i].MatchMode, GUILayout.Width(60));

                            bool removeItem = false;
                            if (newIndex <= 0)
                            {
                                removeItem = true;
                            }
                            else
                            {
                                selectedCsvIdFields[i].FieldName = csvHeaders[newIndex - 1];
                            }
                            if (GUILayout.Button("X", GUILayout.Width(25)))
                            {
                                removeItem = true;
                            }
                            if (removeItem)
                            {
                                selectedCsvIdFields.RemoveAt(i);
                            }
                        }
                    }
                }
                GUILayout.FlexibleSpace();
            }
            if (s_Update && (string.IsNullOrEmpty(selectedScriptableObjectIdField) && selectedScriptableObjectIdField != DefaultIdField || selectedCsvIdFields.Count == 0))
            {
                EditorGUILayout.HelpBox("Please select ID fields for both ScriptableObject and CSV to enable updating.", MessageType.Warning);
            }
        }
        public static void DrawStatusMessage(string status, bool hasError, GUIStyle errorStyle)
        {
            if (string.IsNullOrEmpty(status)) return;
            using var verticalScope = new EditorGUILayout.VerticalScope(EditorUIStyles.GetStatusBoxStyle());
            using var horizontalScope = new EditorGUILayout.HorizontalScope();
            GUILayout.Label(EditorGUIUtility.IconContent(hasError ? "console.erroricon" : "console.infoicon"), EditorUIStyles.GetStatusIconStyle());
            EditorGUILayout.LabelField(status, EditorUIStyles.GetStatusMessageStyle(hasError));
            EditorGUILayout.Space(5);
        }
        private static Dictionary<ScriptableObject, int> FindScriptableObjectsWithSingleIdField(string outputFolder,
                                                                                                Type scriptableObjectType,
                                                                                                string scriptableObjectIdField,
                                                                                                CsvIdFieldConfig csvIdField,
                                                                                                List<List<string>> csvData,
                                                                                                List<string> csvHeaders)
        {
            Dictionary<ScriptableObject, int> objectsToUpdate = new();
            // Create mapping from ID values to row indices and track duplicates
            Dictionary<string, List<int>> idToRowIndices = new();
            int columnIndex = csvHeaders.IndexOf(csvIdField.FieldName);
            if (columnIndex >= 0)
            {
                for (int rowIndex = 0; rowIndex < csvData.Count; rowIndex++)
                {
                    var row = csvData[rowIndex];
                    if (columnIndex < row.Count)
                    {
                        string keyValue = row[columnIndex];

                        if (!idToRowIndices.ContainsKey(keyValue))
                        {
                            idToRowIndices[keyValue] = new();
                        }
                        idToRowIndices[keyValue].Add(rowIndex);
                    }
                }
            }
            if (!Directory.Exists(outputFolder)) return objectsToUpdate;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{scriptableObjectType.Name}", new[] {outputFolder});
            foreach (string guid in assetGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (!so) continue;

                object idValue = GetFieldValue(so, scriptableObjectIdField);
                if (idValue == null) continue;

                string soIdString = idValue.ToString();
                if (idToRowIndices.TryGetValue(soIdString, out var rowIndices) && rowIndices.Count > 0)
                {
                    // Warn about multiple matches
                    if (rowIndices.Count > 1)
                    {
                        string matchingRowsList = string.Join(", ", rowIndices.Select(i => (i + 1).ToString()));
                        Debug.LogWarning($"Multiple matches found for ID '{soIdString}' in CSV rows: {matchingRowsList}. Using first match (row {rowIndices[0] + 1}).");
                    }
                    // Use first match
                    objectsToUpdate[so] = rowIndices[0];
                }
            }
            return objectsToUpdate;
        }
        private static int FindMatchingRowForScriptableObject(string soIdString,
                                                              List<CsvIdFieldConfig> csvIdFields,
                                                              List<List<string>> csvData,
                                                              List<string> csvHeaders)
        {
            // Build mapping of column indices for each field
            Dictionary<int, CompositeKeyMatchMode> fieldColumnIndices = new();
            foreach (var field in csvIdFields)
            {
                int columnIndex = csvHeaders.IndexOf(field.FieldName);
                if (columnIndex >= 0)
                {
                    fieldColumnIndices[columnIndex] = field.MatchMode;
                }
            }

            int firstMatchIndex = -1;
            // Check each row for a match
            for (int rowIndex = 0; rowIndex < csvData.Count; rowIndex++)
            {
                var row = csvData[rowIndex];
                if (row.Count < csvHeaders.Count) continue;

                // For composite keys, evaluate pairwise conditions
                bool isMatch = EvaluateCompositePairwiseConditions(soIdString, row, fieldColumnIndices);

                if (isMatch)
                {
                    if (firstMatchIndex == -1)
                    {
                        firstMatchIndex = rowIndex;
                    }
                }
            }
            return firstMatchIndex; // Return first match or -1 if no matches found
        }
        private static bool EvaluateCompositePairwiseConditions(string soIdString,
                                                                List<string> rowData,
                                                                Dictionary<int, CompositeKeyMatchMode> fieldColumnIndices)
        {
            // Get all column indices in the order they appear in the CSV
            var orderedColumns = fieldColumnIndices.Keys.OrderBy(k => k).ToList();

            if (orderedColumns.Count == 0) return false;

            // If there's only one condition, just check if the value matches
            if (orderedColumns.Count == 1)
            {
                int column = orderedColumns[0];
                return column < rowData.Count && rowData[column] == soIdString;
            }

            // For multiple columns, evaluate pairwise conditions
            bool result = false;
            bool firstCondition = true;

            // Process all columns except the last one (which has no next element to compare with)
            for (int i = 0; i < orderedColumns.Count - 1; i++)
            {
                int currentColumn = orderedColumns[i];
                int nextColumn = orderedColumns[i + 1];

                bool currentMatches = currentColumn < rowData.Count && rowData[currentColumn] == soIdString;
                bool nextMatches = nextColumn < rowData.Count && rowData[nextColumn] == soIdString;

                // Get the match mode (AND/OR) of the current column
                CompositeKeyMatchMode matchMode = fieldColumnIndices[currentColumn];

                // Apply the logical operation based on match mode
                bool pairResult;
                if (matchMode == CompositeKeyMatchMode.And)
                {
                    pairResult = currentMatches && nextMatches;
                }
                else // Or
                {
                    pairResult = currentMatches || nextMatches;
                }

                // Combine with previous results
                if (firstCondition)
                {
                    result = pairResult;
                    firstCondition = false;
                }
                else
                {
                    // We chain results together
                    result = result && pairResult;
                }
            }

            return result;
        }
        /// <summary> Gets the value of a field from an object, supporting nested fields with dot notation </summary>
        private static object GetFieldValue(object target, string fieldPath)
        {
            if (target == null || string.IsNullOrEmpty(fieldPath)) return null;

            // Handle nested fields (e.g. "stats.health")
            string[] pathParts = fieldPath.Split('.');
            object current = target;

            foreach (string part in pathParts)
            {
                FieldInfo field = current.GetType()
                                         .GetField(part,
                                                   BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (field == null) return null;

                current = field.GetValue(current);
                if (current == null) return null;
            }

            return current;
        }
        private static void DrawSearchForMappingScrollView(ref string fieldSearchQuery)
        {
            // Search field container with indentation
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Search Field:", GUILayout.Width(80));

                // Container for search field and icon
                using (new EditorGUILayout.HorizontalScope())
                {
                    // Search field style
                    var searchStyle = new GUIStyle(EditorStyles.textField)
                    {
                        margin = new(0, 0, 2, 2),
                        normal =
                        {
                            textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black,
                        },
                    };

                    // Search input field
                    EditorGUI.BeginChangeCheck();
                    string newSearchQuery = EditorGUILayout.TextField(fieldSearchQuery, searchStyle);
                    if (EditorGUI.EndChangeCheck())
                    {
                        fieldSearchQuery = newSearchQuery;
                    }
                }
            }
        }
        private static char DrawDelimiterSelector(char currentDelimiter)
        {
            GUILayout.Space(10); // Top margin
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(30); // Left indentation
                EditorGUILayout.LabelField("Delimiter:", GUILayout.Width(120));

                string[] commonDelimiters = {",", ";", "|", "\t"};
                string[] displayOptions = {"Comma (,)", "Semicolon (;)", "Pipe (|)", "Tab (\\t)", "Custom..."};

                string delimiterStr = currentDelimiter.ToString();

                // Initialize index if not set
                if (s_lastSelectedIndexDelimiter == -1)
                {
                    s_lastSelectedIndexDelimiter = Array.IndexOf(commonDelimiters, delimiterStr);
                    if (s_lastSelectedIndexDelimiter == -1) s_lastSelectedIndexDelimiter = displayOptions.Length - 1;
                }

                // Display dropdown and get new selection
                int newIndex = EditorGUILayout.Popup(s_lastSelectedIndexDelimiter, displayOptions, GUILayout.Width(120));

                // Update stored index
                s_lastSelectedIndexDelimiter = newIndex;

                // Handle custom option
                if (newIndex == displayOptions.Length - 1)
                {
                    string customInput = EditorGUILayout.TextField(delimiterStr, GUILayout.Width(50));
                    return !string.IsNullOrEmpty(customInput) ? customInput[0] : currentDelimiter;
                }

                // Handle standard delimiters
                return commonDelimiters[newIndex][0];
            }
        }
        private static char DrawQuoteSelector(char currentQuoteChar)
        {
            GUILayout.Space(10); // Add space from top edge
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(30); // Add left indentation
                EditorGUILayout.LabelField("Quote Character:", GUILayout.Width(120));

                string[] commonQuotes = {"\"", "'"};
                string[] displayQuotes = {"Double quote (\")", "Single quote (')", "Custom..."};

                string quoteStr = currentQuoteChar.ToString();

                // Initialize index if not set
                if (s_lastSelectedIndexQuote == -1)
                {
                    s_lastSelectedIndexQuote = Array.IndexOf(commonQuotes, quoteStr);
                    if (s_lastSelectedIndexQuote == -1) s_lastSelectedIndexQuote = displayQuotes.Length - 1;
                }

                // Display dropdown and get new selection
                int newIndex = EditorGUILayout.Popup(s_lastSelectedIndexQuote, displayQuotes, GUILayout.Width(120));

                // Update stored index
                s_lastSelectedIndexQuote = newIndex;

                // Handle custom option
                if (newIndex == displayQuotes.Length - 1)
                {
                    string customInput = EditorGUILayout.TextField(quoteStr, GUILayout.Width(50));
                    return !string.IsNullOrEmpty(customInput) ? customInput[0] : currentQuoteChar;
                }

                // Handle standard quote characters
                return commonQuotes[newIndex][0];
            }
        }
        private static void DrawReloadButton(string csvFilePath, Action<string> loadCsvFile)
        {
            using var horizontalScope = new EditorGUILayout.HorizontalScope();

            using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(csvFilePath)))
            {
                GUILayout.Space(40); // Add left margin
                if (GUILayout.Button(new GUIContent("Reload CSV", EditorGUIUtility.IconContent("Refresh").image), GUILayout.Width(100), GUILayout.Height(20)))
                {
                    try
                    {
                        loadCsvFile?.Invoke(csvFilePath);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to reload CSV: {e.Message}");
                        EditorUtility.DisplayDialog("Error", $"Failed to reload CSV: {e.Message}", "OK");
                    }
                }
            }

            GUILayout.FlexibleSpace();
        }
        private static void DrawOutputFolderSelection(ref string outputFolder)
        {
            using var horizontalScope = new EditorGUILayout.HorizontalScope();

            // Label with bold style
            EditorGUILayout.LabelField("Output Directory:", GUILayout.Width(120));

            // Calculate width for text field
            float textFieldWidth = EditorGUIUtility.currentViewWidth - 250f; // Accounts for label and button

            // Text field with custom style and calculated width
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.indentLevel = 0;
                outputFolder = EditorGUILayout.TextField(outputFolder, GUILayout.Width(textFieldWidth));
            }

            // Browse button with icon
            if (GUILayout.Button(new GUIContent("Browse...", EditorGUIUtility.IconContent("Folder Icon").image),
                                 GUILayout.Width(80),
                                 GUILayout.Height(18)))
            {
                string path = EditorUtility.SaveFolderPanel("Select Output Directory", outputFolder, "");
                if (!string.IsNullOrEmpty(path))
                {
                    outputFolder = ConvertToAssetsRelativePath(path);
                }
            }
        }
        private static string ConvertToAssetsRelativePath(string absolutePath)
        {
            return absolutePath.StartsWith(Application.dataPath)
                           ? "Assets" + absolutePath.Substring(Application.dataPath.Length)
                           : absolutePath;
        }
        private static void DrawFieldMappingScrollView(Dictionary<string, string> fieldMappings,
                                                       List<string> allFieldPaths,
                                                       Dictionary<string, FieldInfo> allFieldsCache,
                                                       List<string> csvHeaders,
                                                       ref Vector2 fieldMappingScrollPosition,
                                                       string searchLower)
        {
            fieldMappingScrollPosition = EditorGUILayout.BeginScrollView(fieldMappingScrollPosition, GUILayout.Height(300));
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                foreach (string fieldPath in allFieldPaths)
                {
                    if (!string.IsNullOrEmpty(searchLower) && !fieldPath.ToLowerInvariant().Contains(searchLower)) continue;

                    if (allFieldsCache.TryGetValue(fieldPath, out FieldInfo fieldInfo))
                    {
                        DrawFieldMapping(fieldPath, fieldInfo, fieldMappings, csvHeaders, searchLower);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
        private static void DrawFieldMapping(string fieldPath,
                                             FieldInfo fieldInfo,
                                             Dictionary<string, string> fieldMappings,
                                             List<string> csvHeaders,
                                             string searchLower)
        {
            using var horizontalScope = new EditorGUILayout.HorizontalScope();

            float totalWidth = EditorGUIUtility.currentViewWidth - 60f;

            // Create highlighted field content
            string displayText = fieldPath;
            if (!string.IsNullOrEmpty(searchLower))
            {
                int index = fieldPath.ToLowerInvariant().IndexOf(searchLower, StringComparison.Ordinal);
                if (index >= 0)
                {
                    string before = fieldPath[..index];
                    string match = fieldPath.Substring(index, searchLower.Length);
                    string after = fieldPath[(index + searchLower.Length)..];
                    displayText = $"{before}<color=#FFA500FF>{match}</color>{after}";
                }
            }

            // Field info with highlighting
            GUIContent fieldContent = new($"{displayText} ({fieldInfo.FieldType.Name})");
            EditorGUILayout.LabelField(fieldContent,
                                       new GUIStyle(EditorStyles.label)
                                       {
                                           richText = true,
                                       },
                                       GUILayout.Width(totalWidth * 0.5f));

            // Dropdown remains unchanged
            string currentMapping = fieldMappings.ContainsKey(fieldPath) ? fieldMappings[fieldPath] : "";
            int currentIndex = csvHeaders.IndexOf(currentMapping);
            string[] options = CreateMappingOptions(csvHeaders);
            int selectedIndex = EditorGUILayout.Popup(currentIndex + 1, options, GUILayout.Width(totalWidth * 0.5f - 10f));
            UpdateFieldMapping(fieldPath, selectedIndex, csvHeaders, fieldMappings);
        }
        private static string[] CreateMappingOptions(List<string> csvHeaders)
        {
            string[] options = new string[csvHeaders.Count + 1];
            options[0] = "None";
            csvHeaders.CopyTo(options, 1);
            return options;
        }
        private static void UpdateFieldMapping(string fieldPath,
                                               int selectedIndex,
                                               List<string> csvHeaders,
                                               Dictionary<string, string> fieldMappings)
        {
            if (selectedIndex <= 0)
            {
                fieldMappings.Remove(fieldPath);
            }
            else
            {
                fieldMappings[fieldPath] = csvHeaders[selectedIndex - 1];
            }
        }
        private static void DrawDataPreview(List<List<string>> csvData, List<string> csvHeaders, Dictionary<string, string> fieldMappings)
        {
            if (csvData.Count == 0 || csvHeaders.Count == 0) return;

            // Cache frequently used values
            int totalRows = csvData.Count;
            int columnCount = csvHeaders.Count;
            float totalWidth = EditorGUIUtility.currentViewWidth - 50;
            float columnWidth = Mathf.Max(80, totalWidth / Math.Min(columnCount, 7));
            // Calculate pagination values only once
            int totalPages = Mathf.CeilToInt((float)totalRows / s_rowsPerPage);
            if (s_currentPage >= totalPages && totalPages > 0) s_currentPage = totalPages - 1;
            int startIndex = s_currentPage * s_rowsPerPage;
            int endIndex = Mathf.Min(startIndex + s_rowsPerPage, totalRows);
            // Process mapping data outside of draw loop
            HashSet<string> mappedColumns = new(fieldMappings.Values);
            Dictionary<string, List<string>> columnToFieldsMap = new();
            foreach (var mapping in fieldMappings)
            {
                string csvColumn = mapping.Value;
                if (!columnToFieldsMap.TryGetValue(csvColumn, out var fields))
                {
                    columnToFieldsMap[csvColumn] = new();
                }
                columnToFieldsMap[csvColumn].Add(mapping.Key);
            }
            // Precache header content to avoid GC during rendering
            GUIContent[] headerContents = new GUIContent[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                string header = csvHeaders[i];
                bool isColumnMapped = mappedColumns.Contains(header);

                string headerText;
                if (isColumnMapped && columnToFieldsMap.TryGetValue(header, out var fields))
                {
                    Color greenColor = EditorGUIUtility.isProSkin ? new(0.3f, 0.95f, 0.7f) : new Color(0.0f, 0.5f, 0.0f);
                    string colorHex = ColorUtility.ToHtmlStringRGB(greenColor);

                    // Limit number of mapped fields shown to reduce height
                    string mappedFields = string.Join("\n→ ", fields.Count > 3 ? fields.Take(3).Append("...") : fields);
                    headerText = $"<color=#{colorHex}><b>✓ {header}</b></color>\n→ {mappedFields}";
                }
                else
                {
                    Color warningColor = EditorGUIUtility.isProSkin ? new(1f, 0.7f, 0.3f) : new Color(0.8f, 0.4f, 0.0f);
                    string colorHex = ColorUtility.ToHtmlStringRGB(warningColor);
                    headerText = $"<color=#{colorHex}><b>⚠ {header}\n(not mapped)</b></color>";
                }

                headerContents[i] = new(headerText);
            }
            // Cache row colors and styles
            GUIStyle evenRowStyle = EditorUIStyles.GetTableRowEvenStyle();
            GUIStyle oddRowStyle = EditorUIStyles.GetTableRowOddStyle();
            GUIStyle headerCellStyle = EditorUIStyles.GetTableHeaderCellStyle();
            GUIStyle cellStyle = EditorUIStyles.GetTableCellStyle();
            GUIStyle cellStyleBold = new(cellStyle)
            {
                fontStyle = FontStyle.Bold,
            };
            // Cache pagination strings
            string pageText = $"Page {s_currentPage + 1}/{totalPages}";
            string rowsText = $"Showing rows {startIndex + 1}-{endIndex} of {totalRows}";
            // Begin custom table layout with ScrollView that has a fixed height
            s_previewScrollPosition = EditorGUILayout.BeginScrollView(s_previewScrollPosition,
                                                                      EditorStyles.helpBox,
                                                                      GUILayout.Height(Math.Min(600, 16 + (endIndex - startIndex) * 21 + 60)));
            // Draw pagination controls
            DrawDataPreviewPagination(totalPages, pageText, rowsText);
            // Use GUILayout.BeginHorizontal/EndHorizontal with custom style for header row
            using (new EditorGUILayout.HorizontalScope(EditorUIStyles.GetTableHeaderStyle()))
            {
                // Draw cached header contents
                for (int i = 0; i < columnCount; i++)
                {
                    EditorGUILayout.LabelField(headerContents[i], headerCellStyle, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                }
            }
            // Virtualize rendering for large datasets
            for (int rowIdx = startIndex; rowIdx < endIndex; rowIdx++)
            {
                GUIStyle rowStyle = rowIdx % 2 == 0 ? evenRowStyle : oddRowStyle;

                using (new EditorGUILayout.HorizontalScope(rowStyle))
                {
                    var row = csvData[rowIdx];

                    // Draw each cell in the row
                    for (int colIdx = 0; colIdx < columnCount; colIdx++)
                    {
                        string cellValue = colIdx < row.Count ? row[colIdx] : "";
                        bool isColumnMapped = mappedColumns.Contains(csvHeaders[colIdx]);
                        // Use precached style
                        EditorGUILayout.LabelField(cellValue, isColumnMapped ? cellStyleBold : cellStyle, GUILayout.Width(columnWidth), GUILayout.ExpandWidth(true));
                    }
                }
            }
            // Add compact legend
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUIStyle legendStyle = new(EditorStyles.miniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    padding = new(5, 5, 2, 2),
                    alignment = TextAnchor.MiddleLeft,
                    wordWrap = true,
                    richText = true,
                };
                EditorGUILayout.LabelField("<b>✓</b> Column mapped | <b>→</b> Target field", legendStyle);
            }
            EditorGUILayout.EndScrollView();
        }
        private static void DrawDataPreviewPagination(int totalPages, string pageText, string rowsText)
        {
            // Cache button styles to avoid recreating them
            const float buttonWidth = 25f;
            GUIStyle paginationStyle = EditorUIStyles.GetPaginationStyle();

            using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
            {
                // First page button
                GUI.enabled = s_currentPage > 0;
                if (GUILayout.Button("<<", paginationStyle, GUILayout.Width(buttonWidth)))
                {
                    s_currentPage = 0;
                }
                // Previous page button
                if (GUILayout.Button("<", paginationStyle, GUILayout.Width(buttonWidth)))
                {
                    s_currentPage--;
                }
                GUI.enabled = true;
                // Page indicator (fixed width)
                GUILayout.Label(pageText, EditorStyles.miniLabel, GUILayout.Width(70));
                // Next page button
                GUI.enabled = s_currentPage < totalPages - 1;
                if (GUILayout.Button(">", paginationStyle, GUILayout.Width(buttonWidth)))
                {
                    s_currentPage++;
                }
                // Last page button
                if (GUILayout.Button(">>", paginationStyle, GUILayout.Width(buttonWidth)))
                {
                    s_currentPage = totalPages - 1;
                }
                GUI.enabled = true;
                GUILayout.Space(10);
                // Rows per page selector
                EditorGUILayout.LabelField("Rows per page:", EditorStyles.miniLabel);
                // Slider for rows per page
                EditorGUI.BeginChangeCheck();
                int newRowsPerPage = EditorGUILayout.IntSlider(s_rowsPerPage, 1, 50);
                if (newRowsPerPage != s_rowsPerPage)
                {
                    s_rowsPerPage = newRowsPerPage;
                    s_currentPage = 0; // Reset to first page
                }
                GUILayout.Space(10);
                EditorGUILayout.LabelField(rowsText, EditorStyles.miniLabel);
                GUILayout.FlexibleSpace();
            }
        }
    }
    public enum CompositeKeyMatchMode
    {
        /// <summary> All ID fields must match </summary>
        And,
        /// <summary> Any ID field can match </summary>
        Or,
    }
    public class CsvIdFieldConfig
    {
        public CsvIdFieldConfig(string fieldName, CompositeKeyMatchMode matchMode = CompositeKeyMatchMode.And)
        {
            this.FieldName = fieldName;
            this.MatchMode = matchMode;
        }
        public string FieldName { get; set; }
        public CompositeKeyMatchMode MatchMode { get; set; }
    }
}
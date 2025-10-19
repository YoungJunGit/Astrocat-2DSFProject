using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSV_To_ScriptableObject.Editor.Window;
using UnityEditor;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Services
{
    /// <summary> Result of processing CSV data into ScriptableObjects </summary>
    public class ProcessResult
    {
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<string> ErrorMessages { get; set; } = new();
    }
    /// <summary> Service for creating ScriptableObject instances from CSV data </summary>
    public class ScriptableObjectProcessor
    {
        private ValueConverter _valueConverter;
        public ProcessResult ProcessCsvData(List<List<string>> csvData,
                                            List<string> csvHeaders,
                                            Type scriptableObjectType,
                                            Dictionary<string, string> fieldMappings,
                                            string outputFolder,
                                            Dictionary<string, FieldInfo> allFieldsCache,
                                            string splitter,
                                            string csvFilePath,
                                            bool update)
        {
            var result = new ProcessResult();
            this._valueConverter = new(splitter);
            // Start asset editing to prevent Unity from trying to save assets while we are processing
            AssetDatabase.StartAssetEditing();
            try
            {
                EnsureDirectoryExists(outputFolder);
                bool cancelled = this.ProcessRows(csvData,
                                                    csvHeaders,
                                                    scriptableObjectType,
                                                    fieldMappings,
                                                    outputFolder,
                                                    splitter,
                                                    csvFilePath,
                                                    result,
                                                    update);
                if (!cancelled)
                {
                    SaveChangesToAssetDatabase();
                }
            }
            catch (Exception ex)
            {
                HandleGlobalException(ex, result);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
            }

            return result;
        }
        private static string SanitizeFilename(string input)
        {
            string sanitized = input.Replace(" ", "_").Replace("/", "_");
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                sanitized = sanitized.Replace(c, '_');
            }
            return sanitized;
        }
        private bool ProcessRows(List<List<string>> csvData,
                                 List<string> csvHeaders,
                                 Type scriptableObjectType,
                                 Dictionary<string, string> fieldMappings,
                                 string outputFolder,
                                 string splitter,
                                 string csvFilePath,
                                 ProcessResult result,
                                 bool update)
        {
            try
            {
                // If in update mode, first find objects to update
                Dictionary<ScriptableObject, int> objectsToUpdate = new();
                if (update && EditorUI.s_selectedScriptableObjectIdField != null && EditorUI.s_SelectedCsvIdFields.Count > 0)
                {
                    objectsToUpdate = EditorUI.FindScriptableObjectsForUpdate(outputFolder,
                                                                              scriptableObjectType,
                                                                              EditorUI.s_selectedScriptableObjectIdField,
                                                                              EditorUI.s_SelectedCsvIdFields,
                                                                              csvData,
                                                                              csvHeaders);
                }
                float totalRows = csvData.Count;
                bool wasCanceled = false;
                for (int rowIndex = 0; rowIndex < csvData.Count && !wasCanceled; rowIndex++)
                {
                    try
                    {
                        // Show progress bar
                        float progress = rowIndex / totalRows;
                        bool canceled = EditorUtility.DisplayCancelableProgressBar("Processing CSV", $"Processing row {rowIndex + 1} of {totalRows}", progress);
                        if (canceled)
                        {
                            wasCanceled = true;
                            Debug.Log("Processing canceled by user");
                            result.ErrorMessages.Add("Processing canceled by user");
                            result.ErrorCount++;
                            return true;
                        }

                        // Testing case to process only a few rows
                        if (EditorUI.s_RunTest && rowIndex == 5) return false;

                        // Process each row of CSV data
                        var row = csvData[rowIndex];
                        ScriptableObject so;

                        if (update)
                        {
                            // Try to find existing object for this row
                            so = objectsToUpdate.FirstOrDefault(x => x.Value == rowIndex).Key;

                            // If no existing object found and we're in update mode, skip creating new one
                            if (!so)
                            {
                                Debug.LogWarning($"Row {rowIndex + 1}: No existing object found for update.");
                                continue;
                            }
                        }
                        else
                        {
                            // Create new instance if not updating
                            so = ScriptableObject.CreateInstance(scriptableObjectType);
                        }

                        string assetName = this.ProcessRowFields(so,
                                                                 row,
                                                                 csvHeaders,
                                                                 fieldMappings,
                                                                 rowIndex,
                                                                 splitter,
                                                                 csvFilePath,
                                                                 result);

                        if (!update)
                        {
                            // Save new asset only if not updating
                            string assetPath = SaveScriptableObject(so, assetName, rowIndex, outputFolder);
                            result.SuccessCount++;
                            Debug.Log($"Successfully created asset: {assetPath}");
                        }
                        else if (so)
                        {
                            // For updates, mark object as dirty and increment success counter
                            EditorUtility.SetDirty(so);
                            result.SuccessCount++;
                            Debug.Log($"Successfully updated asset: {AssetDatabase.GetAssetPath(so)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        HandleRowException(ex, rowIndex, result);
                    }
                }

                return wasCanceled;
            }
            finally
            {
                // Always clear progress bar
                EditorUtility.ClearProgressBar();
            }
        }
        private string ProcessRowFields(ScriptableObject so,
                                        List<string> row,
                                        List<string> csvHeaders,
                                        Dictionary<string, string> fieldMappings,
                                        int rowIndex,
                                        string splitter,
                                        string csvFilePath,
                                        ProcessResult result)
        {
            string assetName = Path.GetFileNameWithoutExtension(csvFilePath);

            foreach (var mapping in fieldMappings.Where(m => !string.IsNullOrEmpty(m.Value)))
            {
                int csvColumnIndex = csvHeaders.IndexOf(mapping.Value);
                if (csvColumnIndex >= 0 && csvColumnIndex < row.Count)
                {
                    string fieldPath = mapping.Key;
                    string csvValue = row[csvColumnIndex];
                    try
                    {
                        if (this.ApplyValueToField(so, fieldPath, csvValue, splitter))
                        {
                            // Successfully set the field value
                        }
                        else
                        {
                            Debug.LogWarning($"Row {rowIndex + 1}: Failed to set {fieldPath} to '{csvValue}'");
                            string detailedError = $"Row {rowIndex + 1}: Failed to set {fieldPath} to '{csvValue}'";
                            result.ErrorMessages.Add(detailedError);
                        }
                    }
                    catch (Exception fieldEx)
                    {
                        string detailedError = $"Error setting field '{fieldPath}' to value '{csvValue}': {fieldEx.Message}";
                        Debug.LogError(detailedError);
                        throw new(detailedError, fieldEx);
                    }
                }
            }
            return assetName;
        }
        private static string SaveScriptableObject(ScriptableObject so, string csvBaseName, int rowIndex, string outputFolder)
        {
            string assetName = $"{csvBaseName}_{rowIndex + 1}";
            string assetPath = $"{outputFolder}/{SanitizeFilename(assetName)}.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(so, assetPath);
            return assetPath;
        }
        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        private static void SaveChangesToAssetDatabase()
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        private static void HandleRowException(Exception ex, int rowIndex, ProcessResult result)
        {
            string errorDetail = $"Row {rowIndex + 1}: {ex.Message}";
            if (ex.InnerException != null) errorDetail += $"\nInner exception: {ex.InnerException.Message}";
            result.ErrorMessages.Add(errorDetail);
            Debug.LogError($"Error processing row {rowIndex + 1}: {ex}");
            result.ErrorCount++;
        }
        private static void HandleGlobalException(Exception ex, ProcessResult result)
        {
            Debug.LogError($"Global error processing CSV: {ex}");
            result.ErrorMessages.Add($"Global error: {ex.Message}");
            result.ErrorCount++;
        }
        private bool ApplyValueToField(object target, string fieldPath, string value, string splitCollectionOrArray)
        {
            try
            {
                if (target == null || string.IsNullOrEmpty(fieldPath)) return false;
                // Direct field (no nesting)
                if (!fieldPath.Contains("."))
                {
                    FieldInfo fieldInfo = target.GetType()
                                                .GetField(fieldPath, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo == null) return false;
                    object convertedValue = this._valueConverter.ConvertValueToType(value, fieldInfo.FieldType, splitCollectionOrArray);
                    if (convertedValue != null)
                    {
                        fieldInfo.SetValue(target, convertedValue);
                        return true;
                    }
                    return false;
                }
                // Process nested path
                string[] pathParts = fieldPath.Split('.');
                object current = target;
                // Navigate to parent object
                for (int i = 0; i < pathParts.Length - 1; i++)
                {
                    FieldInfo field = current.GetType()
                                             .GetField(pathParts[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (field == null) return false;

                    object fieldValue = field.GetValue(current);

                    // Create instance if null
                    if (fieldValue == null)
                    {
                        fieldValue = Activator.CreateInstance(field.FieldType);
                        field.SetValue(current, fieldValue);
                    }

                    current = fieldValue;
                }
                // Set final field
                string finalFieldName = pathParts[pathParts.Length - 1];
                FieldInfo finalField = current.GetType()
                                              .GetField(finalFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (finalField == null) return false;
                object finalValue = this._valueConverter.ConvertValueToType(value, finalField.FieldType, splitCollectionOrArray);
                if (finalValue != null)
                {
                    finalField.SetValue(current, finalValue);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error setting field {fieldPath}: {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Services
{
    /// <summary> Service for managing field mappings between CSV and ScriptableObject </summary>
    public class FieldMappingService
    {
        public void CollectAllFields(Type type,
                                     string basePath,
                                     Dictionary<string, FieldInfo> allFieldsCache,
                                     List<string> allFieldPaths,
                                     HashSet<Type> visitedTypes = null)
        {
            // Initialize visited types tracking set if this is the top-level call
            visitedTypes ??= new();

            // If we've already processed this type in this branch of recursion, skip it
            if (!visitedTypes.Add(type))
                return;

            try
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                 .Where(field => field.IsPublic || field.GetCustomAttributes(typeof(SerializeField), true).Length > 0)
                                 .ToList();

                foreach (var field in fields)
                {
                    string fieldPath = string.IsNullOrEmpty(basePath) ? field.Name : $"{basePath}.{field.Name}";

                    if (this.IsCollectionType(field.FieldType) || this.IsBasicType(field.FieldType))
                    {
                        allFieldsCache[fieldPath] = field;
                        allFieldPaths.Add(fieldPath);
                        continue;
                    }
                    if (this.IsSerializedClass(field.FieldType))
                    {
                        // Create a new set for child recursion that includes current type
                        HashSet<Type> childVisitedTypes = new HashSet<Type>(visitedTypes);

                        // Recursively collect fields from nested classes
                        this.CollectAllFields(field.FieldType, fieldPath, allFieldsCache, allFieldPaths, childVisitedTypes);
                    }
                }
            }
            finally
            {
                // Remove the current type from the visited set when we're done with this branch
                visitedTypes.Remove(type);
            }
        }
        public void GenerateDefaultFieldMappings(List<string> allFieldPaths,
                                                 Dictionary<string, FieldInfo> allFieldsCache,
                                                 List<string> csvHeaders,
                                                 Dictionary<string, string> fieldMappings)
        {
            // First set all fields to "none"
            foreach (var fieldPath in allFieldPaths) fieldMappings[fieldPath] = "none";
        }
        public void TryAutomateFill(List<string> allFieldPaths, Dictionary<string, FieldInfo> allFieldsCache, List<string> csvHeaders, Dictionary<string, string> fieldMappings)
        {
            // Automatically match fields to CSV headers
            foreach (var fieldPath in allFieldPaths)
            {
                if (!allFieldsCache.TryGetValue(fieldPath, out FieldInfo field)) continue;
                if (this.TryMatchCollectionType(field, fieldPath, csvHeaders, fieldMappings)) continue;
                if (this.IsSerializedClass(field.FieldType) && !this.IsBasicType(field.FieldType)) continue;
                TryMatchSimpleField(fieldPath, field, csvHeaders, fieldMappings);
                TryMatchNestedField(fieldPath, csvHeaders, fieldMappings);
            }
        }
        public bool IsSerializedClass(Type type)
        {
            if (!type.IsClass) return false;

            // Must be assigned from UnityEngine.Object
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return true;

            // Check if the class is marked as serializable
            return Attribute.IsDefined(type, typeof(SerializableAttribute));
        }
        public bool IsBasicType(Type type)
        {
            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(Vector2)
                || type == typeof(Vector3)
                || type == typeof(Color)
                || type == typeof(Enum)
                || type.IsEnum;
        }
        public bool IsCollectionType(Type type)
        {
            return type.IsArray || type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }
        private static string FindBestMatchingHeader(string fieldName, List<string> csvHeaders)
        {
            // First try exact match (case-insensitive)
            string matchedHeader = csvHeaders.FirstOrDefault(h => string.Equals(h, fieldName, StringComparison.OrdinalIgnoreCase));

            // If no match, find the closest name using Levenshtein distance
            if (matchedHeader == null)
            {
                string fieldCompare = fieldName.StartsWith("_") ? fieldName.Substring(1) : fieldName;
                int bestDistance = int.MaxValue;
                foreach (var header in csvHeaders)
                {
                    int distance = LevenshteinDistance(fieldCompare.ToLowerInvariant(), header.ToLowerInvariant());
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        matchedHeader = header;
                    }
                }

                // Only use the match if it's reasonably close
                if (bestDistance > Math.Min(fieldCompare.Length, matchedHeader?.Length ?? 0) / 2)
                {
                    matchedHeader = null;
                }
            }
            return matchedHeader ?? "";
        }
        private static int LevenshteinDistance(string a, string b)
        {
            if (string.IsNullOrEmpty(a)) return b?.Length ?? 0;
            if (string.IsNullOrEmpty(b)) return a.Length;
            var distances = new int[a.Length + 1, b.Length + 1];
            for (int i = 0; i <= a.Length; i++) distances[i, 0] = i;
            for (int j = 0; j <= b.Length; j++) distances[0, j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    distances[i, j] = Math.Min(Math.Min(distances[i - 1, j] + 1, distances[i, j - 1] + 1), distances[i - 1, j - 1] + cost);
                }
            }
            return distances[a.Length, b.Length];
        }
        private static void TryMatchSimpleField(string fieldPath, FieldInfo field, List<string> csvHeaders, Dictionary<string, string> fieldMappings)
        {
            string matchedHeader = FindBestMatchingHeader(field.Name, csvHeaders);
            if (!string.IsNullOrEmpty(matchedHeader))
            {
                fieldMappings[fieldPath] = matchedHeader;
            }
        }
        private static void TryMatchNestedField(string fieldPath, List<string> csvHeaders, Dictionary<string, string> fieldMappings)
        {
            string[] parts = fieldPath.Split('.');
            string fieldName = parts[^1]; // Last part
            foreach (string csvHeader in csvHeaders.Where(csvHeader => csvHeader.Contains(fieldName))) fieldMappings[fieldPath] = csvHeader;
        }
        private bool TryMatchCollectionType(FieldInfo field, string fieldPath, List<string> csvHeaders, Dictionary<string, string> fieldMappings)
        {
            if (!this.IsCollectionType(field.FieldType)) return false;

            // Clean field name (remove leading underscore if present)
            string baseName = field.Name;
            if (baseName.StartsWith("_")) baseName = baseName.Substring(1);

            // For nested fields, also try with the last part of the path
            string lastPartName = fieldPath.Contains(".") ? fieldPath.Split('.').Last() : "";
            if (lastPartName.StartsWith("_")) lastPartName = lastPartName.Substring(1);
            foreach (var header in csvHeaders)
            {
                // Check multiple patterns: exact match, with array notation, or last part match
                if (string.Equals(header, baseName, StringComparison.OrdinalIgnoreCase)
                 || header.StartsWith(baseName, StringComparison.OrdinalIgnoreCase)
                 || !string.IsNullOrEmpty(lastPartName)
                 && (string.Equals(header, lastPartName, StringComparison.OrdinalIgnoreCase) || header.StartsWith(lastPartName, StringComparison.OrdinalIgnoreCase)))
                {
                    fieldMappings[fieldPath] = header;
                    return true;
                }
            }
            return true; // We handled this field (even if no match was found)
        }
    }
}
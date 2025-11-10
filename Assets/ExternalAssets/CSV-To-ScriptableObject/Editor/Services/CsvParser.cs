using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Services
{
    /// <summary> Result of CSV file parsing </summary>
    public class CsvParseResult
    {
        /// <summary> CSV file headers </summary>
        public List<string> Headers { get; set; } = new();
        /// <summary> CSV file data </summary>
        public List<List<string>> Data { get; set; } = new();
    }
    /// <summary> Service for processing CSV files </summary>
    public class CsvParser
    {
        public CsvParseResult ParseCsvFile(string filePath, char delimiter, char quoteCharacter, bool hasHeaderRow)
        {
            var result = new CsvParseResult();

            try
            {
                // Read all lines from the file
                string[] lines = File.ReadAllLines(filePath);

                if (lines.Length == 0)
                {
                    throw new("CSV file is empty");
                }

                // Process headers
                if (hasHeaderRow)
                {
                    result.Headers = this.ParseCsvLine(lines[0], delimiter, quoteCharacter);
                }
                else
                {
                    // If there's no header row, generate column names
                    var firstRow = this.ParseCsvLine(lines[0], delimiter, quoteCharacter);
                    for (int i = 0; i < firstRow.Count; i++)
                    {
                        result.Headers.Add($"Column{i + 1}");
                    }
                }

                // Process data rows
                int startIndex = hasHeaderRow ? 1 : 0;
                for (int i = startIndex; i < lines.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[i]))
                    {
                        result.Data.Add(this.ParseCsvLine(lines[i], delimiter, quoteCharacter));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing CSV: {ex.Message}");
                throw;
            }

            return result;
        }
        public List<string> ParseCsvLine(string line, char delimiter, char quoteCharacter)
        {
            List<string> fields = new();
            bool inQuotes = false;
            string currentField = "";

            // Process character by character
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == quoteCharacter)
                {
                    // Handle escape sequence (two consecutive quotes)
                    if (i + 1 < line.Length && line[i + 1] == quoteCharacter)
                    {
                        currentField += quoteCharacter;
                        i++; // Skip the next quote
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == delimiter && !inQuotes)
                {
                    // End of field, add it to the list
                    fields.Add(currentField);
                    currentField = "";
                }
                else
                {
                    currentField += c;
                }
            }

            // Add the final field
            fields.Add(currentField);

            return fields;
        }
    }
}
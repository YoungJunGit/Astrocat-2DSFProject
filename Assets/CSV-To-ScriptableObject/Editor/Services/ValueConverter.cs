using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Services
{
    /// <summary> Service for converting values between different types </summary>
    public class ValueConverter
    {
        private readonly Dictionary<Type, Func<string, string, object>> _typeConverters = new();
        public ValueConverter(string splitter) => this.RegisterConverters(splitter);
        public void RegisterConverter<T>(Func<string, string, object> converter)
        {
            this._typeConverters[typeof(T)] = converter;
        }
        public object ConvertValueToType(string value, Type targetType, string delimiter)
        {
            if (string.IsNullOrEmpty(value)) return this.GetDefaultValue(targetType);
            try
            {
                // Direct type converter
                if (this._typeConverters.TryGetValue(targetType, out var converter)) return converter(value, delimiter);
                // Enum types
                if (targetType.IsEnum) return ParseEnum(value, targetType);
                // Array types
                if (targetType.IsArray) return this.ParseArray(value, targetType.GetElementType(), delimiter);
                // Generic List types
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(List<>)) return this.ParseList(value, targetType.GetGenericArguments()[0], delimiter);
                throw new NotSupportedException($"Conversion for type {targetType.Name} is not implemented");
            }
            catch (Exception ex)
            {
                throw new FormatException($"Cannot convert value '{value}' to type {targetType.Name}", ex);
            }
        }
        public object GetDefaultValue(Type type)
        {
            // Predefined defaults for common types
            if (type == typeof(string)) return "";
            if (type == typeof(int)) return 0;
            if (type == typeof(float)) return 0f;
            if (type == typeof(double)) return 0d;
            if (type == typeof(bool)) return false;
            if (type == typeof(Vector2)) return Vector2.zero;
            if (type == typeof(Vector3)) return Vector3.zero;
            if (type == typeof(Color)) return Color.clear;

            // Default for value types or null for reference types
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
        private static Color ParseColor(string value, string splitter)
        {
            // Support for hex format (#RRGGBB or #RRGGBBAA)
            if (value.StartsWith("#"))
            {
                ColorUtility.TryParseHtmlString(value, out Color color);
                return color;
            }
            // Support for RGB/RGBA format (r;g;b) or (r;g;b;a)
            string[] parts = value.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) throw new FormatException("Color requires 'r;g;b' or 'r;g;b;a' or '#RRGGBB' format");

            float r = float.Parse(parts[0].Trim()) / 255f;
            float g = float.Parse(parts[1].Trim()) / 255f;
            float b = float.Parse(parts[2].Trim()) / 255f;
            float a = parts.Length >= 4 ? float.Parse(parts[3].Trim()) / 255f : 1f;

            return new(r, g, b, a);
        }
        private static object ParseEnum(string value, Type enumType)
        {
            // Try direct parsing
            string normalizedValue = value.Replace(" ", "").Replace("-", "");
            if (Enum.TryParse(enumType, normalizedValue, true, out object result)) return result;
            // Try parsing numeric value
            if (int.TryParse(value, out int intValue))
                if (Enum.IsDefined(enumType, intValue))
                    return Enum.ToObject(enumType, intValue);
            throw new FormatException($"Value '{value}' is not a valid value for enum type {enumType.Name}");
        }
        private static Vector2 ParseVector2(string value, string splitter)
        {
            string[] parts = value.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) throw new FormatException("Vector2 requires 'x;y' format");
            return new(float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                       float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture));
        }
        private static Vector3 ParseVector3(string value, string splitter)
        {
            string[] parts = value.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) throw new FormatException("Vector3 requires 'x;y;z' format");
            return new(float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture),
                       float.Parse(parts[1].Trim(), CultureInfo.InvariantCulture),
                       float.Parse(parts[2].Trim(), CultureInfo.InvariantCulture));
        }
        private void RegisterConverters(string splitter)
        {
            // Basic types
            this.RegisterConverter<string>((value, _) => value);
            this.RegisterConverter<int>((value, _) => ParseInt(value));
            this.RegisterConverter<float>((value, _) => ParseFloat(value));
            this.RegisterConverter<double>((value, _) => ParseDouble(value));
            this.RegisterConverter<bool>((value, _) => ParseBool(value));
            this.RegisterConverter<Enum>((value, _) => ParseEnum(value, typeof(Enum)));

            // Unity types
            this.RegisterConverter<Vector2>((value, _) => ParseVector2(value, splitter));
            this.RegisterConverter<Vector3>((value, _) => ParseVector3(value, splitter));
            this.RegisterConverter<Color>((value, _) => ParseColor(value, splitter));
        }
        private static object ParseBool(string value)
        {
            if (bool.TryParse(value, out bool result)) return result;
            return value.ToLowerInvariant() == "true" || value == "1" || value.ToLowerInvariant() == "yes";
        }
        private static double ParseDouble(string value)
        {
            string normalizedValue = value.Replace(" ", "").Replace(",", ".");
            if (double.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            throw new FormatException($"Cannot parse '{value}' as a double number");
        }
        private static float ParseFloat(string value)
        {
            string normalizedValue = value.Replace(" ", "").Replace(",", ".");
            if (float.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            throw new FormatException($"Cannot parse '{value}' as a float number");
        }
        private static int ParseInt(string value)
        {
            string normalizedValue = value.Replace(" ", "").Replace(",", "");
            if (int.TryParse(normalizedValue, NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
            {
                return result;
            }
            throw new FormatException($"Cannot parse '{value}' as an integer number");
        }
        private object ParseArray(string value, Type elementType, string splitter)
        {
            if (string.IsNullOrEmpty(value)) return Array.CreateInstance(elementType, 0);
            string[] items = value.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            Array array = Array.CreateInstance(elementType, items.Length);
            for (int i = 0; i < items.Length; i++)
            {
                array.SetValue(this.ConvertValueToType(items[i].Trim(), elementType, splitter), i);
            }
            return array;
        }
        private object ParseList(string value, Type elementType, string splitter)
        {
            if (string.IsNullOrEmpty(value)) return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            string[] items = value.Split(new[] {splitter}, StringSplitOptions.RemoveEmptyEntries);
            var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            foreach (string item in items)
            {
                list.Add(this.ConvertValueToType(item.Trim(), elementType, splitter));
            }
            return list;
        }
    }
}
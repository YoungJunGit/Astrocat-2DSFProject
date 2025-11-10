using System.Collections.Generic;
using UnityEngine;

namespace CSV_To_ScriptableObject.Editor.Example
{
    /// <summary> All classes must have public constructor parameterless </summary>
    public class AllTypesExampleScriptableObject: ScriptableObject
    {
        public AllTypesExampleScriptableObject() { }
        // Basic types
        public string stringValue;
        public int intValue;
        public float floatValue;
        public double doubleValue;
        public bool boolValue;
        // Unity types
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Color colorValue;
        // Enum example
        public ExampleEnum enumValue;
        // Arrays
        public int[] intArray;
        public string[] stringArray;
        // Lists
        public List<float> floatList;
        public List<string> stringList;
        // Nested serializable class
        public NestedExample nestedValue;
        [SerializeField]
        private NestedExample privateNestedValueSerialized;
    }
    public enum ExampleEnum
    {
        First,
        Second,
        Third,
    }
    [System.Serializable]
    public class NestedExample
    {
        public NestedExample() { }
        public string name;
        public int value;
        [SerializeField]
        private string privateStringFieldSerialized;
        [SerializeField]
        private int privateIntFieldSerialized;
    }
}
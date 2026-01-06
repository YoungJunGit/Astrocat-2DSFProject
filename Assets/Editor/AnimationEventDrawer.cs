using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationEventCombat))]
public class AnimationEventDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty stateTypeProperty = property.FindPropertyRelative("eventType");
        SerializedProperty stateEventProperty = property.FindPropertyRelative("OnAnimationEvent");

        Rect stateTypeRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        Rect stateEventRect = new(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUI.GetPropertyHeight(stateEventProperty));

        EditorGUI.PropertyField(stateTypeRect, stateTypeProperty);
        EditorGUI.PropertyField(stateEventRect, stateEventProperty, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty stateEventProperty = property.FindPropertyRelative("OnAnimationEvent");
        return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(stateEventProperty) + 4;
    }
}
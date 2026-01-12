using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.InkSupport
{

    [CustomEditor(typeof(BarkInkOnIdle), true)]
    public class BarkInkOnIdleEditor : BarkOnIdleEditor
    {
        private List<InkEntrypoint> entrypoints;
        private string[] entrypointStrings;
        private string[] entrypointFullPaths;

        public void OnEnable()
        {
            List<string> fullPaths;
            entrypoints = InkEditorUtility.GetAllEntrypoints(out fullPaths);
            entrypointStrings = InkEditorUtility.EntrypointsToStrings(entrypoints);
            entrypointFullPaths = fullPaths.ToArray();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            DrawInkSpecific();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawInkSpecific()
        {
            EditorGUILayout.LabelField("Ink-Specific", EditorStyles.boldLabel);
            var barkConversationProperty = serializedObject.FindProperty("conversation");
            var barkKnotProperty = serializedObject.FindProperty("barkKnot");
            EditorGUILayout.PropertyField(barkKnotProperty, true);
            var index = InkEditorUtility.GetEntrypointIndex(barkConversationProperty.stringValue, barkKnotProperty.stringValue, entrypoints);
            EditorGUI.BeginChangeCheck();
#if INK_FULLPATHS
            index = EditorGUILayout.Popup("Entrypoint Picker", index, entrypointFullPaths);
#else
            index = EditorGUILayout.Popup("Entrypoint Picker", index, entrypointStrings);
#endif
            if (EditorGUI.EndChangeCheck())
            {
                barkKnotProperty.stringValue = SetEntrypoint(barkConversationProperty, barkKnotProperty, index);
            }
        }

        protected string SetEntrypoint(SerializedProperty conversationProperty, SerializedProperty startAtKnotProperty, int index)
        {
            if (!(0 <= index && index < entrypoints.Count)) return string.Empty;
            var entrypoint = entrypoints[index];
            conversationProperty.stringValue = entrypoint.story;
            if (string.IsNullOrEmpty(entrypoint.knot))
            {
                startAtKnotProperty.stringValue = string.Empty;
            }
            else
            {
                if (string.IsNullOrEmpty(entrypoint.stitch))
                {
                    startAtKnotProperty.stringValue = entrypoint.knot;
                }
                else
                {
                    startAtKnotProperty.stringValue = (entrypoints[index].knot + "." + entrypoints[index].stitch);
                }
            }
            return startAtKnotProperty.stringValue;
        }

    }
}

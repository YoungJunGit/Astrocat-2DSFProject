using UnityEngine;
using UnityEditor;

namespace NewFiles.Editor
{
    /// <summary>
    /// Defines the settings window for the NewFiles asset.
    /// This window allows users to configure all features, including Zebra Striping and Hierarchy Icons.
    /// </summary>
    public class NewFilesSettingsWindow : EditorWindow
    {
        // Private fields for managing settings and UI state.
        private SerializedObject serializedSettings;
        private SerializedProperty predefinedIconsProperty;
        private Vector2 scrollPosition;
        private bool showZebraSettings = true;

        /// <summary>
        /// Creates and shows the NewFiles settings window.
        /// This method is accessible from the Unity menu via "Tools/NewFiles/Settings Window".
        /// </summary>
        [MenuItem("Tools/NewFiles/Settings Window")]
        public static void ShowWindow()
        {
            NewFilesSettingsWindow window = GetWindow<NewFilesSettingsWindow>("NewFiles Settings");
            window.minSize = new Vector2(450, 550); // Set a minimum size for better layout.
            window.Show();
        }
        
        /// <summary>
        /// Called when the window is enabled or created.
        /// It loads the settings asset and prepares it for serialization.
        /// </summary>
        private void OnEnable()
        {
            var settings = NewFilesSettings.Instance;
            if (settings != null)
            {
                serializedSettings = new SerializedObject(settings);
                predefinedIconsProperty = serializedSettings.FindProperty("predefinedIconPaths");
            }
        }

        /// <summary>
        /// The main GUI loop called by Unity to draw the window's contents.
        /// </summary>
        private void OnGUI()
        {
            if (serializedSettings == null)
            {
                EditorGUILayout.HelpBox("Error: Could not load NewFiles settings.", MessageType.Error);
                return;
            }

            // Update the serialized object to reflect any changes.
            serializedSettings.Update();

            // Begin a scroll view to handle content that might exceed the window's visible area.
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Draw each section of the settings.
            DrawGeneralSettings();
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            
            DrawHierarchySettings();
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            DrawZebraStripingSettings();
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            DrawQuickAccessSettings();
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();

            DrawPredefinedIconsSettings();

            EditorGUILayout.EndScrollView();

            // Apply any modified properties and save the settings if changes were made.
            if (serializedSettings.ApplyModifiedProperties() || GUI.changed)
            {
                NewFilesSettings.SaveSettings();
            }
        }
        
        /// <summary>
        /// Draws the general settings section.
        /// </summary>
        private void DrawGeneralSettings()
        {
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("isEnabled"), new GUIContent("Enable NewFiles"));
        }
        
        /// <summary>
        /// Draws the settings related to the Hierarchy window enhancements.
        /// </summary>
        private void DrawHierarchySettings()
        {
            EditorGUILayout.LabelField("Hierarchy Settings", EditorStyles.boldLabel);
            var hierarchyIconsProp = serializedSettings.FindProperty("enableHierarchyIcons");
            EditorGUILayout.PropertyField(hierarchyIconsProp, new GUIContent("Enable Hierarchy Icons", "Allows assigning custom icons to GameObjects in the Hierarchy window using Alt+Click."));

            // Only show the opacity slider if the main feature is enabled.
            if (hierarchyIconsProp.boolValue)
            {
                EditorGUI.indentLevel++;
                var hierarchyOpacityProp = serializedSettings.FindProperty("hierarchyColorOpacity");
                EditorGUILayout.PropertyField(hierarchyOpacityProp, new GUIContent("Background Opacity", "Controls the transparency of the custom background color for GameObjects."));
                EditorGUI.indentLevel--;
            }
        }

        /// <summary>
        /// Draws the settings for the Zebra Striping feature in the Project window.
        /// </summary>
        private void DrawZebraStripingSettings()
        {
            // A foldout header with a toggle on the same line.
            EditorGUILayout.BeginHorizontal();
            showZebraSettings = EditorGUILayout.Foldout(showZebraSettings, "Zebra Striping Settings", true, EditorStyles.foldoutHeader);
            
            var enableZebraProp = serializedSettings.FindProperty("enableZebraStriping");
            EditorGUI.BeginChangeCheck();
            bool zebraEnabled = EditorGUILayout.Toggle(enableZebraProp.boolValue, GUILayout.Width(20));
            if (EditorGUI.EndChangeCheck())
            {
                enableZebraProp.boolValue = zebraEnabled;
            }
            EditorGUILayout.EndHorizontal();
            
            // If the section is folded, do not draw the rest of the controls.
            if (!showZebraSettings) return;
            
            EditorGUI.indentLevel++;
            
            // Main toggle for enabling/disabling the feature.
            EditorGUILayout.PropertyField(enableZebraProp, new GUIContent("Enable Zebra Striping", "Toggles alternating row colors for better readability in the Project window."));
            
            if (enableZebraProp.boolValue)
            {
                EditorGUILayout.Space(5);
                
                // Find properties for colors, opacity, and folder-only mode.
                var lightColorProp = serializedSettings.FindProperty("zebraLightColor");
                var darkColorProp = serializedSettings.FindProperty("zebraDarkColor");
                var opacityProp = serializedSettings.FindProperty("zebraStripingOpacity");
                var onlyFoldersProp = serializedSettings.FindProperty("zebraOnlyFolders");
                
                EditorGUILayout.LabelField("Colors", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(lightColorProp, new GUIContent("Light Stripe Color", "The color for even-numbered rows."));
                EditorGUILayout.PropertyField(darkColorProp, new GUIContent("Dark Stripe Color", "The color for odd-numbered rows."));
                
                EditorGUILayout.Space(3);
                EditorGUILayout.PropertyField(opacityProp, new GUIContent("Global Opacity", "A master opacity multiplier for both stripe colors."));
                
                EditorGUILayout.Space(5);
                EditorGUILayout.PropertyField(onlyFoldersProp, new GUIContent("Only Folders", "If enabled, applies striping only to folders."));
                
                EditorGUILayout.Space(10);
                
                // Draw a live preview of the current settings.
                DrawZebraPreview(lightColorProp.colorValue, darkColorProp.colorValue, opacityProp.floatValue);
                
                EditorGUILayout.Space(5);
                
                // Buttons for quick presets.
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Reset to Default", GUILayout.Width(120)))
                {
                    lightColorProp.colorValue = new Color(1f, 1f, 1f, 0.05f);
                    darkColorProp.colorValue = new Color(0f, 0f, 0f, 0.05f);
                    opacityProp.floatValue = 0.05f;
                    onlyFoldersProp.boolValue = true;
                }
                
                if (GUILayout.Button("High Contrast", GUILayout.Width(120)))
                {
                    lightColorProp.colorValue = new Color(1f, 1f, 1f, 0.15f);
                    darkColorProp.colorValue = new Color(0f, 0f, 0f, 0.15f);
                    opacityProp.floatValue = 0.355f;
                }
                
                if (GUILayout.Button("Subtle", GUILayout.Width(120)))
                {
                    lightColorProp.colorValue = new Color(1f, 1f, 1f, 0.02f);
                    darkColorProp.colorValue = new Color(0f, 0f, 0f, 0.02f);
                    opacityProp.floatValue = 1f;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUI.indentLevel--;
        }
        
        /// <summary>
        /// Draws a visual preview of the zebra striping based on the current settings.
        /// </summary>
        /// <param name="lightColor">The base color for light stripes.</param>
        /// <param name="darkColor">The base color for dark stripes.</param>
        /// <param name="globalOpacity">The global opacity multiplier.</param>
        private void DrawZebraPreview(Color lightColor, Color darkColor, float globalOpacity)
        {
            EditorGUILayout.LabelField("Preview", EditorStyles.miniBoldLabel);
            
            // Apply the global opacity to the base colors for the preview.
            Color previewLight = lightColor;
            Color previewDark = darkColor;
            previewLight.a *= globalOpacity;
            previewDark.a *= globalOpacity;
            
            // Draw several lines to simulate the effect in the Project window.
            for (int i = 0; i < 6; i++)
            {
                Rect lineRect = GUILayoutUtility.GetRect(0, 18, GUILayout.ExpandWidth(true));
                lineRect.width -= 20; // Indent slightly for better visuals.
                
                Color lineColor = (i % 2 == 0) ? previewLight : previewDark;
                
                // Only draw the rectangle if it's visible to avoid performance issues with zero alpha.
                if (lineColor.a > 0.001f)
                {
                    EditorGUI.DrawRect(lineRect, lineColor);
                }
                
                // Draw a label over the colored rectangle.
                GUI.Label(lineRect, $"  📁 Example Folder {i + 1}", EditorStyles.label);
            }
        }
        
        /// <summary>
        /// Draws settings for the Quick Access Bar.
        /// </summary>
        private void DrawQuickAccessSettings()
        {
            EditorGUILayout.LabelField("Quick Access Bar", EditorStyles.boldLabel);
            var quickAccessItemWidthProp = serializedSettings.FindProperty("quickAccessItemWidth");
            EditorGUILayout.PropertyField(quickAccessItemWidthProp, new GUIContent("Item Width", "The width of each button in the Quick Access Bar, in pixels."));
            
            var itemOpacityProp = serializedSettings.FindProperty("quickAccessItemBackgroundOpacity");
            EditorGUILayout.PropertyField(itemOpacityProp, new GUIContent("Item Background Opacity", "The background opacity for items that have a custom color assigned."));
        }
        
        /// <summary>
        /// Draws the UI for managing the list of predefined icons.
        /// </summary>
        private void DrawPredefinedIconsSettings()
        {
            EditorGUILayout.LabelField("Predefined Icons (Max 20)", EditorStyles.boldLabel);
            
            // Allow the user to set the number of icons.
            int newSize = EditorGUILayout.IntField("Size", predefinedIconsProperty.arraySize);
            if (newSize != predefinedIconsProperty.arraySize)
            {
                // Clamp the size between 0 and a maximum of 20.
                predefinedIconsProperty.arraySize = Mathf.Clamp(newSize, 0, 20);
            }

            EditorGUI.indentLevel++;
            // Loop through the array and draw an object field for each icon.
            for (int i = 0; i < predefinedIconsProperty.arraySize; i++)
            {
                SerializedProperty element = predefinedIconsProperty.GetArrayElementAtIndex(i);
                string currentPath = element.stringValue;
                Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(currentPath);
                
                EditorGUILayout.BeginHorizontal();
                
                // Draw an object field to allow dragging and dropping a Texture2D.
                Texture2D newIcon = (Texture2D)EditorGUILayout.ObjectField($"Icon {i+1}", icon, typeof(Texture2D), false);
                
                // If the icon has changed, update the path in the settings.
                if (newIcon != icon)
                {
                    string newPath = AssetDatabase.GetAssetPath(newIcon);
                    element.stringValue = newPath;
                }
                
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }
    }
}
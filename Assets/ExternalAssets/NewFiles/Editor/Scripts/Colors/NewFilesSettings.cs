using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace NewFiles.Editor
{
    /// <summary>
    /// Manages all settings for the NewFiles asset.
    /// This class uses a ScriptableObject to store user preferences and system configurations,
    /// which are saved to EditorPrefs as a JSON string.
    /// </summary>
    [System.Serializable]
    public class NewFilesSettings : ScriptableObject
    {
        // Singleton instance for easy access to settings throughout the editor.
        private static NewFilesSettings instance;
        
        [Header("General Settings")]
        [Tooltip("Globally enables or disables all features of the asset.")]
        [SerializeField] public bool isEnabled = true;
        [Tooltip("Enables detailed logging to the console for debugging purposes.")]
        [SerializeField] public bool enableDebugLog = false;
        [Tooltip("Shows descriptive tooltips when hovering over UI elements.")]
        [SerializeField] public bool showTooltips = true;
        
        [Header("Input Settings")]
        [Tooltip("Use Alt + Left Click to open the customization menu.")]
        [SerializeField] public bool useAltClick = true;
        [Tooltip("Use Ctrl + Left Click to open the customization menu.")]
        [SerializeField] public bool useCtrlClick = false;
        [Tooltip("Use Shift + Left Click to open the customization menu.")]
        [SerializeField] public bool useShiftClick = false;
        
        [Header("Visual Settings")]
        [Tooltip("The opacity of the color overlay applied to folder backgrounds.")]
        [SerializeField, Range(0f, 1f)] public float colorOpacity = 0.3f;
        [Tooltip("Displays a small color preview next to the folder icon.")]
        [SerializeField] public bool showColorPreview = true;
        [Tooltip("Enables a smooth transition effect when folder colors change.")]
        [SerializeField] public bool animateColorChanges = true;
        
        [Header("Performance Settings")]
        [Tooltip("Enables caching of folder data to improve performance.")]
        [SerializeField] public bool enableCache = true;
        [Tooltip("The maximum number of folder states to keep in the cache.")]
        [SerializeField] public int maxCachedFolders = 500;
        [Tooltip("Loads folder customizations only when they become visible in the project window.")]
        [SerializeField] public bool lazyLoading = true;
        
        [Header("Backup Settings")]
        [Tooltip("Automatically creates backups of the customization data.")]
        [SerializeField] public bool autoBackup = true;
        [Tooltip("The frequency in days at which to create a new backup.")]
        [SerializeField] public int backupFrequencyDays = 7;
        [Tooltip("The maximum number of backup files to retain.")]
        [SerializeField] public int maxBackups = 5;
        
        [Header("Quick Access Bar Settings")]
        [Tooltip("Enables the Quick Access Bar at the top of the Project window.")]
        [SerializeField] public bool enableQuickAccessBar = true;
        [Tooltip("The height of the Quick Access Bar in pixels.")]
        [SerializeField, Range(18f, 40f)] public float quickAccessBarHeight = 18f;
        [Tooltip("The width of each individual item in the Quick Access Bar.")]
        [SerializeField, Range(40f, 300f)] public float quickAccessItemWidth = 250f;
        [Tooltip("The maximum number of items to display in the Quick Access Bar.")]
        [SerializeField] public int maxQuickAccessItems = 20;
        [Tooltip("Show tooltips for items in the Quick Access Bar.")]
        [SerializeField] public bool showQuickAccessTooltips = true;
        [Tooltip("Automatically sort items by most recently used. If disabled, items maintain a fixed order.")]
        [SerializeField] public bool autoSortQuickAccess = true;

        [Header("Quick Access Bar Visual")]
        [Tooltip("The background opacity of the Quick Access Bar itself.")]
        [SerializeField, Range(0.1f, 1.0f)] public float quickAccessBackgroundOpacity = 0.3f;
        
        [Header("Quick Access Item Visual")]
        [Tooltip("The background opacity for individual items within the Quick Access Bar.")]
        [SerializeField, Range(0.0f, 1.0f)] public float quickAccessItemBackgroundOpacity = 0.3f;

        [Header("Icon Settings")]
        [Tooltip("Stores the asset paths for the 20 predefined icons available for selection.")]
        [SerializeField] public List<string> predefinedIconPaths = new List<string>();

        [Header("Zebra Striping Settings")]
        [Tooltip("Enables alternating row colors in the Project window for better readability.")]
        [SerializeField] public bool enableZebraStriping = false;
        [Tooltip("The color for the lighter, even-numbered rows.")]
        [SerializeField] public Color zebraLightColor = new Color(1f, 1f, 1f, 0.05f);
        [Tooltip("The color for the darker, odd-numbered rows.")]
        [SerializeField] public Color zebraDarkColor = new Color(0f, 0f, 0f, 0.05f);
        [Tooltip("The overall opacity of the zebra stripe effect.")]
        [SerializeField, Range(0f, 1f)] public float zebraStripingOpacity = 0.05f;
        [Tooltip("If enabled, zebra striping will only be applied to folders.")]
        [SerializeField] public bool zebraOnlyFolders = true;
        
        [Header("Hierarchy Settings")]
        [Tooltip("Enables applying custom icons and background colors to GameObjects in the Hierarchy window.")]
        [SerializeField] public bool enableHierarchyIcons = true;
        [Tooltip("The opacity of the background color applied to GameObjects in the Hierarchy.")]
        [SerializeField, Range(0f, 0.25f)] public float hierarchyColorOpacity = 0.2f;


        // --- SOLUCIÓ APLICADA AQUÍ ---
        private static Color[] _predefinedColors;

        /// <summary>
        /// A curated and ordered palette of predefined colors for UI selection.
        /// It's now a property to ensure colors are parsed at a safe time.
        /// </summary>
        public static Color[] PredefinedColors
        {
            get
            {
                if (_predefinedColors == null)
                {
                    _predefinedColors = new Color[]
                    {
                        // Row 1: Reds & Pinks
                        HexToColor("#d32f2f"), // Dark Red
                        HexToColor("#F44336"), // Red
                        HexToColor("#ef5350"), // Light Red
                        HexToColor("#E91E63"), // Hot Pink
                        HexToColor("#f48fb1"), // Soft Pink
                        HexToColor("#fab387"), // Peach
                        
                        // Row 2: Oranges & Yellows
                        HexToColor("#FF9800"), // Orange
                        HexToColor("#FFCC80"), // Pastel Orange
                        HexToColor("#FFEB3B"), // Yellow
                        HexToColor("#f9e2af"), // Pastel Yellow
                        HexToColor("#CDDC39"), // Lime
                        HexToColor("#8BC34A"), // Light Green
                        
                        // Row 3: Greens
                        HexToColor("#a6e3a1"), // Pastel Green
                        HexToColor("#4CAF50"), // Green
                        HexToColor("#2e7d32"), // Forest Green
                        HexToColor("#94e2d5"), // Pastel Teal
                        HexToColor("#009688"), // Teal
                        HexToColor("#00BCD4"), // Cyan
                        
                        // Row 4: Blues
                        HexToColor("#89dceb"), // Sky Blue
                        HexToColor("#90caf9"), // Soft Blue
                        HexToColor("#2196F3"), // Blue
                        HexToColor("#3F51B5"), // Indigo
                        HexToColor("#1a237e"), // Dark Blue
                        HexToColor("#b4befe"), // Lavender
                        
                        // Row 5: Purples & Neutrals
                        HexToColor("#9C27B0"), // Purple
                        HexToColor("#673AB7"), // Deep Purple
                        HexToColor("#795548"), // Brown
                        HexToColor("#9e9e9e"), // Grey
                        HexToColor("#6c7086"), // Muted Grey
                        HexToColor("#424242")  // Dark Grey
                    };
                }
                return _predefinedColors;
            }
        }
        // --- FINAL DE LA SOLUCIÓ ---

        /// <summary>
        /// Gets the singleton instance of the settings, creating it if it doesn't exist.
        /// </summary>
        public static NewFilesSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    LoadOrCreateSettings();
                }
                return instance;
            }
        }
        
        /// <summary>
        /// A utility method to convert a HEX color string to a Unity Color object.
        /// </summary>
        /// <param name="hex">The HEX color string (e.g., "#FF5733").</param>
        /// <returns>A Color object, or Color.white if parsing fails.</returns>
        private static Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }

        /// <summary>
        /// Checks if the settings key exists in EditorPrefs.
        /// </summary>
        /// <returns>True if settings have been saved before, false otherwise.</returns>
        public static bool Exists()
        {
            return EditorPrefs.HasKey("NewFiles.Settings");
        }

        /// <summary>
        /// Creates a new settings instance with default values and saves it.
        /// </summary>
        public static void CreateDefault()
        {
            try
            {
                instance = CreateInstance<NewFilesSettings>();
                if (instance == null)
                {
                    Debug.LogError("[NewFiles] Failed to create settings instance.");
                    return;
                }
                
                instance.ResetToDefaults();
                SaveSettings();
                
                Debug.Log("[NewFiles] Default settings created successfully.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewFiles] Error creating default settings: {e.Message}");
            }
        }

        /// <summary>
        /// Loads settings from EditorPrefs. If no settings are found, creates a new default instance.
        /// </summary>
        private static void LoadOrCreateSettings()
        {
            try
            {
                string settingsJson = EditorPrefs.GetString("NewFiles.Settings", "");
                
                if (!string.IsNullOrEmpty(settingsJson))
                {
                    instance = CreateInstance<NewFilesSettings>();
                    if (instance != null)
                    {
                        JsonUtility.FromJsonOverwrite(settingsJson, instance);
                        instance.ValidateSettings();
                        
                        if (instance.enableDebugLog)
                        {
                            Debug.Log("[NewFiles] Settings loaded successfully from EditorPrefs.");
                        }
                    }
                    else
                    {
                        throw new System.Exception("Failed to create settings instance during load.");
                    }
                }
                else
                {
                    Debug.Log("[NewFiles] No existing settings found, creating defaults.");
                    CreateDefault();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[NewFiles] Error loading settings, falling back to defaults: {e.Message}");
                CreateDefault();
            }
        }

        /// <summary>
        /// Saves the current settings instance to EditorPrefs as a JSON string.
        /// Also triggers a repaint of relevant editor windows.
        /// </summary>
        public static void SaveSettings()
        {
            try
            {
                if (instance != null)
                {
                    instance.ValidateSettings();
                    string settingsJson = JsonUtility.ToJson(instance, true);
                    EditorPrefs.SetString("NewFiles.Settings", settingsJson);
                    
                    if (instance.enableDebugLog)
                    {
                        Debug.Log("[NewFiles] Settings saved successfully.");
                    }
                    
                    // Force editor windows to repaint to reflect changes.
                    EditorApplication.RepaintProjectWindow();
                    EditorApplication.RepaintHierarchyWindow(); 
                    OnSettingsChanged?.Invoke();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewFiles] Error saving settings: {e.Message}");
            }
        }

        /// <summary>
        /// Resets all settings to their original default values.
        /// </summary>
        public void ResetToDefaults()
        {
            isEnabled = true;
            enableDebugLog = false;
            showTooltips = true;
            useAltClick = true;
            useCtrlClick = false;
            useShiftClick = false;
            colorOpacity = 0.3f;
            showColorPreview = true;
            animateColorChanges = true;
            enableCache = true;
            maxCachedFolders = 500;
            lazyLoading = true;
            autoBackup = true;
            backupFrequencyDays = 7;
            maxBackups = 5;
            enableQuickAccessBar = true;
            quickAccessBarHeight = 18f;
            quickAccessItemWidth = 150f;
            maxQuickAccessItems = 20;
            showQuickAccessTooltips = true;
            autoSortQuickAccess = true;
            quickAccessBackgroundOpacity = 0.3f;
            quickAccessItemBackgroundOpacity = 0.3f; 
            predefinedIconPaths.Clear();

            enableZebraStriping = false;
            zebraLightColor = new Color(1f, 1f, 1f, 0.05f);
            zebraDarkColor = new Color(0f, 0f, 0f, 0.05f);
            zebraStripingOpacity = 0.05f;
            zebraOnlyFolders = true;
            
            enableHierarchyIcons = true;
            hierarchyColorOpacity = 0.3f;
        }

        /// <summary>
        /// Validates settings to ensure they are within acceptable ranges.
        /// Corrects any invalid values automatically.
        /// </summary>
        /// <returns>True if all settings were already valid, false if corrections were made.</returns>
        public bool ValidateSettings()
        {
            bool isValid = true;
            
            try
            {
                if (colorOpacity < 0f || colorOpacity > 1f) { colorOpacity = Mathf.Clamp01(colorOpacity); isValid = false; }
                if (hierarchyColorOpacity < 0f || hierarchyColorOpacity > 1f) { hierarchyColorOpacity = Mathf.Clamp01(hierarchyColorOpacity); isValid = false; }
                if (maxCachedFolders < 10) { maxCachedFolders = 10; isValid = false; }
                if (backupFrequencyDays < 1) { backupFrequencyDays = 1; isValid = false; }
                if (maxBackups < 1) { maxBackups = 1; isValid = false; }
                if (quickAccessBarHeight < 18f) { quickAccessBarHeight = 18f; isValid = false; }
                if (quickAccessItemWidth < 40f) { quickAccessItemWidth = 40f; isValid = false; }
                if (maxQuickAccessItems < 5) { maxQuickAccessItems = 5; isValid = false; }
                if (maxQuickAccessItems > 100) { maxQuickAccessItems = 100; isValid = false; }
                
                // Ensure at least one input modifier is selected.
                if (!useAltClick && !useCtrlClick && !useShiftClick) { useAltClick = true; isValid = false; }
                
                if (zebraStripingOpacity < 0f || zebraStripingOpacity > 1f) 
                { 
                    zebraStripingOpacity = Mathf.Clamp01(zebraStripingOpacity); 
                    isValid = false; 
                }

                if (!isValid && enableDebugLog) 
                { 
                    Debug.Log("[NewFiles] Settings were validated and corrected.");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewFiles] Error validating settings: {e.Message}. Resetting to defaults.");
                ResetToDefaults();
                isValid = false;
            }
            
            return isValid;
        }

        /// <summary>
        /// Gets the currently configured EventModifier for triggering the asset's main functionality.
        /// </summary>
        /// <returns>The EventModifiers enum corresponding to the user's choice.</returns>
        public EventModifiers GetInputModifier()
        {
            if (useAltClick) return EventModifiers.Alt;
            if (useCtrlClick) return EventModifiers.Control;
            if (useShiftClick) return EventModifiers.Shift;
            return EventModifiers.Alt; // Fallback default.
        }

        /// <summary>
        /// Gets a user-friendly string representation of the currently selected shortcut.
        /// </summary>
        /// <returns>A string like "Alt+Click".</returns>
        public string GetShortcutText()
        {
            if (useAltClick) return "Alt+Click";
            if (useCtrlClick) return "Ctrl+Click";
            if (useShiftClick) return "Shift+Click";
            return "Alt+Click"; // Fallback default.
        }
        
        /// <summary>
        /// Exports the current settings to a JSON string.
        /// </summary>
        /// <returns>A formatted JSON string of the settings.</returns>
        public string ExportToJson() => JsonUtility.ToJson(this, true);

        /// <summary>
        /// Imports settings from a JSON string, overwriting current settings.
        /// </summary>
        /// <param name="json">The JSON string to import.</param>
        /// <returns>True if import was successful, false otherwise.</returns>
        public bool ImportFromJson(string json)
        {
            try {
                JsonUtility.FromJsonOverwrite(json, this);
                ValidateSettings();
                SaveSettings();
                return true;
            }
            catch (System.Exception e) {
                Debug.LogError($"[NewFiles] Error importing settings from JSON: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gathers and formats a string containing various debug information and statistics.
        /// </summary>
        /// <returns>A comprehensive debug information string.</returns>
        public string GetDebugInfo()
        {
            var info = new StringBuilder();
            info.AppendLine("=== NewFiles Settings Debug Info ===");
            info.AppendLine($"Instance Status: {(instance != null ? "Valid" : "NULL")}");
            info.AppendLine($"Enabled: {isEnabled}");
            info.AppendLine($"Hierarchy Icons Enabled: {enableHierarchyIcons}");
            info.AppendLine($"  - Hierarchy Opacity: {hierarchyColorOpacity:F2}");
            info.AppendLine($"Input Method: {GetShortcutText()}");
            info.AppendLine($"Folder Color Opacity: {colorOpacity:F2}");
            info.AppendLine($"Cache Enabled: {enableCache}");
            info.AppendLine($"Max Cached Folders: {maxCachedFolders}");
            info.AppendLine($"Auto Backup: {autoBackup}");
            info.AppendLine($"Quick Access Bar: {enableQuickAccessBar}");
            info.AppendLine($"  - Bar Height: {quickAccessBarHeight}");
            info.AppendLine($"  - Item Width: {quickAccessItemWidth}");
            info.AppendLine($"  - Max Items: {maxQuickAccessItems}");
            info.AppendLine($"  - Auto Sort: {autoSortQuickAccess}");
            info.AppendLine($"Unity Version: {Application.unityVersion}");
            info.AppendLine($"EditorPrefs Key Exists: {EditorPrefs.HasKey("NewFiles.Settings")}");
            
            // Try to get stats from other parts of the asset, but handle exceptions
            // in case those systems aren't available.
            try
            {
                var stats = QuickAccessData.GetStatistics();
                info.AppendLine($"Quick Access Items: {stats.totalItems}");
                var cacheStats = FolderCustomizer.GetCacheStatistics();
                info.AppendLine($"Cache Efficiency: {cacheStats.cacheEfficiency:F1}%");
                int folderCount = NewFilesCore.GetCustomizedFoldersCount();
                info.AppendLine($"Customized Folders: {folderCount}");
            }
            catch (System.Exception e)
            {
                info.AppendLine($"Could not retrieve runtime stats: {e.Message}");
            }
            
            info.AppendLine("====================================");
            return info.ToString();
        }
        
        /// <summary>
        /// An event that is invoked whenever the settings are changed and saved.
        /// Other editor windows can subscribe to this to react to changes.
        /// </summary>
        public static System.Action OnSettingsChanged;

        /// <summary>
        /// Unity's callback when a value is changed in the Inspector.
        /// Ensures settings are validated and saved immediately.
        /// </summary>
        private void OnValidate()
        {
            ValidateSettings();
            // It's recommended to call SaveSettings from the settings window
            // to avoid excessive saving while dragging sliders.
            // OnSettingsChanged is invoked from SaveSettings().
        }

        /// <summary>
        /// Creates a physical NewFilesSettings.asset file in the project.
        /// This can be useful for sharing settings across a team via version control.
        /// </summary>
        [MenuItem("Assets/Create/NewFiles/Settings Asset")]
        public static void CreateSettingsAsset()
        {
            try
            {
                string resourcesPath = "Assets/NewFiles/Editor/Resources";
                
                if (!Directory.Exists(resourcesPath))
                {
                    Directory.CreateDirectory(resourcesPath);
                    AssetDatabase.Refresh();
                }

                NewFilesSettings settings = CreateInstance<NewFilesSettings>();
                settings.ResetToDefaults();

                string assetPath = Path.Combine(resourcesPath, "NewFilesSettings.asset");
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();
                Selection.activeObject = settings;
                
                Debug.Log("[NewFiles] Settings asset created successfully at " + assetPath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewFiles] Error creating settings asset: {e.Message}");
                EditorUtility.DisplayDialog("Error", $"Could not create the settings asset: {e.Message}", "OK");
            }
        }
    }
    
    /// <summary>
    /// A custom editor for the NewFilesSettings ScriptableObject.
    /// It provides a more user-friendly interface in the Inspector, redirecting
    /// users to the main settings window for a better experience.
    /// </summary>
    [CustomEditor(typeof(NewFilesSettings))]
    public class NewFilesSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("For the best experience, please use 'Tools > NewFiles > Settings Window' to edit settings.", MessageType.Info);
            EditorGUILayout.Space();
            
            // Button to quickly open the dedicated settings window.
            if (GUILayout.Button("Open Settings Window", GUILayout.Height(30)))
            {
                EditorApplication.ExecuteMenuItem("Tools/NewFiles/Settings Window");
                return;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.Separator();
            EditorGUILayout.Space();
            
            // Draw the default inspector for direct editing if needed.
            DrawDefaultInspector();
            NewFilesSettings settings = (NewFilesSettings)target;
            
            // If any value was changed in the default inspector, validate and save.
            if (GUI.changed)
            {
                settings.ValidateSettings();
                NewFilesSettings.SaveSettings();
            }
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            // Button to reset all settings to their default values.
            if (GUILayout.Button("Reset to Defaults"))
            {
                if (EditorUtility.DisplayDialog("Reset Settings", "Are you sure you want to reset all settings to their default values?", "Yes", "Cancel"))
                {
                    settings.ResetToDefaults();
                    NewFilesSettings.SaveSettings();
                }
            }
            
            // Button to log debug information to the console.
            if (GUILayout.Button("Show Debug Info"))
            {
                Debug.Log(settings.GetDebugInfo());
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
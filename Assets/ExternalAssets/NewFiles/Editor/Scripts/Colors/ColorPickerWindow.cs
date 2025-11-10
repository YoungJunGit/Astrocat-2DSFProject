using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using NewFiles.Runtime;
using UnityEditor.SceneManagement;

namespace NewFiles.Editor
{
    /// <summary>
    /// Manages an editor window for applying custom colors and icons to folders in the Project window
    /// or GameObjects in the Hierarchy window.
    /// </summary>
    public class ColorPickerWindow : EditorWindow
    {
        // Enum to differentiate between customizing a folder or a GameObject.
        private enum TargetType { Folder, GameObject }
        private TargetType currentTargetType;

        // --- Folder Properties ---
        // Stores the asset paths of the folders being customized.
        private List<string> targetFolderPaths = new List<string>();
        
        // --- GameObject Properties ---
        // Stores the instance IDs of the GameObjects being customized.
        private List<int> targetInstanceIDs = new List<int>();

        // --- Common Properties ---
        // The original color of the target before any changes.
        private Color originalColor = Color.clear;
        // The currently selected color in the picker.
        private Color selectedColor = Color.white;
        // The original icon path of the target.
        private string originalIconPath = "";
        // The currently selected icon path.
        private string selectedIconPath = "";
        
        // Flags to track whether the user has explicitly changed the color or icon.
        private bool userModifiedColor = false;
        private bool userModifiedIcon = false;
        
        // --- History ---
        // Static lists to store recently used colors and icons across sessions.
        private static List<Color> colorHistory = new List<Color>();
        private static List<string> iconHistory = new List<string>();
        
        // --- Constants ---
        // Maximum number of items to store in the history lists.
        private const int MAX_HISTORY_SIZE = 12;
        private const int MAX_ICON_HISTORY_SIZE = 10;
        // Keys for saving history data to EditorPrefs.
        private const string COLOR_HISTORY_KEY = "NewFiles.ColorHistory";
        private const string ICON_HISTORY_KEY = "NewFiles.IconHistory";
        
        // --- UI State ---
        // Toggles the visibility of the icon browser panel.
        private bool showIconBrowser = false;
        // Scroll position for the icon browser.
        private Vector2 scrollPosition = Vector2.zero;
        // Search filter text for the icon browser.
        private string iconSearchFilter = "";

        #region Window Management
        
        /// <summary>
        /// Creates and shows the color picker window for one or more folders.
        /// </summary>
        /// <param name="folderPaths">A list of asset paths for the target folders.</param>
        /// <param name="screenPosition">The position to display the window at.</param>
        public static void ShowWindowForFolders(List<string> folderPaths, Vector2 screenPosition)
        {
            if (folderPaths == null || folderPaths.Count == 0) return;
            LoadDependencies();
            var window = CreateInstance<ColorPickerWindow>();
            window.InitializeForFolders(folderPaths);
            window.ShowAsDropDown(new Rect(screenPosition.x, screenPosition.y, 1, 1), 
                                new Vector2(550f, 470f));
        }
        
        /// <summary>
        /// Creates and shows the color picker window for one or more GameObjects.
        /// </summary>
        /// <param name="instanceIDs">A list of instance IDs for the target GameObjects.</param>
        /// <param name="screenPosition">The position to display the window at.</param>
        public static void ShowWindowForGameObjects(List<int> instanceIDs, Vector2 screenPosition)
        {
            if (instanceIDs == null || instanceIDs.Count == 0) return;
            LoadDependencies();
            var window = CreateInstance<ColorPickerWindow>();
            window.InitializeForGameObjects(instanceIDs);
            window.ShowAsDropDown(new Rect(screenPosition.x, screenPosition.y, 1, 1), 
                                new Vector2(550f, 470f));
        }

        /// <summary>
        /// Initializes the window's state for customizing folders.
        /// </summary>
        private void InitializeForFolders(List<string> folderPaths)
        {
            currentTargetType = TargetType.Folder;
            targetFolderPaths = folderPaths;
            
            string referencePath = folderPaths.First();
            originalColor = NewFilesCore.GetFolderColor(referencePath);
            
            // If the original color is transparent, default to white. This improves the user
            // experience with Unity's color picker, which handles full transparency poorly.
            selectedColor = (originalColor.a > 0.001f) ? originalColor : Color.white;
            
            originalIconPath = NewFilesCore.GetFolderIcon(referencePath);
            selectedIconPath = originalIconPath;
            
            userModifiedColor = false;
            userModifiedIcon = false;
            
            titleContent = new GUIContent("Folder Customization");
        }

        /// <summary>
        /// Initializes the window's state for customizing GameObjects.
        /// </summary>
        private void InitializeForGameObjects(List<int> instanceIDs)
        {
            currentTargetType = TargetType.GameObject;
            targetInstanceIDs = instanceIDs;
            
            var go = EditorUtility.InstanceIDToObject(instanceIDs.First()) as GameObject;
            if (go == null) { Close(); return; }

            var iconComponent = go.GetComponent<HierarchyIcon>();
            
            // Set initial icon path from the component, if it exists.
            originalIconPath = iconComponent != null ? iconComponent.iconPath : "";
            selectedIconPath = originalIconPath;
            
            // Set initial color from the component, if it has a custom color.
            if (iconComponent != null && iconComponent.hasCustomColor)
            {
                originalColor = iconComponent.backgroundColor;
                selectedColor = originalColor;
            }
            else
            {
                // Default to white for a better color picker experience, consistent with folder logic.
                originalColor = Color.clear;
                selectedColor = Color.white;
            }

            userModifiedColor = false;
            userModifiedIcon = false;

            titleContent = new GUIContent("GameObject Customization");
        }

        /// <summary>
        /// Loads persistent data like color and icon history from EditorPrefs.
        /// </summary>
        private static void LoadDependencies()
        {
            LoadColorHistory();
            LoadIconHistory();
        }

        #endregion

        #region GUI Drawing

        /// <summary>
        /// Main GUI loop, called by Unity to draw the window content.
        /// </summary>
        void OnGUI()
        {
            AdjustWindowSize();
            DrawHeader();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginHorizontal();
            
            // Left panel for color selection.
            EditorGUILayout.BeginVertical(GUILayout.Width(220));
            DrawColorSection();
            EditorGUILayout.EndVertical();
            GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true)); // Vertical separator.
            
            // Middle panel for icon selection.
            EditorGUILayout.BeginVertical();
            DrawIconSection();
            EditorGUILayout.EndVertical();

            // Right panel for the icon browser (only shown when active).
            if (showIconBrowser)
            {
                GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true)); // Vertical separator.
                EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
                DrawIconBrowser();
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace(); // Pushes action buttons to the bottom.
            DrawActionButtons();
            HandleKeyboardInput();
        }
        
        /// <summary>
        /// Adjusts the window width dynamically when the icon browser is opened or closed.
        /// </summary>
        private void AdjustWindowSize()
        {
            float targetWidth = showIconBrowser ? 800f : 550f;
            
            if (Mathf.Abs(position.width - targetWidth) > 1f)
            {
                Rect currentRect = position;
                currentRect.size = new Vector2(targetWidth, 470f);
                this.position = currentRect;
                this.minSize = this.maxSize = currentRect.size;
            }
        }

        /// <summary>
        /// Draws the header label indicating what is being customized.
        /// </summary>
        private void DrawHeader()
        {
            string name = "Invalid Target";
            if (currentTargetType == TargetType.Folder)
            {
                if (targetFolderPaths.Count > 1) name = $"{targetFolderPaths.Count} folders";
                else if (targetFolderPaths.Count == 1) name = Path.GetFileName(targetFolderPaths.First());
            }
            else // GameObject
            {
                if (targetInstanceIDs.Count > 1) name = $"{targetInstanceIDs.Count} GameObjects";
                else if (targetInstanceIDs.Count == 1 && EditorUtility.InstanceIDToObject(targetInstanceIDs.First()) is GameObject go) name = go.name;
            }
            
            var style = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 12 };
            GUILayout.Label($"Customizing: \"{name}\"", style);
        }

        /// <summary>
        /// Draws the entire color selection panel.
        /// </summary>
        private void DrawColorSection()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Colors", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("No Color"))
            {
                selectedColor = Color.clear;
                userModifiedColor = true;
            }
            EditorGUILayout.EndHorizontal();

            DrawColorGrid("Predefined Colors", NewFilesSettings.PredefinedColors, 6);
            EditorGUILayout.Space();
            
            // Custom color field.
            EditorGUI.BeginChangeCheck();
            selectedColor = EditorGUILayout.ColorField("Custom Color", selectedColor);
            if (EditorGUI.EndChangeCheck())
            {
                userModifiedColor = true;
            }
            
            EditorGUILayout.Space();
            if(colorHistory.Count > 0) DrawColorGrid("Recent Colors", colorHistory.ToArray(), 6);
        }

        /// <summary>
        /// Draws the entire icon selection panel.
        /// </summary>
        private void DrawIconSection()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Icons", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+ Browse")) showIconBrowser = !showIconBrowser;
            if (GUILayout.Button("No Icon"))
            {
                selectedIconPath = "";
                userModifiedIcon = true;
            }
            EditorGUILayout.EndHorizontal();
            
            DrawIconGrid("Predefined Icons", GetPredefinedIcons());
            if(iconHistory.Count > 0) DrawIconGrid("Recent Icons", iconHistory.ToArray());
        }

        /// <summary>
        /// Draws a grid of selectable color swatches.
        /// </summary>
        private void DrawColorGrid(string label, Color[] colors, int itemsPerRow)
        {
            GUILayout.Label(label, EditorStyles.miniBoldLabel);
            for (int i = 0; i < colors.Length; i += itemsPerRow)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < itemsPerRow && i + j < colors.Length; j++)
                {
                    DrawColorSwatch(colors[i + j]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws a grid of selectable icon buttons.
        /// </summary>
        private void DrawIconGrid(string label, string[] iconPaths)
        {
            GUILayout.Label(label, EditorStyles.miniBoldLabel);
            int iconsPerRow = 5;
            for (int i = 0; i < iconPaths.Length; i += iconsPerRow)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < iconsPerRow && i + j < iconPaths.Length; j++)
                {
                    DrawIconButton(iconPaths[i + j]);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// Draws a single clickable color swatch.
        /// </summary>
        private void DrawColorSwatch(Color color)
        {
            Rect r = GUILayoutUtility.GetRect(24, 24);
            if (GUI.Button(r, ""))
            {
                selectedColor = color;
                userModifiedColor = true;
            }
            EditorGUI.DrawRect(r, color);
            
            // Draw a border around the currently selected color.
            if (selectedColor.a > 0.001f && Vector4.Distance(selectedColor, color) < 0.01f)
            {
                EditorGUI.DrawRect(new Rect(r.x-2, r.y-2, r.width+4, r.height+4), Color.white);
                EditorGUI.DrawRect(r, color);
            }
        }
        
        /// <summary>
        /// Draws a single clickable icon button.
        /// </summary>
        private void DrawIconButton(string iconPath)
        {
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (tex == null) return;
            
            Rect r = GUILayoutUtility.GetRect(32, 32);
            if (GUI.Button(r, ""))
            {
                selectedIconPath = iconPath;
                userModifiedIcon = true;
            }
            GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
            
            // Draw a border around the currently selected icon.
            if (selectedIconPath == iconPath)
            {
                EditorGUI.DrawRect(new Rect(r.x-2, r.y-2, r.width+4, r.height+4), Color.cyan);
                GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
            }
        }
        
        /// <summary>
        /// Draws the icon browser panel with a search field and a scrollable grid of icons.
        /// </summary>
        private void DrawIconBrowser()
        {
            GUILayout.Label("Icon Browser", EditorStyles.boldLabel);
            iconSearchFilter = EditorGUILayout.TextField("Search:", iconSearchFilter);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Find all textures in the specified icon folder and filter them by the search text.
            var allIcons = AssetDatabase.FindAssets("t:texture2d", new[] { "Assets/NewFiles/Editor/Styles/icons" })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => string.IsNullOrEmpty(iconSearchFilter) || Path.GetFileNameWithoutExtension(path).ToLower().Contains(iconSearchFilter.ToLower()))
                .ToArray();
                
            DrawIconGrid("Browser Results", allIcons);
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws the action buttons (Apply, Reset, Cancel) at the bottom of the window.
        /// </summary>
        private void DrawActionButtons()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Height(28))) ApplyCustomization();
            
            // Determine if any of the selected items currently have a customization.
            bool hasCustomization = false;
            if(currentTargetType == TargetType.Folder)
            {
                hasCustomization = targetFolderPaths.Any(p => NewFilesCore.HasCustomColor(p) || NewFilesCore.HasCustomIcon(p));
            }
            else // GameObject
            {
                hasCustomization = targetInstanceIDs.Any(id => (EditorUtility.InstanceIDToObject(id) as GameObject)?.GetComponent<HierarchyIcon>() != null);
            }

            // The "Reset" button is only enabled if there is something to reset.
            GUI.enabled = hasCustomization;
            if (GUILayout.Button("Reset", GUILayout.Height(28))) ResetCustomization();
            GUI.enabled = true;

            if (GUILayout.Button("Cancel", GUILayout.Height(28))) Close();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
        
        #region Logic

        /// <summary>
        /// Applies the selected color and icon to all target items.
        /// </summary>
        private void ApplyCustomization()
        {
            if (currentTargetType == TargetType.Folder)
            {
                foreach (var folderPath in targetFolderPaths)
                {
                    // Apply color if the user changed it.
                    if (userModifiedColor)
                    {
                        if (selectedColor.a > 0.001f) // A small alpha threshold to consider it a valid color.
                            NewFilesCore.SetFolderColor(folderPath, selectedColor);
                        else
                            NewFilesCore.ResetFolderColor(folderPath);
                    }
                    // Apply icon if the user changed it.
                    if (userModifiedIcon)
                    {
                        NewFilesCore.SetFolderIcon(folderPath, selectedIconPath);
                    }
                }
            }
            else // GameObject
            {
                Undo.SetCurrentGroupName("Set Hierarchy Customization");
                int group = Undo.GetCurrentGroup();

                foreach (var instanceID in targetInstanceIDs)
                {
                    var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                    if (go == null) continue;

                    var component = go.GetComponent<HierarchyIcon>();

                    // Skip if the user hasn't made any changes.
                    if (!userModifiedColor && !userModifiedIcon) continue;

                    // Determine if the GameObject will have an icon or color after the operation.
                    bool willHaveIcon = userModifiedIcon ? !string.IsNullOrEmpty(selectedIconPath) : (component != null && !string.IsNullOrEmpty(component.iconPath));
                    bool willHaveColor = userModifiedColor ? (selectedColor.a > 0.001f) : (component != null && component.hasCustomColor);

                    if (willHaveIcon || willHaveColor)
                    {
                        // Add or get the HierarchyIcon component and record it for Undo.
                        if (component == null) component = Undo.AddComponent<HierarchyIcon>(go);
                        Undo.RecordObject(component, "Modify Hierarchy Customization");

                        if (userModifiedColor)
                        {
                            component.hasCustomColor = (selectedColor.a > 0.001f);
                            component.backgroundColor = component.hasCustomColor ? selectedColor : Color.clear;
                        }
                        
                        if (userModifiedIcon)
                        {
                            component.iconPath = selectedIconPath;
                        }
                    }
                    else if (component != null)
                    {
                        // If no color or icon is set, remove the component.
                        Undo.DestroyObjectImmediate(component);
                    }
                    
                    // Mark the scene as dirty to ensure changes are saved.
                    if (!Application.isPlaying && go.scene.isLoaded) EditorSceneManager.MarkSceneDirty(go.scene);
                }
                
                Undo.CollapseUndoOperations(group);
            }
            
            // Add the applied color/icon to the history for future use.
            if (userModifiedColor && selectedColor.a > 0.001f) AddColorToHistory(selectedColor);
            if (userModifiedIcon && !string.IsNullOrEmpty(selectedIconPath)) AddIconToHistory(selectedIconPath);
            
            // Repaint windows to show the changes immediately.
            EditorApplication.RepaintProjectWindow();
            EditorApplication.RepaintHierarchyWindow();
            Close();
        }

        /// <summary>
        /// Resets all color and icon customizations for the target items.
        /// </summary>
        private void ResetCustomization()
        {
            if (currentTargetType == TargetType.Folder)
            {
                foreach (var folderPath in targetFolderPaths)
                {
                    NewFilesCore.ResetFolderColor(folderPath);
                    NewFilesCore.ResetFolderIcon(folderPath);
                }
            }
            else // GameObject
            {
                Undo.SetCurrentGroupName("Reset Hierarchy Customization");
                int group = Undo.GetCurrentGroup();

                foreach (var instanceID in targetInstanceIDs)
                {
                    var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                    var component = go?.GetComponent<HierarchyIcon>();
                    if (component != null)
                    {
                        // Remove the component to reset customization, supporting Undo.
                        Undo.DestroyObjectImmediate(component);
                        if (!Application.isPlaying && go.scene.isLoaded) EditorSceneManager.MarkSceneDirty(go.scene);
                    }
                }

                Undo.CollapseUndoOperations(group);
            }

            EditorApplication.RepaintProjectWindow();
            EditorApplication.RepaintHierarchyWindow();
            Close();
        }
        
        /// <summary>
        /// Handles keyboard shortcuts for Apply (Enter) and Cancel (Escape).
        /// </summary>
        private void HandleKeyboardInput()
        {
            Event e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter) { ApplyCustomization(); e.Use(); }
                else if (e.keyCode == KeyCode.Escape) { Close(); e.Use(); }
            }
        }
        
        #endregion
        
        #region History Management
        
        /// <summary>
        /// Adds a color to the history, ensuring no duplicates and respecting the maximum size.
        /// </summary>
        private static void AddColorToHistory(Color color)
        {
            // Remove any existing similar color to avoid duplicates.
            colorHistory.RemoveAll(c => Vector4.Distance(c, color) < 0.01f);
            colorHistory.Insert(0, color); // Add the new color to the front.
            if (colorHistory.Count > MAX_HISTORY_SIZE) colorHistory.RemoveAt(colorHistory.Count-1);
            SaveColorHistory();
        }

        /// <summary>
        /// Adds an icon path to the history, ensuring no duplicates and respecting the maximum size.
        /// </summary>
        private static void AddIconToHistory(string iconPath)
        {
            iconHistory.Remove(iconPath);
            iconHistory.Insert(0, iconPath);
            if (iconHistory.Count > MAX_ICON_HISTORY_SIZE) iconHistory.RemoveAt(iconHistory.Count-1);
            SaveIconHistory();
        }

        /// <summary>
        /// Saves the color history to EditorPrefs as a semicolon-separated string of hex codes.
        /// </summary>
        private static void SaveColorHistory()
        {
            string s = string.Join(";", colorHistory.Select(c => $"#{ColorUtility.ToHtmlStringRGB(c)}"));
            EditorPrefs.SetString(COLOR_HISTORY_KEY, s);
        }

        /// <summary>
        /// Loads the color history from EditorPrefs.
        /// </summary>
        private static void LoadColorHistory()
        {
            colorHistory.Clear();
            string s = EditorPrefs.GetString(COLOR_HISTORY_KEY, "");
            if (!string.IsNullOrEmpty(s))
            {
                colorHistory = s.Split(';').Select(h => ColorUtility.TryParseHtmlString(h, out var c) ? c : Color.clear).ToList();
            }
        }
        
        /// <summary>
        /// Saves the icon history to EditorPrefs as a semicolon-separated string of paths.
        /// </summary>
        private static void SaveIconHistory() => EditorPrefs.SetString(ICON_HISTORY_KEY, string.Join(";", iconHistory));

        /// <summary>
        /// Loads the icon history from EditorPrefs.
        /// </summary>
        private static void LoadIconHistory()
        {
            iconHistory.Clear();
            string s = EditorPrefs.GetString(ICON_HISTORY_KEY, "");
            if (!string.IsNullOrEmpty(s))
            {
                iconHistory = s.Split(';').Where(p => !string.IsNullOrEmpty(p)).ToList();
            }
        }
        
        #endregion
        
        #region Utilities
        
        /// <summary>
        /// Gets the list of predefined icon paths to display.
        /// It first tries to get them from settings, otherwise it falls back to a default search.
        /// </summary>
        private static string[] GetPredefinedIcons()
        {
            var settings = NewFilesSettings.Instance;
            if (settings.predefinedIconPaths != null && settings.predefinedIconPaths.Count > 0)
            {
                return settings.predefinedIconPaths.Where(p => !string.IsNullOrEmpty(p)).ToArray();
            }
            // Fallback: Find the first 20 icons in the default folder if none are set in settings.
            return AssetDatabase.FindAssets("t:texture2d", new[] { "Assets/NewFiles/Editor/Styles/icons" })
                .Take(20).Select(AssetDatabase.GUIDToAssetPath).ToArray();
        }
        
        #endregion
    }
}
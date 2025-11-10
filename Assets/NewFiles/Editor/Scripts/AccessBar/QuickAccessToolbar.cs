// Scripts/AccessBar/QuickAccessToolbar.cs

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Manages and renders a quick access toolbar within the Unity Project window's header.
    /// This class uses reflection to hook into the Project window's GUI repaint cycle,
    /// allowing it to draw a custom toolbar for frequently used assets.
    /// </summary>
    [InitializeOnLoad]
    public static class QuickAccessToolbar
    {
        // --- Private Fields ---

        #region State and References
        
        /// <summary>
        /// Flag to ensure the GUI hook is only applied once.
        /// </summary>
        private static bool isHooked = false;

        /// <summary>
        /// Cached reference to the Unity Editor's Project Browser window.
        /// </summary>
        private static EditorWindow projectBrowser;
        
        /// <summary>
        /// Cached type of the Project Browser window for reflection purposes.
        /// </summary>
        private static System.Type projectBrowserType;

        /// <summary>
        /// Cached reflection info for the ProjectBrowser.ShowFolderContents method.
        /// This is used to navigate *into* a folder when clicked.
        /// </summary>
        private static MethodInfo showFolderContentsMethod;
        
        /// <summary>
        /// Tracks the focus state of the Project Browser to re-apply the hook if necessary.
        /// </summary>
        private static bool lastFocusState = false;

        #endregion

        #region Styles and Layout
        
        // GUIStyles for rendering the toolbar elements.
        private static GUIStyle backgroundStyle;
        private static GUIStyle buttonStyle;
        private static GUIStyle itemButtonStyle;
        private static GUIStyle labelStyle;
        private static GUIStyle removeButtonStyle;
        
        /// <summary>
        /// Flag to prevent redundant style initialization.
        /// </summary>
        private static bool stylesInitialized = false;

        // Constants defining the toolbar's geometry and positioning.
        private static float ToolbarHeight = 18f;
        private static float ButtonSize = 18f;
        private static readonly float TOOLBAR_OFFSET_X = 45f;
        private static readonly float TOOLBAR_RIGHT_PADDING = 470f;
        
        #endregion

        #region Interaction State
        
        /// <summary>
        /// Stores the GUID of the item currently being hovered over by the mouse.
        /// </summary>
        private static string hoveredGuid = null;
        
        /// <summary>
        /// Stores the GUID of an item that is queued for removal.
        /// Removal is deferred to the end of the GUI cycle to avoid modification during iteration.
        /// </summary>
        private static string itemToRemove = null;
        
        #endregion

        // --- Initialization ---

        /// <summary>
        /// Static constructor called by Unity when the editor loads, due to the [InitializeOnLoad] attribute.
        /// Sets up the main update loop.
        /// </summary>
        static QuickAccessToolbar()
        {
            // Use delayCall to ensure the editor is fully initialized before we begin.
            EditorApplication.delayCall += () => {
                EditorApplication.update += Initialize;
            };
        }

        /// <summary>
        /// The main update method, called frequently by the editor.
        /// It finds the Project Browser and attempts to hook into its GUI.
        /// </summary>
        private static void Initialize()
        {
            FindProjectBrowser();

            if (projectBrowser != null)
            {
                bool currentFocus = projectBrowser.hasFocus;
                
                // If the window just regained focus, it might need re-hooking.
                if (!lastFocusState && currentFocus)
                {
                    isHooked = false;
                }
                lastFocusState = currentFocus;

                // Attempt to hook if not already hooked.
                if (!isHooked)
                {
                    bool success = TryHookIntoGUI();
                    if (success)
                    {
                        isHooked = true;
                        if (NewFilesSettings.Instance.enableDebugLog)
                        {
                            Debug.Log("[NewFiles] Quick Access Toolbar hooked successfully");
                        }
                    }
                }

                // Force a repaint of the project browser to draw our toolbar.
                if (projectBrowser.hasFocus && ShouldDrawToolbar())
                {
                    projectBrowser.Repaint();
                }
            }
        }

        /// <summary>
        /// Finds and caches a reference to the active Project Browser window
        /// and the 'ShowFolderContents' method via reflection.
        /// </summary>
        private static void FindProjectBrowser()
        {
            // Find the window if we don't have it
            if (projectBrowser == null)
            {
                var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
                projectBrowser = windows.FirstOrDefault(w => w.GetType().Name == "ProjectBrowser");

                if (projectBrowser != null)
                {
                    projectBrowserType = projectBrowser.GetType();
                }
            }
            
            // Find the method if we have the window type but not the method
            if (projectBrowserType != null && showFolderContentsMethod == null)
            {
                showFolderContentsMethod = projectBrowserType.GetMethod(
                    "ShowFolderContents",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    new System.Type[] { typeof(int), typeof(bool) }, // Signature: (int instanceID, bool revealAndFrame)
                    null
                );

                if (showFolderContentsMethod == null && NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.LogWarning("[NewFiles] Could not find ProjectBrowser.ShowFolderContents method. Folder navigation will fall back to 'ping'.");
                }
            }
        }
        
        // --- GUI Hooking via Reflection ---

        /// <summary>
        /// Tries to inject the DrawToolbar method into the Project Browser's GUI update loop.
        /// It attempts multiple reflection-based methods to support different Unity versions.
        /// </summary>
        /// <returns>True if the hook was successfully applied, false otherwise.</returns>
        private static bool TryHookIntoGUI()
        {
            if (projectBrowser == null || projectBrowserType == null) return false;
            
            // Attempt different hooking strategies.
            if (TryMethod1()) return true;
            if (TryMethod2()) return true;

            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.LogWarning("[NewFiles] Failed to hook the Quick Access Toolbar using primary methods.");
            }
            return false;
        }

        /// <summary>
        /// Primary hooking method. It finds the `m_Parent` view of the Project Browser
        /// and appends the DrawToolbar call to its `m_OnGUI` delegate.
        /// </summary>
        private static bool TryMethod1()
        {
            try
            {
                FieldInfo parentField = projectBrowserType.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
                if (parentField == null) return false;

                object parent = parentField.GetValue(projectBrowser);
                if (parent == null) return false;

                FieldInfo onGUIField = parent.GetType().GetField("m_OnGUI", BindingFlags.Instance | BindingFlags.NonPublic);
                if (onGUIField == null) return false;

                var originalDelegate = onGUIField.GetValue(parent) as System.Delegate;
                
                // Create a new action that calls the original delegate and then our toolbar drawing method.
                System.Action newAction = () =>
                {
                    originalDelegate?.DynamicInvoke();
                    DrawToolbar();
                };

                // Replace the old delegate with our new composite delegate.
                var newDelegate = System.Delegate.CreateDelegate(onGUIField.FieldType, newAction.Target, newAction.Method);
                onGUIField.SetValue(parent, newDelegate);
                return true;
            }
            catch (System.Exception e)
            {
                if (NewFilesSettings.Instance.enableDebugLog) Debug.LogWarning($"[NewFiles] Hook Method 1 failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// A secondary or fallback hooking method. (Currently a stub, can be implemented for other Unity versions).
        /// </summary>
        private static bool TryMethod2()
        {
            // This method can be implemented as an alternative hooking strategy if Method 1 fails
            // on certain Unity versions. It's left as a placeholder for future compatibility.
            return false;
        }
        
        // --- Drawing Logic ---

        /// <summary>
        /// Checks if the toolbar should be drawn based on settings and window state.
        /// </summary>
        private static bool ShouldDrawToolbar()
        {
            return NewFilesSettings.Instance.enableQuickAccessBar &&
                   projectBrowser != null &&
                   projectBrowser.position.width > 300; // Do not draw if the window is too narrow.
        }
        
        /// <summary>
        /// Verifies if the reference to the project browser is still valid.
        /// </summary>
        private static bool IsHookValid()
        {
            if (projectBrowser == null) return false;
            try { var pos = projectBrowser.position; return true; } // A simple check to see if the object is accessible.
            catch { return false; }
        }

        /// <summary>
        /// The main drawing entry point. This method is called every GUI frame for the Project window.
        /// </summary>
        private static void DrawToolbar()
        {
            if (!ShouldDrawToolbar()) return;
            
            // If the hook is broken (e.g., after a domain reload), re-initialize.
            if (!IsHookValid())
            {
                isHooked = false;
                Initialize();
                return;
            }

            try
            {
                InitializeStyles();
                Rect toolbarRect = CalculateToolbarRect();
                
                // Only draw the background during the Repaint event.
                if (Event.current.type == EventType.Repaint)
                {
                    // Clear hover state if the mouse leaves the toolbar area.
                    if (!toolbarRect.Contains(Event.current.mousePosition))
                    {
                        hoveredGuid = null;
                    }
                    DrawToolbarBackground(toolbarRect);
                }

                // Use GUILayout for flexible content arrangement.
                GUILayout.BeginArea(toolbarRect);
                GUILayout.BeginHorizontal();
                
                DrawToolbarContent();
                
                GUILayout.EndHorizontal();
                GUILayout.EndArea();

                // Handle drag-and-drop operations over the toolbar area.
                HandleDragAndDrop(toolbarRect);
                
                // Process any pending item removals at the end of the GUI cycle.
                if (!string.IsNullOrEmpty(itemToRemove))
                {
                    QuickAccessData.RemoveItem(itemToRemove);
                    itemToRemove = null;
                    hoveredGuid = null;
                    projectBrowser?.Repaint();
                }
            }
            catch (System.Exception e)
            {
                // Unhook on error to prevent continuous exceptions.
                isHooked = false; 
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.LogError($"[NewFiles] Error in DrawToolbar: {e.Message}\nStacktrace: {e.StackTrace}");
                }
            }
        }
        
        /// <summary>
        /// Initializes the GUIStyles used for drawing the toolbar.
        /// </summary>
        private static void InitializeStyles()
        {
            if (stylesInitialized) return;

            backgroundStyle = new GUIStyle("Toolbar") { fixedHeight = ToolbarHeight, stretchWidth = true };
            buttonStyle = new GUIStyle("ToolbarButton") { fixedWidth = ButtonSize, fixedHeight = ButtonSize - 2, margin = new RectOffset(2, 2, 1, 1), padding = new RectOffset(2, 2, 2, 2) };
            itemButtonStyle = new GUIStyle("ToolbarButton") { fixedHeight = ButtonSize - 2, margin = new RectOffset(2, 2, 1, 1), padding = new RectOffset(4, 4, 2, 2), alignment = TextAnchor.MiddleCenter, imagePosition = ImagePosition.ImageLeft, clipping = TextClipping.Clip };
            labelStyle = new GUIStyle("ToolbarButton") { fixedHeight = ButtonSize - 2, margin = new RectOffset(4, 4, 1, 1), padding = new RectOffset(4, 4, 2, 2), alignment = TextAnchor.MiddleLeft, fontSize = 11 };
            removeButtonStyle = new GUIStyle(EditorStyles.label) { padding = new RectOffset(0, 0, 0, 0), fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter, fixedWidth = 16f, fixedHeight = 16f };
            
            stylesInitialized = true;
        }

        /// <summary>
        /// Calculates the screen rectangle for the toolbar based on the Project window's size.
        /// </summary>
        private static Rect CalculateToolbarRect()
        {
            if (projectBrowser == null) return Rect.zero;

            float x = TOOLBAR_OFFSET_X;
            float y = 3f;
            float width = projectBrowser.position.width - x - TOOLBAR_RIGHT_PADDING;
            float height = ToolbarHeight;

            // Ensure the toolbar does not become too small.
            width = Mathf.Max(width, 100f); 
            return new Rect(x, y, width, height);
        }
        
        /// <summary>
        /// Renders the semi-transparent background of the toolbar.
        /// </summary>
        private static void DrawToolbarBackground(Rect rect)
        {
            float opacity = NewFilesSettings.Instance.quickAccessBackgroundOpacity;
            
            if (backgroundStyle != null)
            {
                Color prevColor = GUI.color;
                GUI.color = new Color(1f, 1f, 1f, opacity);
                GUI.Box(rect, GUIContent.none, backgroundStyle);
                GUI.color = prevColor;
            }
            else
            {
                // Fallback drawing if styles fail to initialize.
                EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, opacity));
            }
        }
        
        /// <summary>
        /// Lays out the primary content of the toolbar: label, items, and control buttons.
        /// </summary>
        private static void DrawToolbarContent()
        {
            GUILayout.Label("Quick:", labelStyle, GUILayout.Width(45));
            GUILayout.Space(2);
            
            var items = QuickAccessData.GetItemsByRecentAccess();
            if (items.Count == 0)
            {
                GUILayout.Label("Drag files/folders here", labelStyle);
            }
            else
            {
                DrawQuickAccessItems(items);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.Space(5);
            DrawSeparator();
            GUILayout.Space(5);
            
            DrawControlButtons();
        }

        /// <summary>
        /// Draws a vertical separator line.
        /// </summary>
        private static void DrawSeparator()
        {
            Rect rect = GUILayoutUtility.GetRect(1, 18, GUILayout.Width(1));
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
        }

        /// <summary>
        /// Renders the quick access items that fit within the available toolbar width.
        /// </summary>
        /// <param name="items">A list of all items, sorted by access time.</param>
        private static void DrawQuickAccessItems(List<QuickAccessItem> items)
        {
            if (projectBrowser == null) return;
            
            // Calculate available width to avoid overflow.
            float availableWidth = projectBrowser.position.width - TOOLBAR_OFFSET_X - TOOLBAR_RIGHT_PADDING - 50;
            var itemsToDraw = new List<QuickAccessItem>();
            float currentWidth = 0f;
            float itemWidth = NewFilesSettings.Instance.quickAccessItemWidth;

            // Determine which items will fit on the toolbar.
            foreach (var item in items)
            {
                string fileName = System.IO.Path.GetFileName(AssetDatabase.GUIDToAssetPath(item.guid));
                float neededWidth = Mathf.Max(itemWidth, fileName.Length * 7 + 40); // Estimate width.

                if (currentWidth + neededWidth <= availableWidth)
                {
                    itemsToDraw.Add(item);
                    currentWidth += neededWidth + 10;
                }
                else
                {
                    break; // No more space.
                }
            }

            // Draw the visible items.
            for (int i = 0; i < itemsToDraw.Count; i++)
            {
                if (itemsToDraw[i].IsValid())
                {
                    DrawQuickAccessButton(itemsToDraw[i]);
                    
                    if (i < itemsToDraw.Count - 1)
                    {
                        GUILayout.Space(2);
                        DrawSeparator();
                        GUILayout.Space(2);
                    }
                }
            }
            
            // If some items were hidden, show a "+N" indicator.
            if (items.Count > itemsToDraw.Count)
            {
                GUILayout.Space(5);
                GUILayout.Label($"+{items.Count - itemsToDraw.Count}", labelStyle, GUILayout.Width(30));
            }
        }

        /// <summary>
        /// Draws a single button for a quick access item, handling all its interaction logic.
        /// </summary>
        /// <param name="item">The quick access item to draw.</param>
        private static void DrawQuickAccessButton(QuickAccessItem item)
        {
            string path = AssetDatabase.GUIDToAssetPath(item.guid);
            if (string.IsNullOrEmpty(path)) return;

            // Prepare button content (icon, text, tooltip).
            Texture2D icon = AssetDatabase.GetCachedIcon(path) as Texture2D ?? EditorGUIUtility.FindTexture("DefaultAsset Icon");
            string fileName = System.IO.Path.GetFileName(path);
            float itemWidth = NewFilesSettings.Instance.quickAccessItemWidth;
            string tooltip = NewFilesSettings.Instance.showQuickAccessTooltips ? path : string.Empty;
            GUIContent buttonContent = new GUIContent(CalculateOptimalDisplayName(fileName, itemWidth - 20f, itemButtonStyle, icon), icon, tooltip);

            // Get button rectangle and calculate the remove 'x' button's sub-rectangle.
            Rect buttonRect = GUILayoutUtility.GetRect(buttonContent, itemButtonStyle, GUILayout.MinWidth(60), GUILayout.MaxWidth(itemWidth));
            float xButtonSize = 16f;
            Rect xButtonRect = new Rect(buttonRect.xMax - xButtonSize - 2f, buttonRect.y + (buttonRect.height - xButtonSize) / 2f, xButtonSize, xButtonSize);
            
            // Determine hover state for the main button and the remove 'x' button.
            bool isHoveringButton = buttonRect.Contains(Event.current.mousePosition);
            bool isHoveringX = xButtonRect.Contains(Event.current.mousePosition) && isHoveringButton;

            // Handle clicking the 'x' button.
            if (isHoveringButton && hoveredGuid == item.guid)
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && isHoveringX)
                {
                    itemToRemove = item.guid; // Queue for removal.
                    Event.current.Use();
                    return;
                }
            }
            
            // Store original GUI colors to restore them later.
            Color originalBackgroundColor = GUI.backgroundColor;
            Color originalContentColor = GUI.contentColor;
            Color originalGuiColor = GUI.color;

            // Apply custom folder colors if applicable.
            if (AssetDatabase.IsValidFolder(path))
            {
                icon = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
                if (NewFilesCore.HasCustomColor(path))
                {
                    Color folderColor = NewFilesCore.GetFolderColor(path);
                    GUI.backgroundColor = folderColor;
                    GUI.backgroundColor = new Color(GUI.backgroundColor.r, GUI.backgroundColor.g, GUI.backgroundColor.b, NewFilesSettings.Instance.quickAccessItemBackgroundOpacity);
                    float intensity = 1.35f;
                    GUI.color = new Color(folderColor.r * intensity, folderColor.g * intensity, folderColor.b * intensity, 1f);
                }
            }
            
            // The main button drawing and click detection logic.
            if (GUI.Button(buttonRect, buttonContent, itemButtonStyle))
            {
                // This block executes only on the frame the button is clicked.
                // We only process the click if the mouse was not over the 'x' area.
                if (!isHoveringX)
                {
                    HandleItemClick(item, path);
                }
            }

            // Restore original GUI colors.
            GUI.backgroundColor = originalBackgroundColor;
            GUI.contentColor = originalContentColor;
            GUI.color = originalGuiColor;

            // Update the global hover state.
            if (isHoveringButton && !isHoveringX)
            {
                hoveredGuid = item.guid;
            }

            // Draw the remove 'x' button overlay if the main button is hovered.
            if (hoveredGuid == item.guid && isHoveringButton)
            {
                Color prevContentColor = GUI.contentColor;
                GUI.contentColor = isHoveringX ? Color.red : Color.white;
                GUI.Label(xButtonRect, "×", removeButtonStyle);
                GUI.contentColor = prevContentColor;
            }

            // Handle right-click context menu to remove the item.
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && buttonRect.Contains(Event.current.mousePosition))
            {
                itemToRemove = item.guid;
                Event.current.Use();
            }
        }
        
        /// <summary>
        /// Truncates a string with "..." to fit within a given pixel width.
        /// </summary>
        /// <param name="originalName">The full name to display.</param>
        /// <param name="availableWidth">The maximum width in pixels for the text.</param>
        /// <param name="style">The GUIStyle used for rendering, to calculate text size.</param>
        /// <param name="icon">An optional icon, to account for its width.</param>
        /// <returns>A truncated string that fits the available space.</returns>
        private static string CalculateOptimalDisplayName(string originalName, float availableWidth, GUIStyle style, Texture2D icon = null)
        {
            if (string.IsNullOrEmpty(originalName)) return "";

            float iconWidth = (icon != null) ? 20f : 0f;
            float textWidth = availableWidth - iconWidth - style.padding.horizontal - 8f;
            
            if (textWidth < 30f) return "...";

            if (style.CalcSize(new GUIContent(originalName)).x <= textWidth)
            {
                return originalName;
            }

            // Reduce string length until it fits.
            string result = originalName;
            for (int i = originalName.Length - 1; i > 0; --i)
            {
                result = originalName.Substring(0, i) + "...";
                if (style.CalcSize(new GUIContent(result)).x <= textWidth)
                {
                    return result;
                }
            }
            return "...";
        }

        /// <summary>
        /// Draws the control buttons on the right side of the toolbar (e.g., Clear, Settings).
        /// </summary>
        private static void DrawControlButtons()
        {
            // Clear all button.
            GUI.enabled = QuickAccessData.GetItems().Count > 0;
            if (GUILayout.Button(new GUIContent("×", "Clear all items"), buttonStyle, GUILayout.Width(ButtonSize)))
            {
                if (EditorUtility.DisplayDialog("Clear Quick Access", "Remove all items from Quick Access?", "Yes", "Cancel"))
                {
                    QuickAccessData.ClearAll();
                }
            }
            GUI.enabled = true;

            // Settings button.
            if (GUILayout.Button(new GUIContent("⚙", "Settings"), buttonStyle, GUILayout.Width(ButtonSize)))
            {
                ShowSettingsMenu();
            }
        }
        
        // --- Event Handlers ---

        /// <summary>
        /// Handles drag-and-drop events to add new items to the toolbar.
        /// </summary>
        /// <param name="dropArea">The rectangle where drops are accepted.</param>
        private static void HandleDragAndDrop(Rect dropArea)
        {
            Event evt = Event.current;
            if (!dropArea.Contains(evt.mousePosition)) return;
            
            switch (evt.type)
            {
                case EventType.DragUpdated:
                    // Show a copy icon if the dragged objects are valid assets.
                    DragAndDrop.visualMode = CanAcceptDraggedObjects() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    evt.Use();
                    break;

                case EventType.DragPerform:
                    // On drop, accept the drag and add the assets to the quick access list.
                    if (CanAcceptDraggedObjects())
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (string path in DragAndDrop.paths)
                        {
                            string guid = AssetDatabase.AssetPathToGUID(path);
                            if (!string.IsNullOrEmpty(guid)) QuickAccessData.AddItem(guid);
                        }
                        evt.Use();
                    }
                    break;
            }
        }
        
        /// <summary>
        /// Checks if the currently dragged objects are valid project assets.
        /// </summary>
        private static bool CanAcceptDraggedObjects()
        {
            return DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 &&
                   DragAndDrop.paths.Any(path => !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(path)));
        }

        /// <summary>
        /// --- MODIFIED ---
        /// Defines the action to take when a quick access item is clicked.
        /// If it's a folder, it navigates into it.
        /// If it's an asset, it pings it.
        /// </summary>
        /// <param name="item">The item that was clicked.</param>
        /// <param name="path">The asset path of the clicked item.</param>
        private static void HandleItemClick(QuickAccessItem item, string path)
        {
            QuickAccessData.UpdateItemAccess(item.guid);
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            if (asset == null) return;

            Event evt = Event.current;
            if (evt.control || evt.command)
            {
                // Ctrl/Cmd + Click: Ping the object without changing selection.
                EditorGUIUtility.PingObject(asset);
            }
            else if (AssetDatabase.IsValidFolder(path))
            {
                // --- BEHAVIOR 1: Navigate into the folder ---
                if (showFolderContentsMethod != null && projectBrowser != null)
                {
                    try
                    {
                        // Call projectBrowser.ShowFolderContents(asset.GetInstanceID(), true)
                        showFolderContentsMethod.Invoke(projectBrowser, new object[] { asset.GetInstanceID(), true });
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[NewFiles] Error invoking ShowFolderContents: {e.Message}");
                        // Fallback to old ping behavior
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);
                    }
                }
                else
                {
                    // Fallback if reflection failed (e.g., Unity version change)
                    if (NewFilesSettings.Instance.enableDebugLog)
                    {
                         Debug.LogWarning("[NewFiles] ShowFolderContents method not found. Falling back to ping.");
                    }
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
                // --- END BEHAVIOR 1 ---
            }
            else
            {
                // --- BEHAVIOR 2 (MODIFIED): Select and ping the asset ---
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
                // --- END BEHAVIOR 2 ---
            }
        }
        
        // --- Settings Menu ---

        /// <summary>
        /// Displays a generic menu with settings related to the quick access bar.
        /// </summary>
        private static void ShowSettingsMenu()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Enable Quick Access Bar"), NewFilesSettings.Instance.enableQuickAccessBar, ToggleQuickAccessBar);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Clear All Items"), false, () => {
                if (EditorUtility.DisplayDialog("Clear Quick Access", "Remove all items from Quick Access?", "Yes", "Cancel"))
                {
                    QuickAccessData.ClearAll();
                }
            });
            menu.ShowAsContext();
        }

        /// <summary>
        /// Menu item to enable or disable the toolbar. Accessible from "Tools/NewFiles".
        /// </summary>
        [MenuItem("Tools/NewFiles/Toggle Quick Access Toolbar")]
        public static void ToggleQuickAccessBar()
        {
            var settings = NewFilesSettings.Instance;
            settings.enableQuickAccessBar = !settings.enableQuickAccessBar;
            NewFilesSettings.SaveSettings();
            
            projectBrowser?.Repaint();
            Debug.Log($"[NewFiles] Quick Access Bar {(settings.enableQuickAccessBar ? "enabled" : "disabled")}");
        }
    }
}
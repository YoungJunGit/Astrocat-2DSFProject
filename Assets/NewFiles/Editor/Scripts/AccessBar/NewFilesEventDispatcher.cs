using UnityEngine;
using UnityEditor;
using NewFiles.Runtime;
using System.Collections.Generic;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Centralized dispatcher for GUI events in the Project and Hierarchy windows.
    /// This static class ensures that all custom GUI drawing and input handling
    /// are managed from a single, reliable entry point, preventing conflicts.
    /// It uses [InitializeOnLoad] to hook into Unity's editor events automatically.
    /// </summary>
    [InitializeOnLoad]
    public static class NewFilesEventDispatcher
    {
        private static bool isInitialized = false;

        /// <summary>
        /// Static constructor called automatically by Unity when the editor loads.
        /// </summary>
        static NewFilesEventDispatcher()
        {
            Initialize();
        }

        /// <summary>
        /// Subscribes to the necessary EditorApplication events.
        /// The unsubscribe/subscribe pattern prevents duplicate subscriptions during script reloads.
        /// </summary>
        private static void Initialize()
        {
            if (isInitialized) return;

            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;

            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemGUI;

            isInitialized = true;

            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log("[NewFiles] Event Dispatcher initialized for Project & Hierarchy");
            }
        }

        /// <summary>
        /// Handles GUI events for each item drawn in the Project window.
        /// This method is responsible for both drawing custom folder icons and handling
        /// input to open the color picker.
        /// </summary>
        /// <param name="guid">The asset GUID of the item being drawn.</param>
        /// <param name="selectionRect">The rect of the item in the window.</param>
        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            try
            {
                GUIDebugger.Begin("OnProjectWindowItemGUI");

                // Early exit if the feature is disabled in settings.
                if (!NewFilesSettings.Instance.isEnabled) return;
                
                // Ignore invalid items or events.
                if (selectionRect.width <= 0 || selectionRect.height <= 0) return;
                if (string.IsNullOrEmpty(guid)) return;

                Event currentEvent = Event.current;
                if (currentEvent == null) return;

                // --- Input Handling for Opening the Color Picker ---
                // Triggered by Alt + Left Mouse Click on a folder.
                if (currentEvent.type == EventType.MouseDown &&
                    currentEvent.button == 0 &&
                    currentEvent.alt &&
                    selectionRect.Contains(currentEvent.mousePosition))
                {
                    var selectedGUIDs = Selection.assetGUIDs;
                    List<string> selectedFolderPaths;

                    // Handle multi-selection.
                    if (selectedGUIDs.Length > 1)
                    {
                        selectedFolderPaths = selectedGUIDs
                            .Select(g => AssetDatabase.GUIDToAssetPath(g))
                            .Where(p => !string.IsNullOrEmpty(p) && AssetDatabase.IsValidFolder(p))
                            .ToList();
                    }
                    else // Handle single selection.
                    {
                        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                        selectedFolderPaths = new List<string>();
                        if (!string.IsNullOrEmpty(assetPath) && AssetDatabase.IsValidFolder(assetPath))
                        {
                            selectedFolderPaths.Add(assetPath);
                        }
                    }

                    // If any valid folders are selected, show the color picker window.
                    if (selectedFolderPaths.Count > 0)
                    {
                        Vector2 screenPosition = GUIUtility.GUIToScreenPoint(
                            new Vector2(selectionRect.x, selectionRect.y + selectionRect.height));
                        
                        ColorPickerWindow.ShowWindowForFolders(selectedFolderPaths, screenPosition);
                        currentEvent.Use(); // Consume the event to prevent other actions.
                        return;
                    }
                }

                // --- Drawing Logic ---
                // During the Repaint event, draw the custom folder graphics.
                if (currentEvent.type == EventType.Repaint)
                {
                    try
                    {
                        NewFilesDrawer.DrawCustomFolderGUI(guid, selectionRect);
                    }
                    catch (System.Exception e)
                    {
                        if (NewFilesSettings.Instance.enableDebugLog)
                        {
                            Debug.LogError($"[NewFiles] Error drawing folder customization: {e.Message}");
                        }
                    }
                }
            }
            finally
            {
                // Ensures GUI state is always cleaned up, even if errors occur.
                GUIDebugger.End("OnProjectWindowItemGUI");
                GUIDebugger.CheckCleanState();
            }
        }

        /// <summary>
        /// Handles GUI events for each item drawn in the Hierarchy window.
        /// This method is responsible for handling input to open the color picker for GameObjects.
        /// </summary>
        /// <param name="instanceID">The instance ID of the GameObject being drawn.</param>
        /// <param name="selectionRect">The rect of the item in the window.</param>
        private static void OnHierarchyWindowItemGUI(int instanceID, Rect selectionRect)
        {
            var settings = NewFilesSettings.Instance;
            if (!settings.isEnabled || !settings.enableHierarchyIcons) return;

            Event e = Event.current;
            
            // Triggered by Alt + Left Mouse Click on a GameObject.
            if (e.type == EventType.MouseDown && e.button == 0 && e.alt && selectionRect.Contains(e.mousePosition))
            {
                var selectedIDs = Selection.instanceIDs;
                List<int> validGameObjectIDs;

                // Handle multi-selection.
                if (selectedIDs.Length > 1)
                {
                    validGameObjectIDs = selectedIDs
                        .Where(id => EditorUtility.InstanceIDToObject(id) is GameObject)
                        .ToList();
                }
                else // Handle single selection.
                {
                    validGameObjectIDs = new List<int>();
                    if (EditorUtility.InstanceIDToObject(instanceID) is GameObject)
                    {
                        validGameObjectIDs.Add(instanceID);
                    }
                }

                // If any valid GameObjects are selected, show the color picker window.
                if (validGameObjectIDs.Count > 0)
                {
                    Vector2 pos = GUIUtility.GUIToScreenPoint(new Vector2(selectionRect.x, selectionRect.y + selectionRect.height));
                    ColorPickerWindow.ShowWindowForGameObjects(validGameObjectIDs, pos);
                    e.Use(); // Consume the event.
                }
            }
        }

        /// <summary>
        /// Unsubscribes from all editor events. This method is obsolete and generally not needed
        /// due to the robust initialization logic, but is kept for manual cleanup if ever required.
        /// </summary>
        [System.Obsolete]
        static void Cleanup()
        {
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemGUI;
            isInitialized = false;
        }
    }
}
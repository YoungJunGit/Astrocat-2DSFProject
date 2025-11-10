using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Provides additional menu items in the Unity Editor for managing folder icons.
    /// </summary>
    public static class IconManagementMenu
    {
        // --- START: New Context Menu Handlers ---

        private const int MENU_PRIORITY = 1000;

        /// <summary>
        /// Context menu item for customizing folders in the Project window.
        /// </summary>
        [MenuItem("Assets/Customize - New Files", false, MENU_PRIORITY)]
        private static void CustomizeProjectAsset(MenuCommand command)
        {
            var selectedGUIDs = Selection.assetGUIDs;
            if (selectedGUIDs == null || selectedGUIDs.Length == 0) return;

            List<string> selectedFolderPaths = selectedGUIDs
                .Select(g => AssetDatabase.GUIDToAssetPath(g))
                .Where(p => !string.IsNullOrEmpty(p) && AssetDatabase.IsValidFolder(p))
                .ToList();

            if (selectedFolderPaths.Count > 0)
            {
                // --- FIX ---
                // Event.current is null in a [MenuItem] callback, causing a NullReferenceException.
                // We must get a valid screen position without it.
                // We'll use the center of the currently focused editor window as a fallback.
                Vector2 screenPosition;
                if (EditorWindow.focusedWindow != null)
                {
                    // Get the center of the window (e.g., the Project window)
                    screenPosition = EditorWindow.focusedWindow.position.center;
                }
                else
                {
                    // Absolute fallback if no window is focused
                    screenPosition = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
                }
                // --- END FIX ---
                
                ColorPickerWindow.ShowWindowForFolders(selectedFolderPaths, screenPosition);
            }
        }

        /// <summary>
        /// Validates the "Customize - New Files" menu item in the Project window.
        /// It's enabled only if at least one selected asset is a folder.
        /// </summary>
        [MenuItem("Assets/Customize - New Files", true)]
        private static bool ValidateCustomizeProjectAsset()
        {
            // Enable if the feature is on and the selection contains at least one folder.
            return NewFilesSettings.Instance.isEnabled &&
                   Selection.assetGUIDs.Any(g => AssetDatabase.IsValidFolder(AssetDatabase.GUIDToAssetPath(g)));
        }

        /// <summary>
        /// Context menu item for customizing GameObjects in the Hierarchy window.
        /// </summary>
        [MenuItem("GameObject/Customize - New Files", false, MENU_PRIORITY)]
        private static void CustomizeHierarchyObject(MenuCommand command)
        {
            var selectedIDs = Selection.instanceIDs;
            if (selectedIDs == null || selectedIDs.Length == 0) return;

            List<int> validGameObjectIDs = selectedIDs
                .Where(id => EditorUtility.InstanceIDToObject(id) is GameObject)
                .ToList();

            if (validGameObjectIDs.Count > 0)
            {
                // --- FIX ---
                // Event.current is null in a [MenuItem] callback.
                // We use the same fix as CustomizeProjectAsset.
                Vector2 screenPosition;
                if (EditorWindow.focusedWindow != null)
                {
                    // Get the center of the window (e.g., the Hierarchy window)
                    screenPosition = EditorWindow.focusedWindow.position.center;
                }
                else
                {
                    // Absolute fallback
                    screenPosition = new Vector2(Screen.currentResolution.width / 2, Screen.currentResolution.height / 2);
                }
                // --- END FIX ---

                ColorPickerWindow.ShowWindowForGameObjects(validGameObjectIDs, screenPosition);
            }
        }

        /// <summary>
        /// Validates the "Customize - New Files" menu item in the Hierarchy window.
        /// It's enabled only if at least one selected item is a GameObject.
        /// </summary>
        [MenuItem("GameObject/Customize - New Files", true)]
        private static bool ValidateCustomizeHierarchyObject()
        {
            // Enable if the feature is on and the selection contains at least one GameObject.
            return NewFilesSettings.Instance.isEnabled &&
                   NewFilesSettings.Instance.enableHierarchyIcons &&
                   Selection.instanceIDs.Any(id => EditorUtility.InstanceIDToObject(id) is GameObject);
        }

        // --- END: New Context Menu Handlers ---

        /// <summary>
        /// Adds a menu item to reset all custom folder icons to their default state.
        /// It displays a confirmation dialog before proceeding with the operation.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Reset All Icons")]
        public static void ResetAllIcons()
        {
            // Display a confirmation dialog to prevent accidental resets.
            if (EditorUtility.DisplayDialog("Reset All Icons", 
                "Are you sure you want to remove all custom folder icons?", 
                "Yes", "Cancel"))
            {
                // If confirmed, call the core method to perform the reset.
                NewFilesCore.ResetAllIcons();
                Debug.Log("[NewFiles] All custom folder icons have been reset.");
            }
        }

        /// <summary>
        /// Adds a menu item to open the asset's default icons folder in the system's file explorer.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Show Icons Folder")]
        public static void ShowIconsFolder()
        {
            // Define the relative path to the icons folder.
            string iconsPath = "Assets/NewFiles/Editor/Styles/icons";
            
            // Check if the directory exists before attempting to open it.
            if (Directory.Exists(iconsPath))
            {
                // Open the folder in the OS file explorer (Finder on macOS, Explorer on Windows).
                EditorUtility.RevealInFinder(iconsPath);
            }
            else
            {
                // If the folder is not found, show an error message.
                EditorUtility.DisplayDialog("Icons Folder Not Found", 
                    $"The icons folder was not found at:\n{iconsPath}\n\nPlease make sure the NewFiles package is properly installed.", 
                    "OK");
            }
        }

        /// <summary>
        /// Adds a menu item to validate the files within the icons folder.
        /// It checks for valid image extensions and reports the results.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Validate Icons")]
        public static void ValidateIcons()
        {
            // Define the path to the icons folder.
            string iconsPath = "Assets/NewFiles/Editor/Styles/icons";
            
            // Exit if the icons folder doesn't exist.
            if (!Directory.Exists(iconsPath))
            {
                EditorUtility.DisplayDialog("Validation Failed", 
                    $"Icons folder not found at:\n{iconsPath}", 
                    "OK");
                return;
            }

            // Get all files from the icons directory and its subdirectories.
            var iconFiles = Directory.GetFiles(iconsPath, "*.*", SearchOption.AllDirectories);
            // Define an array of supported image file extensions.
            var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tga" };
            
            int totalFiles = iconFiles.Length;
            int validIcons = 0;
            int invalidIcons = 0;

            // Iterate over each file to check its extension.
            foreach (var file in iconFiles)
            {
                string extension = Path.GetExtension(file).ToLower();
                if (System.Array.Exists(validExtensions, ext => ext == extension))
                {
                    validIcons++;
                }
                else
                {
                    invalidIcons++;
                }
            }

            // Prepare a summary message with the validation results.
            string message = $"Icon Validation Results:\n\n" +
                           $"Total files: {totalFiles}\n" +
                           $"Valid icons: {validIcons}\n" +
                           $"Invalid files: {invalidIcons}\n\n" +
                           $"Icons folder: {iconsPath}";

            // Display the results in a dialog box and log it to the console.
            EditorUtility.DisplayDialog("Icon Validation", message, "OK");
            Debug.Log($"[NewFiles] {message}");
        }

        /// <summary>
        /// Adds a menu item to clear any cached icon data, forcing a refresh.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Clear Icon Cache")]
        public static void ClearIconCache()
        {
            // Calls a method to clear internal caches.
            NewFilesDrawer.ClearAllCaches();
            Debug.Log("[NewFiles] Icon cache cleared successfully.");
        }

        /// <summary>
        /// It copies valid image files from a user-selected folder to the asset's icon directory.
        /// </summary>
        [MenuItem("Tools/NewFiles/Icon Management/Import Icon Pack...")]
        public static void ImportIconPack()
        {
            // Open a folder selection dialog for the user.
            string selectedPath = EditorUtility.OpenFolderPanel("Select Icon Pack Folder", "", "");
            
            // If the user cancels the dialog, do nothing.
            if (string.IsNullOrEmpty(selectedPath))
                return;

            // Define the target directory for the imported icons.
            string iconsPath = "Assets/NewFiles/Editor/Styles/icons";
            
            // If the target directory doesn't exist, create it.
            if (!Directory.Exists(iconsPath))
            {
                Directory.CreateDirectory(iconsPath);
                AssetDatabase.Refresh(); // Refresh Unity's asset database to recognize the new folder.
            }

            // Get all files from the source directory.
            var sourceFiles = Directory.GetFiles(selectedPath, "*.*", SearchOption.AllDirectories);
            // Define an array of supported image file extensions.
            var validExtensions = new[] { ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tga" };
            
            int importedCount = 0;
            int skippedCount = 0;

            // Process each file from the source folder.
            foreach (var sourceFile in sourceFiles)
            {
                string extension = Path.GetExtension(sourceFile).ToLower();
                // Check if the file has a valid image extension.
                if (System.Array.Exists(validExtensions, ext => ext == extension))
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string targetPath = Path.Combine(iconsPath, fileName);
                    
                    // Copy the file only if it doesn't already exist in the target directory.
                    if (!File.Exists(targetPath))
                    {
                        File.Copy(sourceFile, targetPath);
                        importedCount++;
                    }
                    else
                    {
                        // Skip files that already exist to avoid duplicates.
                        skippedCount++;
                    }
                }
            }

            // Refresh the asset database so Unity detects the new icon files.
            AssetDatabase.Refresh();

            // Prepare a summary message for the user.
            string message = $"Import completed!\n\n" +
                           $"Imported: {importedCount} icons\n" +
                           $"Skipped: {skippedCount} (already exist)\n\n" +
                           $"Icons imported to: {iconsPath}";

            // Display the summary in a dialog box and log it to the console.
            EditorUtility.DisplayDialog("Icon Import Complete", message, "OK");
            Debug.Log($"[NewFiles] {message}");
        }
    }
}
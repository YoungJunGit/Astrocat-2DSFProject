using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NewFiles.Editor
{
    /// <summary>
    /// Main controller for the NewFiles system.
    /// This class manages all folder customizations (colors and icons) and handles data persistence.
    /// It uses a lazy loading approach to ensure data is loaded only when first needed,
    /// preventing slow domain reloads in the editor.
    /// </summary>
    [InitializeOnLoad]
    public static class NewFilesCore
    {
        /// <summary>
        /// Flag to ensure data is loaded only once per Unity session.
        /// </summary>
        private static bool _dataLoaded = false;

        /// <summary>
        /// In-memory cache for folder path-to-color mappings.
        /// </summary>
        private static Dictionary<string, Color> folderColors = new Dictionary<string, Color>();

        /// <summary>
        /// In-memory cache for folder path-to-icon path mappings.
        /// </summary>
        private static Dictionary<string, string> folderIcons = new Dictionary<string, string>();

        /// <summary>
        /// Static constructor required by the [InitializeOnLoad] attribute.
        /// It is intentionally left empty to prevent performance overhead on every script recompile.
        /// The actual initialization is deferred to EnsureDataIsLoaded().
        /// </summary>
        static NewFilesCore()
        {
            // The constructor remains empty to optimize editor performance.
        }

        /// <summary>
        /// Ensures that all necessary data (settings, colors, icons) is loaded from persistence.
        /// This method implements a lazy loading pattern, executing the load operations only once
        /// when the data is first requested.
        /// </summary>
        private static void EnsureDataIsLoaded()
        {
            // If data is already loaded for this session, do nothing.
            if (_dataLoaded) return;

            // Check if settings file exists, create a default one if not.
            if (!NewFilesSettings.Exists())
            {
                NewFilesSettings.CreateDefault();
            }

            // Load customization data from EditorPrefs.
            LoadFolderColors();
            LoadFolderIcons();

            // Mark data as loaded to prevent re-initialization.
            _dataLoaded = true;

            // Optional debug log.
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log($"[NewFiles] Data loaded on first use (Lazy Load).");
            }
        }

        #region Color Management

        /// <summary>
        /// Assigns a custom color to a specified folder path.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <param name="color">The color to apply.</param>
        public static void SetFolderColor(string folderPath, Color color)
        {
            EnsureDataIsLoaded();

            if (folderColors.ContainsKey(folderPath))
            {
                folderColors[folderPath] = color;
            }
            else
            {
                folderColors.Add(folderPath, color);
            }

            SaveFolderColors();
            RefreshProjectWindow();
        }

        /// <summary>
        /// Removes any custom color associated with a folder path.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        public static void ResetFolderColor(string folderPath)
        {
            EnsureDataIsLoaded();

            if (folderColors.ContainsKey(folderPath))
            {
                folderColors.Remove(folderPath);
                SaveFolderColors();
                RefreshProjectWindow();
            }
        }

        /// <summary>
        /// Retrieves the custom color for a folder.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <returns>The custom color, or Color.clear if none is set.</returns>
        public static Color GetFolderColor(string folderPath)
        {
            EnsureDataIsLoaded();
            return folderColors.ContainsKey(folderPath) ? folderColors[folderPath] : Color.clear;
        }

        /// <summary>
        /// Checks if a folder has a custom color applied.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <returns>True if a custom color is set, false otherwise.</returns>
        public static bool HasCustomColor(string folderPath)
        {
            EnsureDataIsLoaded();
            return folderColors.ContainsKey(folderPath);
        }

        #endregion

        #region Icon Management

        /// <summary>
        /// Assigns a custom icon to a specified folder path.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <param name="iconPath">The asset path of the icon texture.</param>
        public static void SetFolderIcon(string folderPath, string iconPath)
        {
            EnsureDataIsLoaded();

            // If the provided icon path is null or empty, treat it as a reset request.
            if (string.IsNullOrEmpty(iconPath))
            {
                ResetFolderIcon(folderPath);
                return;
            }

            if (folderIcons.ContainsKey(folderPath))
            {
                folderIcons[folderPath] = iconPath;
            }
            else
            {
                folderIcons.Add(folderPath, iconPath);
            }

            SaveFolderIcons();
            RefreshProjectWindow();

            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log($"[NewFiles] Set icon for '{folderPath}': {iconPath}");
            }
        }

        /// <summary>
        /// Removes any custom icon associated with a folder path.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        public static void ResetFolderIcon(string folderPath)
        {
            EnsureDataIsLoaded();

            if (folderIcons.ContainsKey(folderPath))
            {
                folderIcons.Remove(folderPath);
                SaveFolderIcons();
                RefreshProjectWindow();

                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log($"[NewFiles] Reset icon for '{folderPath}'");
                }
            }
        }

        /// <summary>
        /// Retrieves the custom icon path for a folder.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <returns>The asset path of the icon, or an empty string if none is set.</returns>
        public static string GetFolderIcon(string folderPath)
        {
            EnsureDataIsLoaded();
            return folderIcons.ContainsKey(folderPath) ? folderIcons[folderPath] : "";
        }

        /// <summary>
        /// Checks if a folder has a custom icon applied.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <returns>True if a custom icon is set, false otherwise.</returns>
        public static bool HasCustomIcon(string folderPath)
        {
            EnsureDataIsLoaded();
            return folderIcons.ContainsKey(folderPath) && !string.IsNullOrEmpty(folderIcons[folderPath]);
        }

        /// <summary>
        /// Gets a copy of the dictionary containing all folder-icon mappings.
        /// </summary>
        /// <returns>A new dictionary with all custom icon data.</returns>
        public static Dictionary<string, string> GetAllCustomIcons()
        {
            EnsureDataIsLoaded();
            return new Dictionary<string, string>(folderIcons);
        }
        #endregion

        #region General Management

        /// <summary>
        /// Clears all custom folder color settings.
        /// </summary>
        public static void ResetAllColors()
        {
            EnsureDataIsLoaded();
            folderColors.Clear();
            SaveFolderColors();
            RefreshProjectWindow();
        }

        /// <summary>
        /// Clears all custom folder icon settings.
        /// </summary>
        public static void ResetAllIcons()
        {
            EnsureDataIsLoaded();
            folderIcons.Clear();
            SaveFolderIcons();
            RefreshProjectWindow();
        }

        /// <summary>
        /// Resets all folder customizations (both colors and icons).
        /// </summary>
        public static void ResetAllCustomizations()
        {
            ResetAllColors();
            ResetAllIcons();
        }
        #endregion

        #region Persistence

        /// <summary>
        /// Loads folder color data from EditorPrefs by deserializing a JSON string.
        /// </summary>
        private static void LoadFolderColors()
        {
            folderColors.Clear();
            string serializedData = EditorPrefs.GetString("NewFiles.FolderColors", "");
            if (!string.IsNullOrEmpty(serializedData))
            {
                try
                {
                    var data = JsonUtility.FromJson<FolderColorData>(serializedData);
                    if (data != null && data.folders != null)
                    {
                        foreach (var folder in data.folders)
                        {
                            folderColors[folder.path] = new Color(folder.r, folder.g, folder.b, folder.a);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NewFiles] Error loading folder colors: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Saves the current folder color data to EditorPrefs by serializing it to a JSON string.
        /// </summary>
        private static void SaveFolderColors()
        {
            var folders = new FolderColorEntry[folderColors.Count];
            int index = 0;
            foreach (var kvp in folderColors)
            {
                folders[index++] = new FolderColorEntry
                {
                    path = kvp.Key,
                    r = kvp.Value.r,
                    g = kvp.Value.g,
                    b = kvp.Value.b,
                    a = kvp.Value.a
                };
            }
            var data = new FolderColorData { folders = folders };
            string serializedData = JsonUtility.ToJson(data);
            EditorPrefs.SetString("NewFiles.FolderColors", serializedData);
        }

        /// <summary>
        /// Loads folder icon data from EditorPrefs by deserializing a JSON string.
        /// </summary>
        private static void LoadFolderIcons()
        {
            folderIcons.Clear();
            string serializedData = EditorPrefs.GetString("NewFiles.FolderIcons", "");
            if (!string.IsNullOrEmpty(serializedData))
            {
                try
                {
                    var data = JsonUtility.FromJson<FolderIconData>(serializedData);
                    if (data != null && data.folders != null)
                    {
                        foreach (var folder in data.folders)
                        {
                            if (!string.IsNullOrEmpty(folder.iconPath))
                            {
                                folderIcons[folder.path] = folder.iconPath;
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[NewFiles] Error loading folder icons: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Saves the current folder icon data to EditorPrefs by serializing it to a JSON string.
        /// </summary>
        private static void SaveFolderIcons()
        {
            var folders = new FolderIconEntry[folderIcons.Count];
            int index = 0;
            foreach (var kvp in folderIcons)
            {
                folders[index++] = new FolderIconEntry
                {
                    path = kvp.Key,
                    iconPath = kvp.Value
                };
            }
            var data = new FolderIconData { folders = folders };
            string serializedData = JsonUtility.ToJson(data);
            EditorPrefs.SetString("NewFiles.FolderIcons", serializedData);
        }
        #endregion

        /// <summary>
        /// Forces the Unity Project window to repaint, making customization changes visible immediately.
        /// </summary>
        private static void RefreshProjectWindow()
        {
            EditorApplication.RepaintProjectWindow();
        }
        
        /// <summary>
        /// Gets the total number of folders with custom colors.
        /// </summary>
        /// <returns>The count of customized folders.</returns>
        public static int GetCustomizedFoldersCount()
        {
            EnsureDataIsLoaded();
            return folderColors.Count;
        }

        /// <summary>
        /// Gets the total number of folders with custom icons.
        /// </summary>
        /// <returns>The count of folders with custom icons.</returns>
        public static int GetCustomIconsCount()
        {
            EnsureDataIsLoaded();
            return folderIcons.Count;
        }
    }

    #region Data Structures

    /// <summary>
    /// Serializable container for an array of FolderColorEntry objects.
    /// Used for JSON serialization.
    /// </summary>
    [System.Serializable]
    public class FolderColorData
    {
        public FolderColorEntry[] folders;
    }

    /// <summary>
    /// Serializable class representing a single folder's color customization.
    /// </summary>
    [System.Serializable]
    public class FolderColorEntry
    {
        public string path;
        public float r, g, b, a;
    }

    /// <summary>
    /// Serializable container for an array of FolderIconEntry objects.
    /// Used for JSON serialization.
    /// </summary>
    [System.Serializable]
    public class FolderIconData
    {
        public FolderIconEntry[] folders;
    }

    /// <summary>
    /// Serializable class representing a single folder's icon customization.
    /// </summary>
    [System.Serializable]
    public class FolderIconEntry
    {
        public string path;
        public string iconPath;
    }
    #endregion
}
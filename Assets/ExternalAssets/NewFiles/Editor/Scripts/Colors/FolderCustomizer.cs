using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Manages the application and persistence of folder customizations.
    /// Includes a caching system to optimize performance.
    /// </summary>
    public static class FolderCustomizer
    {
        // Caches for storing folder settings and icons to avoid redundant lookups.
        private static Dictionary<string, FolderCustomization> customizationCache = new Dictionary<string, FolderCustomization>();
        private static Dictionary<string, Texture2D> iconCache = new Dictionary<string, Texture2D>();
        
        // Flag to ensure initialization only runs once.
        private static bool cacheInitialized = false;
        
        // In-memory backup system for user actions.
        private static List<FolderCustomizationBackup> backupHistory = new List<FolderCustomizationBackup>();
        private const int MAX_BACKUP_HISTORY = 10;

        /// <summary>
        /// Initializes the cache system when Unity loads.
        /// </summary>
        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            if (!cacheInitialized)
            {
                LoadCache();
                // Note: The direct drawing delegate is disabled here. Drawing is likely triggered by another manager script.
                // EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
                cacheInitialized = true;
            }
        }

        /// <summary>
        /// DEPRECATED: Callback to draw customizations on Project window items.
        /// This method is kept for potential future use but is not currently subscribed to any events.
        /// </summary>
        /// <param name="guid">The GUID of the asset being drawn.</param>
        /// <param name="selectionRect">The rect of the asset in the Project window.</param>
        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            // Abort if the main feature is disabled.
            if (!NewFilesSettings.Instance.isEnabled) return;
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(assetPath)) return;
            
            ApplyFolderCustomization(assetPath, selectionRect);
        }

        /// <summary>
        /// Applies the visual customization (color and icon) to a folder's GUI representation.
        /// </summary>
        /// <param name="folderPath">The path of the folder to customize.</param>
        /// <param name="rect">The drawing rectangle for the folder item.</param>
        private static void ApplyFolderCustomization(string folderPath, Rect rect)
        {
            FolderCustomization customization = GetCustomization(folderPath);
            if (customization == null) return;

            // Apply background color if specified.
            if (customization.hasColor)
            {
                Color backgroundColor = customization.color;
                backgroundColor.a = NewFilesSettings.Instance.colorOpacity;
                
                DrawFolderBackground(rect, backgroundColor);
            }

            // Apply custom icon if specified.
            if (customization.hasCustomIcon && !string.IsNullOrEmpty(customization.iconPath))
            {
                DrawCustomIcon(rect, customization.iconPath);
            }
        }
        
        /// <summary>
        /// Draws a colored background rectangle for the folder.
        /// Handles compatibility for different Unity versions.
        /// </summary>
        /// <param name="rect">The folder's GUI rectangle.</param>
        /// <param name="color">The background color to draw.</param>
        private static void DrawFolderBackground(Rect rect, Color color)
        {
            // Create a slightly smaller rect for padding.
            Rect backgroundRect = new Rect(rect.x + 1, rect.y + 1, rect.width - 2, rect.height - 2);
            
            #if UNITY_2022_3_OR_NEWER
            EditorGUI.DrawRect(backgroundRect, color);
            #elif UNITY_2020_3_OR_NEWER
            // Fallback for older Unity versions that lack EditorGUI.DrawRect.
            Color previousColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(backgroundRect, EditorGUIUtility.whiteTexture);
            GUI.color = previousColor;
            #endif
        }

        /// <summary>
        /// Draws a custom icon on top of the folder item.
        /// </summary>
        /// <param name="rect">The folder's GUI rectangle.</param>
        /// <param name="iconPath">The asset path to the icon texture.</param>
        private static void DrawCustomIcon(Rect rect, string iconPath)
        {
            Texture2D icon = GetCachedIcon(iconPath);
            if (icon != null)
            {
                // Position the icon at the top-right corner.
                Rect iconRect = new Rect(rect.x + rect.width - 20, rect.y + 2, 16, 16);
                GUI.DrawTexture(iconRect, icon);
            }
        }

        /// <summary>
        /// Retrieves an icon texture from the cache or loads it from the AssetDatabase if not cached.
        /// </summary>
        /// <param name="iconPath">The asset path of the icon.</param>
        /// <returns>The Texture2D for the icon, or null if not found.</returns>
        private static Texture2D GetCachedIcon(string iconPath)
        {
            if (iconCache.ContainsKey(iconPath))
            {
                return iconCache[iconPath];
            }

            Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (icon != null)
            {
                iconCache[iconPath] = icon;
                // If the cache exceeds its max size, clean it up.
                if (iconCache.Count > NewFilesSettings.Instance.maxCachedFolders)
                {
                    CleanupIconCache();
                }
            }

            return icon;
        }

        /// <summary>
        /// Cleans the icon cache by removing the oldest half of the entries to save memory.
        /// </summary>
        private static void CleanupIconCache()
        {
            var keysToRemove = iconCache.Keys.Take(iconCache.Count / 2).ToList();
            foreach (var key in keysToRemove)
            {
                iconCache.Remove(key);
            }
        }

        /// <summary>
        /// Gets the customization data for a specific folder path.
        /// It first checks the cache and then queries the core system if not found.
        /// </summary>
        /// <param name="folderPath">The path of the folder.</param>
        /// <returns>The FolderCustomization object, or null if no customization exists.</returns>
        public static FolderCustomization GetCustomization(string folderPath)
        {
            if (NewFilesSettings.Instance.enableCache && customizationCache.ContainsKey(folderPath))
            {
                return customizationCache[folderPath];
            }
            
            Color folderColor = NewFilesCore.GetFolderColor(folderPath);
            if (folderColor != Color.clear)
            {
                var customization = new FolderCustomization
                {
                    folderPath = folderPath,
                    color = folderColor,
                    hasColor = true,
                    hasCustomIcon = false, // Icon logic is handled separately for now.
                    lastModified = System.DateTime.Now
                };

                if (NewFilesSettings.Instance.enableCache)
                {
                    UpdateCache(folderPath, customization);
                }

                return customization;
            }

            return null;
        }

        /// <summary>
        /// Sets or updates the customization for a folder.
        /// This creates a backup before applying the changes.
        /// </summary>
        /// <param name="folderPath">The path of the folder to customize.</param>
        /// <param name="customization">The customization data to apply.</param>
        public static void SetCustomization(string folderPath, FolderCustomization customization)
        {
            CreateBackup();

            if (customization.hasColor)
            {
                NewFilesCore.SetFolderColor(folderPath, customization.color);
            }
            else
            {
                NewFilesCore.ResetFolderColor(folderPath);
            }

            UpdateCache(folderPath, customization);
        }

        /// <summary>
        /// Removes all customizations from a folder.
        /// </summary>
        /// <param name="folderPath">The path of the folder to reset.</param>
        public static void RemoveCustomization(string folderPath)
        {
            CreateBackup();
            NewFilesCore.ResetFolderColor(folderPath);
            if (customizationCache.ContainsKey(folderPath))
            {
                customizationCache.Remove(folderPath);
            }
        }

        /// <summary>
        /// Adds or updates a customization entry in the cache.
        /// </summary>
        /// <param name="folderPath">The folder's path (cache key).</param>
        /// <param name="customization">The customization data (cache value).</param>
        private static void UpdateCache(string folderPath, FolderCustomization customization)
        {
            if (!NewFilesSettings.Instance.enableCache) return;
            
            customizationCache[folderPath] = customization;
            // If cache exceeds its limit, perform a cleanup.
            if (customizationCache.Count > NewFilesSettings.Instance.maxCachedFolders)
            {
                CleanupCustomizationCache();
            }
        }

        /// <summary>
        /// Cleans the customization cache by removing the least recently used half of the entries.
        /// </summary>
        private static void CleanupCustomizationCache()
        {
            var oldestEntries = customizationCache.Values
                .OrderBy(c => c.lastModified)
                .Take(customizationCache.Count / 2)
                .Select(c => c.folderPath)
                .ToList();
            foreach (var path in oldestEntries)
            {
                customizationCache.Remove(path);
            }
        }

        /// <summary>
        /// Loads the cache from a persistent source (if implemented). Currently just clears it.
        /// </summary>
        private static void LoadCache()
        {
            if (!NewFilesSettings.Instance.enableCache) return;
            customizationCache.Clear();
        }

        /// <summary>
        /// Placeholder for saving the cache to a persistent source. Not currently implemented.
        /// </summary>
        private static void SaveCache() { }

        /// <summary>
        /// Clears all cached customizations and icons.
        /// </summary>
        public static void ClearCache()
        {
            customizationCache.Clear();
            iconCache.Clear();
        }

        /// <summary>
        /// Creates a backup of the current customization state if auto-backup is enabled.
        /// </summary>
        private static void CreateBackup()
        {
            if (!NewFilesSettings.Instance.autoBackup) return;
            
            var backup = new FolderCustomizationBackup
            {
                timestamp = System.DateTime.Now,
                customizations = new Dictionary<string, FolderCustomization>(customizationCache)
            };
            
            backupHistory.Add(backup);
            // Ensure the backup history does not exceed the maximum size.
            if (backupHistory.Count > MAX_BACKUP_HISTORY)
            {
                backupHistory.RemoveAt(0);
            }
        }

        /// <summary>
        /// Restores the folder customizations from a selected backup.
        /// </summary>
        /// <param name="backupIndex">The index of the backup in the history.</param>
        /// <returns>True if the restoration was successful, false otherwise.</returns>
        public static bool RestoreFromBackup(int backupIndex)
        {
            if (backupIndex < 0 || backupIndex >= backupHistory.Count)
                return false;

            var backup = backupHistory[backupIndex];
            NewFilesCore.ResetAllColors(); // Clear current state before restoring.
            
            // Apply the backed-up customizations.
            foreach (var kvp in backup.customizations)
            {
                if (kvp.Value.hasColor)
                {
                    NewFilesCore.SetFolderColor(kvp.Key, kvp.Value.color);
                }
            }
            
            // Restore the cache state.
            customizationCache = new Dictionary<string, FolderCustomization>(backup.customizations);
            return true;
        }

        /// <summary>
        /// Retrieves the timestamps of all available backups.
        /// </summary>
        /// <returns>An array of DateTime objects representing the backup points.</returns>
        public static System.DateTime[] GetAvailableBackups()
        {
            return backupHistory.Select(b => b.timestamp).ToArray();
        }

        /// <summary>
        /// Gathers statistics about the current state of the cache.
        /// </summary>
        /// <returns>A CacheStatistics struct with performance metrics.</returns>
        public static CacheStatistics GetCacheStatistics()
        {
            return new CacheStatistics
            {
                customizationCacheSize = customizationCache.Count,
                iconCacheSize = iconCache.Count,
                maxCacheSize = NewFilesSettings.Instance.maxCachedFolders,
                cacheEfficiency = CalculateCacheEfficiency()
            };
        }

        /// <summary>
        /// Calculates the current cache efficiency as a percentage of its maximum capacity.
        /// </summary>
        /// <returns>A float representing the cache usage percentage.</returns>
        private static float CalculateCacheEfficiency()
        {
            if (customizationCache.Count == 0 || NewFilesSettings.Instance.maxCachedFolders == 0) return 0f;
            // Calculate usage percentage, capped at 100%.
            return Mathf.Min(100f, (customizationCache.Count / (float)NewFilesSettings.Instance.maxCachedFolders) * 100f);
        }
    }

    /// <summary>
    /// Data container for a folder's visual customization settings.
    /// </summary>
    [System.Serializable]
    public class FolderCustomization
    {
        /// <summary>The project-relative path of the folder.</summary>
        public string folderPath;
        /// <summary>The background color applied to the folder.</summary>
        public Color color = Color.white;
        /// <summary>Whether a custom color is applied.</summary>
        public bool hasColor = false;
        /// <summary>The asset path to a custom icon texture.</summary>
        public string iconPath = "";
        /// <summary>Whether a custom icon is applied.</summary>
        public bool hasCustomIcon = false;
        /// <summary>Timestamp of the last modification, used for cache management.</summary>
        public System.DateTime lastModified = System.DateTime.Now;
        
        public FolderCustomization() { }
        
        public FolderCustomization(string path, Color folderColor)
        {
            folderPath = path;
            color = folderColor;
            hasColor = true;
            lastModified = System.DateTime.Now;
        }
    }

    /// <summary>
    /// Represents a snapshot of all folder customizations at a specific point in time.
    /// </summary>
    [System.Serializable]
    public class FolderCustomizationBackup
    {
        /// <summary>The date and time when the backup was created.</summary>
        public System.DateTime timestamp;
        /// <summary>A dictionary containing all folder customizations at the time of backup.</summary>
        public Dictionary<string, FolderCustomization> customizations;
    }

    /// <summary>
    /// A struct holding performance and usage metrics for the caching system.
    /// </summary>
    public struct CacheStatistics
    {
        /// <summary>The number of items currently in the customization cache.</summary>
        public int customizationCacheSize;
        /// <summary>The number of icons currently in the icon cache.</summary>
        public int iconCacheSize;
        /// <summary>The maximum configured size for the caches.</summary>
        public int maxCacheSize;
        /// <summary>The cache usage expressed as a percentage.</summary>
        public float cacheEfficiency;
    }
}
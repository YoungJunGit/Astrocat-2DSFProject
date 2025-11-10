using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Handles the custom rendering of folder items in Unity's Project Window.
    /// This class provides performance-optimized drawing overrides, including support
    /// for custom folder colors, icons, and a stable zebra striping effect.
    /// </summary>
    [InitializeOnLoad]
    public static class NewFilesDrawer
    {
        // Caching dictionaries to store rendered data and loaded textures for performance.
        private static Dictionary<string, CachedRenderData> renderCache = new Dictionary<string, CachedRenderData>();
        private static Dictionary<string, Texture2D> iconTextureCache = new Dictionary<string, Texture2D>();

        // Timer and interval for periodic cache cleanup.
        private static float lastCacheCleanupTime = 0f;
        private const float CACHE_CLEANUP_INTERVAL = 30f;

        // Cached texture for the default folder icon.
        private static Texture2D folderIconTexture;

        // Flag to ensure styles are initialized only once.
        private static bool stylesInitialized = false;

        // State variable to detect if the current drawing context is the list view in the Project Window.
        private static bool isInListView = false;

        /// <summary>
        /// Static constructor called automatically by Unity due to the [InitializeOnLoad] attribute.
        /// </summary>
        static NewFilesDrawer()
        {
            Initialize();
        }

        /// <summary>
        /// Subscribes to necessary Editor events for custom drawing and updates.
        /// </summary>
        private static void Initialize()
        {
            // Note: The 'projectWindowItemOnGUI' subscription is managed by NewFilesEventDispatcher.
            EditorApplication.update -= PerformanceUpdate;
            EditorApplication.update += PerformanceUpdate;
            
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log("[NewFiles] Drawer initialized successfully with stable zebra striping support");
            }
        }
        
        /// <summary>
        /// A periodic update method called by EditorApplication.update.
        /// Used to trigger cache cleanup at regular intervals to manage memory usage.
        /// </summary>
        private static void PerformanceUpdate()
        {
            if (Time.realtimeSinceStartup - lastCacheCleanupTime > CACHE_CLEANUP_INTERVAL)
            {
                CleanupRenderCache();
                CleanupIconCache();
                lastCacheCleanupTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// The main GUI drawing method for each item in the Project Window.
        /// </summary>
        /// <param name="guid">The asset GUID of the item being drawn.</param>
        /// <param name="selectionRect">The rect (position and size) of the item's GUI element.</param>
        public static void DrawCustomFolderGUI(string guid, Rect selectionRect)
        {
            // Exit if the feature is disabled in settings.
            if (!NewFilesSettings.Instance.isEnabled) return;
            
            // Perform validation checks to avoid unnecessary processing.
            if (string.IsNullOrEmpty(guid)) return;
            if (selectionRect.width <= 0 || selectionRect.height <= 0) return;
            if (Event.current.type != EventType.Repaint) return; // Only execute drawing logic during the Repaint event.
            
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath)) return;
            
            // Determine if the item is in the one-column list view or the two-column grid view.
            DetectProjectWindowZone(selectionRect);
            
            // Draw zebra striping if enabled, but only for the list view for a cleaner look.
            if (NewFilesSettings.Instance.enableZebraStriping && isInListView)
            {
                bool isFolder = AssetDatabase.IsValidFolder(assetPath);
                // Apply to all items or only folders based on settings.
                if (!NewFilesSettings.Instance.zebraOnlyFolders || isFolder)
                {
                    DrawZebraStriping(selectionRect);
                }
            }
            
            // The following logic applies only to folders.
            if (!AssetDatabase.IsValidFolder(assetPath)) return;
            
            // If the folder has a custom color or icon, proceed with enhanced drawing.
            if (NewFilesCore.HasCustomColor(assetPath) || NewFilesCore.HasCustomIcon(assetPath))
            {
                DrawEnhancedFolder(assetPath, selectionRect);
            }
        }
        
        /// <summary>
        /// Heuristically detects if the current item is in the Project Window's list view (left panel)
        /// based on the item's rectangle geometry.
        /// </summary>
        /// <param name="rect">The item's drawing rectangle.</param>
        private static void DetectProjectWindowZone(Rect rect)
        {
            // List view items are typically short and wide.
            bool isListViewRect = rect.height <= 20f && rect.width > 100f;
            
            // They also appear on the left side of the window.
            bool isLeftSide = rect.x < 300f;
            
            isInListView = isListViewRect && isLeftSide;
        }
        
        /// <summary>
        /// Draws the zebra-striped background for list view items.
        /// This implementation is stable and does not flicker or change with scrolling.
        /// </summary>
        /// <param name="rect">The item's drawing rectangle.</param>
        private static void DrawZebraStriping(Rect rect)
        {
            // Prevent division by zero if the rect has an invalid height.
            if (rect.height < 1f) return;

            // Calculate a stable index based on the item's absolute vertical position.
            // This ensures the odd/even pattern remains consistent regardless of the view's scroll position.
            int stableIndex = Mathf.FloorToInt(rect.y / rect.height);
            
            // Determine if the line is even or odd.
            bool isEvenLine = (stableIndex % 2) == 0;
            
            // Select the appropriate color from settings.
            Color zebraColor = isEvenLine 
                ? NewFilesSettings.Instance.zebraLightColor 
                : NewFilesSettings.Instance.zebraDarkColor;
            
            // Apply the global opacity setting.
            zebraColor.a *= NewFilesSettings.Instance.zebraStripingOpacity;
            
            // Draw the background rectangle if it's visible.
            if (zebraColor.a > 0.001f)
            {
                EditorGUI.DrawRect(rect, zebraColor);
            }
        }

        /// <summary>
        /// Orchestrates the drawing of a folder with custom colors and/or icons.
        /// It uses a caching system to optimize performance.
        /// </summary>
        /// <param name="folderPath">The asset path of the folder.</param>
        /// <param name="rect">The folder's drawing rectangle.</param>
        private static void DrawEnhancedFolder(string folderPath, Rect rect)
        {
            InitializeStyles();
            
            Color folderColor = NewFilesCore.GetFolderColor(folderPath);
            string iconPath = NewFilesCore.GetFolderIcon(folderPath);

            // Attempt to retrieve valid cached data for the folder.
            CachedRenderData cachedData = GetCachedRenderData(folderPath, folderColor, iconPath, rect);
            if (cachedData != null && cachedData.IsValid(rect, folderColor, iconPath))
            {
                DrawFolderWithCustomizations(cachedData, rect);
                return;
            }

            // If no valid cache exists, render the folder and cache the new data.
            var newCachedData = RenderAndCacheFolder(folderPath, folderColor, iconPath, rect);
            DrawFolderWithCustomizations(newCachedData, rect);
        }

        /// <summary>
        /// Initializes required GUI styles and resources, such as the folder icon texture.
        /// This is done lazily to avoid performance hits on editor startup.
        /// </summary>
        private static void InitializeStyles()
        {
            if (stylesInitialized) return;
            folderIconTexture = EditorGUIUtility.IconContent("Folder Icon").image as Texture2D;
            stylesInitialized = true;
        }

        /// <summary>
        /// Retrieves cached rendering data for a folder if caching is enabled and the data is valid.
        /// </summary>
        private static CachedRenderData GetCachedRenderData(string folderPath, Color color, string iconPath, Rect rect)
        {
            if (!NewFilesSettings.Instance.enableCache) return null;
            if (renderCache.TryGetValue(folderPath, out var cached) && cached.IsValid(rect, color, iconPath))
            {
                // Update access time for cache cleanup logic.
                cached.lastAccessed = Time.realtimeSinceStartup;
                return cached;
            }
            return null;
        }

        /// <summary>
        /// Creates and stores new cached data for a folder's rendered appearance.
        /// </summary>
        private static CachedRenderData RenderAndCacheFolder(string folderPath, Color color, string iconPath, Rect rect)
        {
            var cachedData = new CachedRenderData
            {
                folderPath = folderPath,
                color = color,
                iconPath = iconPath,
                rect = rect,
                lastAccessed = Time.realtimeSinceStartup,
                creationTime = Time.realtimeSinceStartup
            };

            if (NewFilesSettings.Instance.enableCache)
            {
                renderCache[folderPath] = cachedData;
                // If the cache exceeds its max size, trigger a cleanup.
                if (renderCache.Count > NewFilesSettings.Instance.maxCachedFolders)
                {
                    CleanupRenderCache();
                }
            }
            return cachedData;
        }

        /// <summary>
        /// Executes the actual drawing calls for the folder's background color and custom icon.
        /// </summary>
        private static void DrawFolderWithCustomizations(CachedRenderData cachedData, Rect rect)
        {
            // First, draw the colored background icon if a color is assigned.
            if (cachedData.color != Color.clear)
            {
                DrawColoredFolderIcon(cachedData, rect);
            }
            
            // Second, draw the custom overlay icon if one is assigned.
            if (!string.IsNullOrEmpty(cachedData.iconPath))
            {
                DrawCustomIcon(cachedData.iconPath, rect);
            }
        }

        /// <summary>
        /// Draws the tinted default folder icon.
        /// </summary>
        private static void DrawColoredFolderIcon(CachedRenderData cachedData, Rect rect)
        {
            if (folderIconTexture == null) return;

            Rect iconRect = GetAdjustedRectForIcon(rect);
            Color targetColor = cachedData.color;
            
            // Apply a slight intensity boost to the color to make it more vibrant and visible.
            float intensityCorrection = 1.35f;
            Color finalColor = new Color(
                targetColor.r * intensityCorrection,
                targetColor.g * intensityCorrection,
                targetColor.b * intensityCorrection,
                1.0f
            );
            
            // Tint and draw the folder icon texture.
            Color originalGuiColor = GUI.color;
            GUI.color = finalColor;
            GUI.DrawTexture(iconRect, folderIconTexture);
            GUI.color = originalGuiColor;
        }

        /// <summary>
        /// Draws the user-defined custom icon over the folder.
        /// </summary>
        private static void DrawCustomIcon(string iconPath, Rect rect)
        {
            Texture2D customIcon = GetCachedIconTexture(iconPath);
            if (customIcon == null) return;

            Rect iconRect = GetCustomIconRect(rect);
            
            // Draw the icon texture without any tint.
            Color originalColor = GUI.color;
            GUI.color = Color.white;
            GUI.DrawTexture(iconRect, customIcon, ScaleMode.ScaleToFit);
            GUI.color = originalColor;
        }

        /// <summary>
        /// Retrieves a cached Texture2D for a given icon path. If not cached, it loads it from the AssetDatabase.
        /// </summary>
        private static Texture2D GetCachedIconTexture(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath)) return null;
            
            // Return cached texture if available.
            if (iconTextureCache.TryGetValue(iconPath, out Texture2D cached))
            {
                return cached;
            }

            // Load the texture from the asset path and add it to the cache.
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
            if (texture != null)
            {
                iconTextureCache[iconPath] = texture;
                
                // Trigger cleanup if cache size exceeds the limit.
                if (iconTextureCache.Count > NewFilesSettings.Instance.maxCachedFolders)
                {
                    CleanupIconCache();
                }
            }

            return texture;
        }

        /// <summary>
        /// Calculates the correct position and size for the main folder icon,
        /// adapting for both list and grid view layouts.
        /// </summary>
        private static Rect GetAdjustedRectForIcon(Rect originalRect)
        {
            // For list view (one-column layout).
            if (originalRect.height <= 20f && originalRect.width > 20f)
            {
                float yOffset = (originalRect.height - 16f) / 2f;
                return new Rect(originalRect.x + 1, originalRect.y + yOffset, 16f, 16f);
            }
            // For grid view (two-column layout).
            else
            {
                float iconSize = Mathf.Min(originalRect.width, originalRect.height - 16f);
                return new Rect(originalRect.x + (originalRect.width - iconSize) / 2f, originalRect.y, iconSize, iconSize);
            }
        }

        /// <summary>
        /// Calculates the correct position and size for the custom overlay icon,
        /// adapting for both list and grid view layouts.
        /// </summary>
        private static Rect GetCustomIconRect(Rect originalRect)
        {
            // For list view (one-column layout).
            if (originalRect.height <= 20f && originalRect.width > 20f)
            {
                float iconSize = 8f;
                return new Rect(
                    originalRect.x + 4f,
                    originalRect.y + (originalRect.height - iconSize) / 2f,
                    iconSize,
                    iconSize
                );
            }
            // For grid view (two-column layout).
            else
            {
                float folderIconSize = Mathf.Min(originalRect.width, originalRect.height - 16f);
                float customIconSize = folderIconSize * 0.4f;
                
                return new Rect(
                    originalRect.x + (originalRect.width - customIconSize) / 2f,
                    originalRect.y + (folderIconSize - customIconSize) / 2f + folderIconSize * 0.1f,
                    customIconSize,
                    customIconSize
                );
            }
        }

        /// <summary>
        /// Cleans up the render cache by removing old or excess entries.
        /// It removes entries that haven't been accessed in a while, and if still over capacity,
        /// removes the least recently used entries.
        /// </summary>
        private static void CleanupRenderCache()
        {
            if (!NewFilesSettings.Instance.enableCache || renderCache.Count == 0) return;
            
            var keysToRemove = new List<string>();
            float currentTime = Time.realtimeSinceStartup;
            int maxCacheSize = NewFilesSettings.Instance.maxCachedFolders;

            // Mark entries that are older than 60 seconds for removal.
            foreach (var kvp in renderCache)
            {
                if (currentTime - kvp.Value.lastAccessed > 60f)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
            
            // If the cache is still over the size limit, remove the least recently used entries.
            if (renderCache.Count - keysToRemove.Count > maxCacheSize)
            {
                var sortedEntries = new List<KeyValuePair<string, CachedRenderData>>();
                foreach(var kvp in renderCache)
                {
                    if (!keysToRemove.Contains(kvp.Key)) sortedEntries.Add(kvp);
                }
                sortedEntries.Sort((a, b) => a.Value.lastAccessed.CompareTo(b.Value.lastAccessed));
                int excessCount = renderCache.Count - keysToRemove.Count - maxCacheSize;
                for (int i = 0; i < excessCount; i++)
                {
                    keysToRemove.Add(sortedEntries[i].Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                renderCache.Remove(key);
            }
            
            if (keysToRemove.Count > 0 && NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log($"[NewFiles] Cleaned up {keysToRemove.Count} cached render entries");
            }
        }

        /// <summary>
        /// Cleans up the icon texture cache if it exceeds the maximum size limit.
        /// Removes the oldest half of the entries.
        /// </summary>
        private static void CleanupIconCache()
        {
            if (iconTextureCache.Count <= NewFilesSettings.Instance.maxCachedFolders) return;
            
            var keysToRemove = iconTextureCache.Keys.Take(iconTextureCache.Count / 2).ToList();
            foreach (var key in keysToRemove)
            {
                iconTextureCache.Remove(key);
            }
            
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log($"[NewFiles] Cleaned up {keysToRemove.Count} cached icon textures");
            }
        }

        /// <summary>
        /// Public method to manually clear all caches.
        /// This can be called from settings or other editor windows.
        /// </summary>
        public static void ClearAllCaches()
        {
            renderCache.Clear();
            iconTextureCache.Clear();
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log("[NewFiles] All render and icon caches cleared");
            }
        }
    }
    
    /// <summary>
    /// A data structure to hold cached information about a folder's custom appearance.
    /// This prevents recalculating and redrawing on every GUI frame.
    /// </summary>
    public class CachedRenderData
    {
        public string folderPath;
        public Color color;
        public string iconPath;
        public Rect rect;
        public float lastAccessed;
        public float creationTime;

        /// <summary>
        /// DEPRECATED: Use the overload with iconPath.
        /// Checks if the cached data is still valid by comparing rect dimensions and color.
        /// </summary>
        public bool IsValid(Rect currentRect, Color currentColor)
        {
            const float EPSILON = 0.001f;
            bool rectSame = Mathf.Abs(rect.width - currentRect.width) < EPSILON &&
                            Mathf.Abs(rect.height - currentRect.height) < EPSILON;
            bool colorSame = Mathf.Abs(color.r - currentColor.r) < EPSILON &&
                             Mathf.Abs(color.g - currentColor.g) < EPSILON &&
                             Mathf.Abs(color.b - currentColor.b) < EPSILON;
            return rectSame && colorSame;
        }

        /// <summary>
        /// Checks if the cached data is still valid for the current drawing context.
        /// The cache is considered invalid if the item's rect, color, or icon path has changed.
        /// </summary>
        /// <param name="currentRect">The current drawing rectangle of the folder.</param>
        /// <param name="currentColor">The current color setting for the folder.</param>
        /// <param name="currentIconPath">The current icon path setting for the folder.</param>
        /// <returns>True if the cached data is still valid, false otherwise.</returns>
        public bool IsValid(Rect currentRect, Color currentColor, string currentIconPath)
        {
            const float EPSILON = 0.001f;
            // Check if the item's size has changed (e.g., project zoom level changed).
            bool rectSame = Mathf.Abs(rect.width - currentRect.width) < EPSILON &&
                            Mathf.Abs(rect.height - currentRect.height) < EPSILON;
            // Check if the folder's color has been modified.
            bool colorSame = Mathf.Abs(color.r - currentColor.r) < EPSILON &&
                             Mathf.Abs(color.g - currentColor.g) < EPSILON &&
                             Mathf.Abs(color.b - currentColor.b) < EPSILON;
            // Check if the folder's icon has been changed or removed.
            bool iconSame = iconPath == currentIconPath;
            return rectSame && colorSame && iconSame;
        }
    }
}
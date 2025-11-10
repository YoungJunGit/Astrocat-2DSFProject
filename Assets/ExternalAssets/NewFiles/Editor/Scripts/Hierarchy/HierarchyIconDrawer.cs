using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using NewFiles.Runtime;

namespace NewFiles.Editor.Hierarchy
{
    /// <summary>
    /// Handles drawing custom icons, background colors, and zebra striping in the Unity Hierarchy window.
    /// It subscribes to the editor's hierarchy drawing event to apply custom visuals.
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyDrawer
    {
        /// <summary>
        /// Caches loaded icon textures to avoid redundant disk access and improve performance.
        /// The key is the asset path of the texture.
        /// </summary>
        private static Dictionary<string, Texture2D> iconTextureCache = new Dictionary<string, Texture2D>();
        
        /// <summary>
        /// Static constructor called when the editor loads, thanks to the [InitializeOnLoad] attribute.
        /// It subscribes the drawing method to the hierarchy GUI event.
        /// </summary>
        static HierarchyDrawer()
        {
            // Ensure we only have one subscription to the event at all times.
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;

            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log("[NewFiles] Hierarchy Drawer initialized for backgrounds, zebra striping, and icons.");
            }
        }

        /// <summary>
        /// This method is called for each visible item being drawn in the Hierarchy window.
        /// </summary>
        /// <param name="instanceID">The instance ID of the object being drawn.</param>
        /// <param name="selectionRect">The rectangle area of the hierarchy item.</param>
        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            var settings = NewFilesSettings.Instance;
            
            // Early exit if the feature is disabled or if it's not a repaint event, for optimization.
            if (!settings.isEnabled || Event.current.type != EventType.Repaint) return;
            
            // The drawing order is crucial for correct visual layering.
            // 1. Zebra Striping is drawn first to act as the bottom-most background layer.
            if (settings.enableZebraStriping)
            {
                DrawZebraStriping(selectionRect);
            }

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;
            
            var iconComponent = go.GetComponent<HierarchyIcon>();
            if (iconComponent == null) return;

            // 2. The custom background color is drawn on top of the zebra striping.
            // It has transparency to allow the striping to show through if desired.
            if (iconComponent.hasCustomColor)
            {
                DrawBackgroundColor(selectionRect, iconComponent.backgroundColor);
            }
            
            // 3. Finally, the custom icon is drawn on the top layer.
            if (!string.IsNullOrEmpty(iconComponent.iconPath))
            {
                DrawCustomIcon(selectionRect, iconComponent.iconPath);
            }
        }

        /// <summary>
        /// Draws alternating background colors for rows to improve readability.
        /// </summary>
        /// <param name="rect">The rectangle area of the hierarchy item.</param>
        private static void DrawZebraStriping(Rect rect)
        {
            if (rect.height < 1f) return;

            // Calculate a stable row index based on the item's vertical position.
            int stableIndex = Mathf.FloorToInt(rect.y / rect.height);
            bool isEvenLine = (stableIndex % 2) == 0;
            
            // Select the appropriate color from settings based on whether the row is even or odd.
            Color zebraColor = isEvenLine 
                ? NewFilesSettings.Instance.zebraLightColor 
                : NewFilesSettings.Instance.zebraDarkColor;
            
            // Apply the global opacity setting for zebra striping.
            zebraColor.a *= NewFilesSettings.Instance.zebraStripingOpacity;
            
            if (zebraColor.a > 0.001f)
            {
                // Extend the rectangle horizontally to cover the entire hierarchy row.
                Rect zebraRect = new Rect(rect.x - 16f, rect.y, rect.width + 32f, rect.height);
                EditorGUI.DrawRect(zebraRect, zebraColor);
            }
        }

        /// <summary>
        /// Draws a solid custom background color for a specific hierarchy item.
        /// </summary>
        /// <param name="rect">The rectangle area of the hierarchy item.</param>
        /// <param name="color">The color to draw.</param>
        private static void DrawBackgroundColor(Rect rect, Color color)
        {
            // Apply the specific opacity setting for custom hierarchy backgrounds.
            color.a = NewFilesSettings.Instance.hierarchyColorOpacity;
            
            if (color.a > 0.001f)
            {
                // Extend the rectangle horizontally to cover the entire hierarchy row.
                Rect backgroundRect = new Rect(rect.x - 16f, rect.y, rect.width + 32f, rect.height);
                EditorGUI.DrawRect(backgroundRect, color);
            }
        }

        /// <summary>
        /// Draws a custom icon for the GameObject, replacing the default one.
        /// </summary>
        /// <param name="selectionRect">The rectangle area of the hierarchy item.</param>
        /// <param name="iconPath">The asset path to the icon texture.</param>
        private static void DrawCustomIcon(Rect selectionRect, string iconPath)
        {
            Texture2D customIconTexture = GetCachedIconTexture(iconPath);
            if (customIconTexture == null) return;

            // Define the 16x16 pixel area where the icon will be drawn.
            Rect iconRect = new Rect(
                selectionRect.x,
                selectionRect.y + (selectionRect.height - 16f) / 2f,
                16f, 
                16f
            );

            // A background rect is drawn first to cover Unity's default GameObject icon (e.g., the cube).
            // Its color matches the editor's theme (light or dark pro skin).
            Color backgroundColor = EditorGUIUtility.isProSkin
                ? new Color(0.22f, 0.22f, 0.22f, 1f) // Dark theme
                : new Color(0.78f, 0.78f, 0.78f, 1f); // Light theme
            
            EditorGUI.DrawRect(iconRect, backgroundColor);

            // Draw the custom icon texture on top of the background rect.
            GUI.DrawTexture(iconRect, customIconTexture, ScaleMode.ScaleToFit);
        }

        /// <summary>
        /// Retrieves a Texture2D from the given asset path, using a cache to avoid redundant disk access.
        /// If the texture is not in the cache, it loads it from the AssetDatabase and adds it.
        /// </summary>
        /// <param name="path">The asset path of the texture (e.g., "Assets/Icons/my_icon.png").</param>
        /// <returns>The loaded Texture2D, or null if the path is invalid or the asset cannot be loaded.</returns>
        private static Texture2D GetCachedIconTexture(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            // Attempt to retrieve the texture from the cache first.
            if (iconTextureCache.TryGetValue(path, out Texture2D cachedTexture))
            {
                // If the cached texture is not null, it's valid and we can return it.
                if(cachedTexture != null) return cachedTexture;
                
                // If the cached texture has become null (e.g., asset was deleted), remove the invalid entry.
                iconTextureCache.Remove(path);
            }

            // If not found in cache, load it from the AssetDatabase.
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture != null)
            {
                // Add the newly loaded texture to the cache for future use.
                iconTextureCache[path] = texture;
            }
            return texture;
        }
    }
}
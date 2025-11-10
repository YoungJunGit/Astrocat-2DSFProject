using UnityEngine;
using UnityEditor;

namespace NewFiles.Editor
{
    /// <summary>
    /// Handles the drawing of alternating row colors (zebra striping) in the Hierarchy Window.
    /// This implementation provides a stable background that does not flicker or change when scrolling.
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyZebraStriping
    {
        /// <summary>
        /// Static constructor called when the editor loads, ensuring the feature is initialized.
        /// </summary>
        static HierarchyZebraStriping()
        {
            Initialize();
        }
        
        /// <summary>
        /// Subscribes the drawing method to the hierarchy window item GUI event.
        /// It first unsubscribes to prevent duplicate subscriptions during script reloads.
        /// </summary>
        private static void Initialize()
        {
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log("[NewFiles] Hierarchy zebra striping initialized with stable scrolling fix");
            }
        }
        
        /// <summary>
        /// Callback executed for each item drawn in the Hierarchy window.
        /// </summary>
        /// <param name="instanceID">The instance ID of the hierarchy item being drawn.</param>
        /// <param name="selectionRect">The rectangle area of the hierarchy item.</param>
        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            // Only proceed if the feature is enabled in settings and the current event is a Repaint event.
            // This ensures the drawing logic only runs when necessary, improving performance.
            if (!NewFilesSettings.Instance.isEnabled || !NewFilesSettings.Instance.enableZebraStriping) return;
            if (Event.current.type != EventType.Repaint) return;
            
            DrawHierarchyZebraStriping(selectionRect);
        }
        
        /// <summary>
        /// Draws the colored background for a single hierarchy row.
        /// </summary>
        /// <param name="rect">The rectangle area of the hierarchy item to draw on.</param>
        private static void DrawHierarchyZebraStriping(Rect rect)
        {
            // Prevent division by zero if the item's rect has an invalid height.
            if (rect.height < 1f) return;

            // Calculate a stable index based on the item's vertical position in the window.
            // This is crucial for preventing the background color from changing during scrolling,
            // as it remains consistent regardless of which items are currently visible.
            int stableIndex = Mathf.FloorToInt(rect.y / rect.height);
            
            // Determine if the row is even or odd based on the stable index.
            bool isEvenLine = (stableIndex % 2) == 0;
            
            // Select the appropriate color from the settings.
            Color zebraColor = isEvenLine 
                ? NewFilesSettings.Instance.zebraLightColor 
                : NewFilesSettings.Instance.zebraDarkColor;
            
            // Apply the global opacity setting.
            zebraColor.a *= NewFilesSettings.Instance.zebraStripingOpacity;
            
            // Draw the background only if the color is visible (alpha is greater than a small threshold).
            if (zebraColor.a > 0.001f)
            {
                // Expand the rectangle slightly to ensure it covers the full width of the row,
                // including the space for the foldout arrow on the left.
                Rect zebraRect = new Rect(
                    rect.x - 16f, 
                    rect.y, 
                    rect.width + 32f,
                    rect.height
                );
                
                EditorGUI.DrawRect(zebraRect, zebraColor);
            }
        }
    }
}
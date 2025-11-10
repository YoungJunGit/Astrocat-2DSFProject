using UnityEngine;
using UnityEditor;

namespace NewFiles.Editor
{
    /// <summary>
    /// Provides a menu item in the Unity Editor to enable or disable the zebra striping feature.
    /// </summary>
    public static class ZebraStripingMenu
    {
        /// <summary>
        /// Toggles the zebra striping feature. This method is executed when the user
        /// clicks the corresponding menu item. It updates the settings, logs the change,
        /// and forces the editor windows to repaint.
        /// </summary>
        [MenuItem("Tools/NewFiles/Zebra Striping/Toggle Zebra Striping")]
        public static void ToggleZebraStriping()
        {
            // Access settings and toggle the feature
            var settings = NewFilesSettings.Instance;
            settings.enableZebraStriping = !settings.enableZebraStriping;
            NewFilesSettings.SaveSettings();

            // Log the new status to the console for feedback
            string status = settings.enableZebraStriping ? "enabled" : "disabled";
            Debug.Log($"[NewFiles] Zebra Striping {status}");

            // Force the Project and Hierarchy windows to repaint to immediately reflect the change
            EditorApplication.RepaintProjectWindow();
            EditorApplication.RepaintHierarchyWindow();

            // Display a notification dialog when the feature is enabled to inform the user
            if (settings.enableZebraStriping)
            {
                EditorUtility.DisplayDialog("Zebra Striping",
                    "Zebra striping has been enabled.\n" +
                    "• Project Window: List view only (left panel)\n" +
                    "• Hierarchy Window: All items\n\n" +
                    "You can adjust colors and opacity in the settings.",
                    "OK");
            }
        }

        /// <summary>
        /// Validates the "Toggle Zebra Striping" menu item.
        /// This method sets the checkmark next to the menu item based on whether the feature is currently enabled.
        /// </summary>
        /// <returns>Always returns true to ensure the menu item is always enabled.</returns>
        [MenuItem("Tools/NewFiles/Zebra Striping/Toggle Zebra Striping", true)]
        public static bool ToggleZebraStripingValidation()
        {
            // Set the checked state of the menu item to reflect the current setting
            Menu.SetChecked("Tools/NewFiles/Zebra Striping/Toggle Zebra Striping", NewFilesSettings.Instance.enableZebraStriping);
            return true;
        }
    }
}
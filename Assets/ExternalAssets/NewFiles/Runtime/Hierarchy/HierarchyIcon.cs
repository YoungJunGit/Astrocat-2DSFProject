using UnityEngine;

namespace NewFiles.Runtime
{
    /// <summary>
    /// Stores the data for a custom icon and background color for a GameObject in the Hierarchy window.
    /// </summary>
    [DisallowMultipleComponent]
    public class HierarchyIcon : MonoBehaviour
    {
        /// <summary>
        /// The path to the icon file, relative to the Assets folder.
        /// </summary>
        [Tooltip("Path to the icon file, relative to the Assets folder.")]
        public string iconPath = "";

        /// <summary>
        /// Determines whether a custom background color has been assigned.
        /// </summary>
        [Tooltip("Specifies if a custom background color should be applied.")]
        public bool hasCustomColor = false;
        
        /// <summary>
        /// The custom background color for this GameObject in the Hierarchy.
        /// </summary>
        [Tooltip("The custom background color to apply in the Hierarchy view.")]
        public Color backgroundColor = Color.clear;
    }
}
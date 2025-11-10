using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace NewFiles.Editor
{
    /// <summary>
    /// Represents a single item in the Quick Access bar.
    /// </summary>
    [System.Serializable]
    public class QuickAccessItem
    {
        /// <summary>
        /// The unique identifier of the asset.
        /// </summary>
        public string guid;
        
        /// <summary>
        /// The asset's path. Stored as a fallback and for display purposes (e.g., tooltips).
        /// </summary>
        public string path;
        
        /// <summary>
        /// Timestamp of the last access time, used for sorting by recency.
        /// </summary>
        public float lastAccessed;
        
        public QuickAccessItem() { }
        
        public QuickAccessItem(string guid, string path)
        {
            this.guid = guid;
            this.path = path;
            this.lastAccessed = (float)EditorApplication.timeSinceStartup;
        }
        
        /// <summary>
        /// Updates the last accessed timestamp to the current time.
        /// </summary>
        public void UpdateAccessTime()
        {
            lastAccessed = (float)EditorApplication.timeSinceStartup;
        }
        
        /// <summary>
        /// Checks if the asset associated with this item still exists in the project.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));
        }
        
        /// <summary>
        /// Updates the item's path if the asset has been moved or renamed.
        /// </summary>
        public void RefreshPath()
        {
            string newPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!string.IsNullOrEmpty(newPath))
            {
                path = newPath;
            }
        }
    }

    /// <summary>
    /// A container for the list of QuickAccessItem objects. This wrapper class simplifies JSON serialization.
    /// </summary>
    [System.Serializable]
    public class QuickAccessCollection
    {
        /// <summary>
        /// The list of items in the Quick Access bar.
        /// </summary>
        public List<QuickAccessItem> items = new List<QuickAccessItem>();
        
        /// <summary>
        /// The data format version, for handling future updates.
        /// </summary>
        public int version = 1;
    }

    /// <summary>
    /// Manages the persistence (saving and loading) of Quick Access bar data.
    /// It includes features like data validation, automatic cleanup, and sorting.
    /// </summary>
    public static class QuickAccessData
    {
        // The key used to store the Quick Access data in EditorPrefs.
        private const string PREFS_KEY = "NewFiles.QuickAccessItems";
        
        // The collection of Quick Access items currently loaded in memory.
        private static QuickAccessCollection data = new QuickAccessCollection();
        
        // A flag indicating that data has changed and needs to be saved.
        private static bool isDirty = false;
        
        // Used for the delayed save mechanism to prevent excessive writes.
        private static float lastSaveTime = 0f;
        private const float SAVE_DELAY = 1f; // Delay in seconds before saving after a change.

        /// <summary>
        /// Static constructor. Loads items on startup and subscribes to the editor's update loop for delayed saving.
        /// </summary>
        static QuickAccessData()
        {
            LoadItems();
            EditorApplication.update += DelayedSave;
        }

        /// <summary>
        /// Gets the list of all Quick Access items.
        /// </summary>
        /// <returns>A list of QuickAccessItem objects.</returns>
        public static List<QuickAccessItem> GetItems()
        {
            ValidateItems();
            return data.items;
        }
        
        /// <summary>
        /// Gets the items, sorted by the most recently accessed if the feature is enabled in settings.
        /// </summary>
        /// <returns>A sorted or unsorted list of QuickAccessItem objects based on user settings.</returns>
        public static List<QuickAccessItem> GetItemsByRecentAccess()
        {
            ValidateItems();
            
            if (NewFilesSettings.Instance.autoSortQuickAccess)
            {
                return data.items.OrderByDescending(item => item.lastAccessed).ToList();
            }
            
            return data.items;
        }

        /// <summary>
        /// Adds a new item to the Quick Access bar using its asset GUID.
        /// If the item already exists, its access time is updated.
        /// If the bar is full, the oldest item is removed.
        /// </summary>
        /// <param name="guid">The GUID of the asset to add.</param>
        /// <returns>True if a new item was added, false otherwise.</returns>
        public static bool AddItem(string guid)
        {
            if (string.IsNullOrEmpty(guid)) return false;
            
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(path)) return false;
            
            var existing = data.items.FirstOrDefault(item => item.guid == guid);
            if (existing != null)
            {
                existing.UpdateAccessTime();
                existing.RefreshPath();
                MarkDirty();
                return false; 
            }
            
            int maxItems = NewFilesSettings.Instance.maxQuickAccessItems;
            if (data.items.Count >= maxItems)
            {
                var oldest = data.items.OrderBy(item => item.lastAccessed).FirstOrDefault();
                if (oldest != null)
                {
                    data.items.Remove(oldest);
                }
            }

            var newItem = new QuickAccessItem(guid, path);
            data.items.Add(newItem);
            MarkDirty();
            
            if (NewFilesSettings.Instance.enableDebugLog)
            {
                Debug.Log($"[NewFiles] Added '{path}' to Quick Access Bar");
            }
            
            return true;
        }
        
        /// <summary>
        /// Removes an item from the Quick Access bar using its GUID.
        /// </summary>
        /// <param name="guid">The GUID of the asset to remove.</param>
        /// <returns>True if the item was successfully removed, false otherwise.</returns>
        public static bool RemoveItem(string guid)
        {
            int removedCount = data.items.RemoveAll(item => item.guid == guid);
            if (removedCount > 0)
            {
                MarkDirty();
                
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log($"[NewFiles] Removed {removedCount} item(s) from Quick Access Bar");
                }
                
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Removes an item from the Quick Access bar using its path.
        /// </summary>
        /// <param name="path">The path of the asset to remove.</param>
        /// <returns>True if the item was successfully removed, false otherwise.</returns>
        public static bool RemoveItemByPath(string path)
        {
            var item = data.items.FirstOrDefault(i => i.path == path);
            if (item != null)
            {
                return RemoveItem(item.guid);
            }
            return false;
        }
        
        /// <summary>
        /// Checks if an item is already present in the Quick Access bar.
        /// </summary>
        /// <param name="guid">The GUID of the asset to check.</param>
        /// <returns>True if the item exists, false otherwise.</returns>
        public static bool ContainsItem(string guid)
        {
            return data.items.Any(item => item.guid == guid);
        }
        
        /// <summary>
        /// Updates the last access timestamp for a specific item.
        /// </summary>
        /// <param name="guid">The GUID of the asset to update.</param>
        public static void UpdateItemAccess(string guid)
        {
            var item = data.items.FirstOrDefault(i => i.guid == guid);
            if (item != null)
            {
                item.UpdateAccessTime();
                MarkDirty();
            }
        }

        /// <summary>
        /// Removes all items from the Quick Access bar.
        /// </summary>
        public static void ClearAll()
        {
            if (data.items.Count > 0)
            {
                data.items.Clear();
                MarkDirty();
                
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log("[NewFiles] Cleared all items from Quick Access Bar");
                }
            }
        }
        
        /// <summary>
        /// Validates all items, removing any that no longer correspond to an existing asset.
        /// Also refreshes the paths of valid items.
        /// </summary>
        private static void ValidateItems()
        {
            int originalCount = data.items.Count;
            data.items = data.items.Where(item => item.IsValid()).ToList();
            
            foreach (var item in data.items)
            {
                item.RefreshPath();
            }
            
            if (data.items.Count != originalCount)
            {
                MarkDirty();
                
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log($"[NewFiles] Removed {originalCount - data.items.Count} invalid items from Quick Access Bar");
                }
            }
        }

        /// <summary>
        /// Loads the Quick Access items from EditorPrefs.
        /// </summary>
        private static void LoadItems()
        {
            try
            {
                string json = EditorPrefs.GetString(PREFS_KEY, "{}");
                if (!string.IsNullOrEmpty(json) && json != "{}")
                {
                    data = JsonUtility.FromJson<QuickAccessCollection>(json) ?? new QuickAccessCollection();
                    
                    if (data.version == 0) data.version = 1;
                }
                else
                {
                    data = new QuickAccessCollection();
                }
                
                ValidateItems();
                
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log($"[NewFiles] Loaded {data.items.Count} items in Quick Access Bar");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"[NewFiles] Error loading Quick Access items, using empty list: {e.Message}");
                data = new QuickAccessCollection();
            }
        }

        /// <summary>
        /// Saves the current list of Quick Access items to EditorPrefs.
        /// </summary>
        private static void SaveItems()
        {
            try
            {
                ValidateItems();
                string json = JsonUtility.ToJson(data, true);
                EditorPrefs.SetString(PREFS_KEY, json);
                isDirty = false;
                
                if (NewFilesSettings.Instance.enableDebugLog)
                {
                    Debug.Log($"[NewFiles] Saved {data.items.Count} items to Quick Access Bar");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[NewFiles] Error saving Quick Access items: {e.Message}");
            }
        }
        
        /// <summary>
        /// Marks the data as changed, scheduling it to be saved.
        /// </summary>
        private static void MarkDirty()
        {
            isDirty = true;
            lastSaveTime = (float)EditorApplication.timeSinceStartup;
        }
        
        /// <summary>
        /// Called by EditorApplication.update. Saves data if it's dirty and the delay has passed.
        /// This approach batches multiple changes and avoids frequent disk writes.
        /// </summary>
        private static void DelayedSave()
        {
            if (isDirty && EditorApplication.timeSinceStartup - lastSaveTime > SAVE_DELAY)
            {
                SaveItems();
            }
        }
        
        /// <summary>
        /// Forces an immediate save if there are pending changes.
        /// </summary>
        public static void ForceSave()
        {
            if (isDirty)
            {
                SaveItems();
            }
        }
        
        /// <summary>
        /// Gathers and returns statistics about the Quick Access bar.
        /// </summary>
        /// <returns>A QuickAccessStatistics struct with current data.</returns>
        public static QuickAccessStatistics GetStatistics()
        {
            ValidateItems();
            return new QuickAccessStatistics
            {
                totalItems = data.items.Count,
                maxItems = NewFilesSettings.Instance.maxQuickAccessItems,
                validItems = data.items.Count(item => item.IsValid()),
                mostRecentAccess = data.items.Count > 0 ? data.items.Max(item => item.lastAccessed) : 0f
            };
        }
    }
    
    /// <summary>
    /// A simple data structure to hold statistics about the Quick Access bar.
    /// </summary>
    public struct QuickAccessStatistics
    {
        public int totalItems;
        public int maxItems;
        public int validItems;
        public float mostRecentAccess;
        
        /// <summary>
        /// The percentage of the bar's capacity that is currently used.
        /// </summary>
        public float Usage => maxItems > 0 ? (float)totalItems / maxItems : 0f;
        
        /// <summary>
        /// Indicates whether the Quick Access bar has reached its maximum capacity.
        /// </summary>
        public bool IsFull => totalItems >= maxItems;
    }
}
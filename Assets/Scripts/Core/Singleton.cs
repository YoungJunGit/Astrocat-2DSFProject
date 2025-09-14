using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _appIsQuitting = false;

    public static T Instance
    {
        get
        {
            // If the application is quitting, do not create a new instance.
            if (_appIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                    "' already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            // Use a lock for thread safety.
            lock (_lock)
            {
                if (_instance == null)
                {
                    // Find the instance in the scene.
                    _instance = (T)FindFirstObjectByType(typeof(T));

                    // Check if more than one instance exists in the scene.
                    T[] instances = FindObjectsByType<T>(FindObjectsSortMode.InstanceID);
                    if (instances.Length > 1)
                    {
                        Debug.LogError("[Singleton] More than one instance of '" + typeof(T) + "' found!");
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Debug.LogWarning("Destroying duplicate instance: " + instances[i].name);
                            Destroy(instances[i].gameObject);
                        }
                    }

                    // If no instance exists in the scene, create a new one.
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = "(Singleton) " + typeof(T).ToString();

                        // Prevent the singleton from being destroyed when loading new scenes.
                        DontDestroyOnLoad(singletonObject);

                        Debug.Log("[Singleton] A new instance of " + typeof(T) + " was created.");
                    }
                }

                return _instance;
            }
        }
    }

    private void OnDestroy()
    {
        _appIsQuitting = true;
    }
}
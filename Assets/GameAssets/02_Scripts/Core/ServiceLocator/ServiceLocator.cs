using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator global;
    private static Dictionary<Scene, ServiceLocator> sceneLocators;

    readonly ServiceManager services = new();

    internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
    {
        if (global == this)
        {
            Debug.LogWarning("ServiceLocator is already configured as global");
        }
        else if (global != null)
        {
            Debug.LogWarning("Another ServiceLocator is already configured as global");
        }
        else
        {
            global = this;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }
    }

    internal void ConfigureAsScene()
    {
        Scene scene = gameObject.scene;

        if (sceneLocators.ContainsKey(scene))
        {
            Debug.LogError($"Another ServiceLocator is already configured for this scene");
            return;
        }

        sceneLocators.Add(scene, this);
    }

    public static ServiceLocator Global
    {
        get
        {
            if (global == null)
            {
                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                var go = new GameObject("ServiceLocator [Global]");

                go.AddComponent<ServiceLocator>();
                go.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();
            }

            return global;
        }
    }

    private static List<GameObject> tempGO;


    public static ServiceLocator For(MonoBehaviour mono)
    {
        var locator = mono.GetComponentInParent<ServiceLocator>();
        if (locator == null)
        {
            locator = ForSceneOf(mono);

            if (locator == null)
            {
                return Global;
            }
        }

        return locator;
    }

    public static ServiceLocator For(ScriptableObject scriptableObject)
    {
        return ForSceneOf(scriptableObject);
    }


    public static ServiceLocator ForSceneOf(UnityEngine.Object obj)
    {
        Scene scene;
        if (obj is MonoBehaviour mono)
        {
            scene = mono.gameObject.scene;
        }
        else
        {
            scene = SceneManager.GetActiveScene();
        }

        if (sceneLocators.TryGetValue(scene, out var locator) && locator != obj)
        {
            return locator;
        }

        tempGO.Clear();
        scene.GetRootGameObjects(tempGO);

        foreach (var go in tempGO.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
        {
            if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Locator != null)
            {
                bootstrapper.BootstrapOnDemand();
            }

            return bootstrapper.Locator;
        }

        return Global;
    }

    public ServiceLocator Register<T>(T service)
    {
        services.Register(service);
        Debug.Log($"ServiceLocater.Register : {typeof(T).Name} registered");
        return this;
    }

    public ServiceLocator Register(Type type, object service)
    {
        services.Register(type, service);
        Debug.Log($"ServiceLocater.Register : {type.Name} registered");
        return this;
    }

    public ServiceLocator Get<T>(out T service) where T : class
    {
        if (TryGetService(out service))
        {
            return this;
        }
        
        if (this == Global)
        {
            return null;
        }

        if (TryGetGetNextInHierarchy(out var locator))
        {
            locator.Get(out service);
            return this;
        }
        
        Debug.LogWarning($"ServiceLocater.Get : {typeof(T).Name} is not registered in active scene\n" + "Try out to Global...");

        if (Global.Get(out service) != null) return this;

        Debug.LogError($"ServiceLocater.Get : {typeof(T).Name} is not registered");
        return null;
    }

    private bool TryGetService<T>(out T service) where T : class
    {
        return services.TryGet(out service);
    }

    private bool TryGetGetNextInHierarchy(out ServiceLocator locator)
    {
        locator = null;

        if (this == global)
        {
            locator = null;
            return false;
        }

        var parent = transform.parent;
        if (parent != null)
        {
            var servect = parent.GetComponentInParent<ServiceLocator>();

            if (servect != null)
            {
                locator = ForSceneOf(this);
            }
        }

        return locator != null;
    }

    private void OnDestroy()
    {
        if (this == global)
            global = null;
        else if (sceneLocators.ContainsValue(this))
            sceneLocators.Remove(gameObject.scene);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        global = null;
        sceneLocators = new();
        tempGO = new();
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/ServiceLocator/Add Global")]
    static void AddGlobal()
    {
        var go = new GameObject("ServiceLocator [Global]");
        go.AddComponent<ServiceLocatorGlobalBootstrapper>();

        Undo.RegisterCreatedObjectUndo(go, "Create Global ServiceLocator");
        EditorSceneManager.MarkSceneDirty(go.scene);

        Selection.activeObject = go;
    }

    [MenuItem("GameObject/ServiceLocator/Add Scene")]
    static void AddScene()
    {
        var go = new GameObject("ServiceLocator [Scene]");
        go.AddComponent<ServiceLocatorSceneBootstrapper>();

        Undo.RegisterCreatedObjectUndo(go, "Create Scene ServiceLocator");
        EditorSceneManager.MarkSceneDirty(go.scene);

        Selection.activeObject = go;
    }
#endif
}











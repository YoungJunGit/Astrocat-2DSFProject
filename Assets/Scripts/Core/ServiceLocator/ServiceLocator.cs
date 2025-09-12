using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceLocator : MonoBehaviour
{
    private static ServiceLocator global;
    private static Dictionary<Scene, ServiceLocator> sceneLocators = new();
    
    readonly ServiceManager serviceManager = new();

    public static ServiceLocator Global
    {
        get
        {
            if (global == null)
            {
                var go = new GameObject("ServiceLocator [Global]");
                
                go.AddComponent<ServiceLocator>();
                go.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();
            }
            
            return global;
        }
    }
}











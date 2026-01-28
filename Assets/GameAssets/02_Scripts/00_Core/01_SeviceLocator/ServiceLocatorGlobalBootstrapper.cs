using UnityEngine;

[AddComponentMenu("ServiceLocator/Bootstrapper Global")]
public class ServiceLocatorGlobalBootstrapper : Bootstrapper
{
    [SerializeField] bool dontDestroyOnLoad = true;

    protected override void Bootstrap()
    {
        Locator.ConfigureAsGlobal(dontDestroyOnLoad);
    }
}
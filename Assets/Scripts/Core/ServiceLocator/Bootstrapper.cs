using UnityEngine;
[DisallowMultipleComponent]
[RequireComponent(typeof(ServiceLocator))]
public abstract class Bootstrapper : MonoBehaviour
{
    ServiceLocator _locator;
    internal ServiceLocator Locator
    {
        get
        {
            if (_locator == null)
                _locator = GetComponent<ServiceLocator>();
            
            return _locator;
        }
    }

    private bool _hasBeenBootstraped;

    private void Awake()
    {
        BootstrapOnDemand();
    }

    public void BootstrapOnDemand()
    {
        if (_hasBeenBootstraped)
            return;
        
        _hasBeenBootstraped = true;
        Bootstrap();
    }
    
    protected abstract void Bootstrap();
}

[AddComponentMenu("Service Locator/Bootstrapper Global")]
public class ServiceLocatorGlobalBootstrapper : Bootstrapper
{
    [SerializeField] bool dontDestroyOnLoad = true;
    
    protected override void Bootstrap()
    {
        
    }
}

[AddComponentMenu("Service Locator/Bootstrapper Scene")]
public class ServiceLocatorSceneBootstrapper : Bootstrapper
{
    protected override void Bootstrap()
    {
        
    }
}
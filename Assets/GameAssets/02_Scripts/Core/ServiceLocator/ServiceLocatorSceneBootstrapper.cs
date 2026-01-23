using UnityEngine;

[AddComponentMenu("ServiceLocator/Bootstrapper Scene")]
public class ServiceLocatorSceneBootstrapper : Bootstrapper
{
    protected override void Bootstrap()
    {
        Locator.ConfigureAsScene();
    }
}
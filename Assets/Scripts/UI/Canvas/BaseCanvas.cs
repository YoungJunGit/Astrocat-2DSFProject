using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseCanvas : MonoBehaviour
{
    [SerializeField, BoxGroup("Camera Override Setting")]
    private bool overrideCamera = false;

    [SerializeField, BoxGroup("Camera Override Setting"), ShowIf("overrideCamera")]
    private string targetCameraName;

    [SerializeField, BoxGroup("Camera Override Setting"), ShowIf("overrideCamera")]
    private string sortingLayerName;

    public virtual void Init()
    {
        if(overrideCamera)
            SetCanvasToCamera(targetCameraName, sortingLayerName);
    }

    public void SetCanvasToCamera(string cameraName, string sortingLayerName)
    {
        ICameraManager cameraManager;
        ServiceLocator.For(this).Get(out cameraManager);

        if(cameraManager.TryGetCamera(cameraName, out var camera))
        {
            var canvas = GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.sortingLayerName = sortingLayerName;
        }
    }
}

using FunkyCode.Rendering.Day;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseCanvas : MonoBehaviour
{
    [SerializeField]
    private string sortingLayerName;

    [SerializeField, BoxGroup("Camera Override Setting")]
    private bool overrideCamera = false;

    [SerializeField, BoxGroup("Camera Override Setting"), ShowIf("overrideCamera")]
    private string targetCameraName;

    public virtual void Init()
    {
        if (overrideCamera)
        {
            var cameraManager = Camera.main.GetComponent<CameraManager>();

            if (!cameraManager.TryGetCamera(targetCameraName, out var camera)) 
            {
                Debug.Log($"No Camrea found : {targetCameraName}");
                return;
            }
            
            SetCanvasToCamera(camera, sortingLayerName);
        }
        else
        {
            SetCanvasToCamera(Camera.main, sortingLayerName);
        }
    }

    public void SetCanvasToCamera(Camera camera, string sortingLayerName)
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = camera;
        canvas.sortingLayerName = sortingLayerName;
    }
}

using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface ICameraManager
{
    public bool TryGetCamera(string name, out Camera camera);
}

public class CameraManager : MonoBehaviour, ICameraManager
{
    [SerializedDictionary("Name", "Camera")]
    public SerializedDictionary<string, Camera> CameraList;

    private void Awake()
    {
        ServiceLocator.Global.Register(this as ICameraManager);

        foreach(var camera in CameraList)
        {
            AddOverlayCamera(camera.Value);
        }
    }

    private void AddOverlayCamera(Camera overlayCamera)
    {
        var baseData = Camera.main.GetUniversalAdditionalCameraData();
        var overlayData = overlayCamera.GetUniversalAdditionalCameraData();

        overlayData.renderType = CameraRenderType.Overlay;

        if(!baseData.cameraStack.Contains(overlayCamera))
        {
            baseData.cameraStack.Add(overlayCamera);
        }
    }

    public bool TryGetCamera(string name, out Camera camera) => CameraList.TryGetValue(name, out camera);
}

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScene : AbstractScene
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private EventSystem eventSystem;

    [Header("Manager Setting")]
    [SerializeField] private TitleManager titleManager;
    [SerializeField] private SceneHandler sceneChanger;

    protected override int SceneIdx => 1;
    protected override void BindObjects() 
    {
        mainCamera = Instantiate(mainCamera);
        eventSystem = Instantiate(eventSystem);
    }

    protected override async UniTask InitializeObjects() 
    {
        titleManager.Init();
        DontDestroyOnLoad(sceneChanger);
    }
    protected override async UniTask CreateObjects()
    {
        titleManager.CreateObejct();
    }
    protected override void PrepareGame() { }
    protected override async UniTask BeginGame() 
    {
        await UniTask.WaitUntil(() => Input.anyKeyDown);

        SceneHandler.Instance.ChangeScene(2);
    }
}

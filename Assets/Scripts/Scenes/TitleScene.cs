using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScene : AbstractScene
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private EventSystem eventSystem;

    [Header("Manager Setting")]
    [SerializeField] private TitleManager titleManager;
    
    private ISoundService _soundService;

    protected override int SceneIdx { get; } = 1;
    protected override void BindObjects() 
    {
        mainCamera = Instantiate(mainCamera);
        eventSystem = Instantiate(eventSystem);
        
        ServiceLocator.For(this).Get(out _soundService);
    }

    protected override async UniTask InitializeObjects() 
    {
        titleManager.Init();
    }
    protected override async UniTask CreateObjects()
    {
        titleManager.CreateObejct();
    }
    protected override void PrepareGame() { }
    protected override async UniTask BeginGame() 
    {
        _soundService.PlayBackGround("");
        
        await UniTask.WaitUntil(() => Input.anyKeyDown);
        
        _soundService.PlayEffectSound("");

        _soundService.Clear();
        sceneHandler.ChangeScene(1);
    }
}

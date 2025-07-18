using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : AbstractScene
{
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    
    protected override int SceneIdx
    {
        get { return 1; }
    }
    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);
    }

    protected override async UniTask InitializeObjects()
    {
        
    }

    protected override async UniTask CreateObjects()
    {
        
    }

    protected override void PrepareGame()
    {
        
    }

    protected override async UniTask BeginGame()
    {
        
    }
}

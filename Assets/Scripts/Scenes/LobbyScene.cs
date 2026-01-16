using Cysharp.Threading.Tasks;
using UnityEngine;

public class LobbyScene : AbstractScene
{
    [SerializeField]
    CameraMove cameraMove;

    protected override int SceneIdx => 2;

    bool isFinishedDialogue = false;

    protected override async UniTask BeginGame()
    {
        cameraMove.StartCameraMove();
        UpdatePublisher.SubscribeObserver(cameraMove);
        await UniTask.WaitUntil(() => isFinishedDialogue);
    }

    protected override void BindObjects()
    {
        ServiceLocator.ForSceneOf(this)
            .Register(cameraMove as ICameraMove);
    }

    protected override async UniTask CreateObjects()
    {
        
    }

    protected override async UniTask InitializeObjects()
    {
        
    }

    protected override void PrepareGame()
    {
        
    }

    public void LoadScene()
    {
        UpdatePublisher.DiscribeObserver(cameraMove);
        isFinishedDialogue = true;
    }
}
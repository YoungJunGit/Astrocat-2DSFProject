using Cysharp.Threading.Tasks;
using UnityEngine;

public class TitleScene : AbstractScene
{
    [SerializeField] private Camera mainCamera;

    protected override int SceneIdx => 1;
    protected override void BindObjects() 
    {
        mainCamera = Instantiate(mainCamera);
    }

    protected override async UniTask InitializeObjects() { }
    protected override async UniTask CreateObjects(){ }
    protected override void PrepareGame() { }
    protected override async UniTask BeginGame() { }
}

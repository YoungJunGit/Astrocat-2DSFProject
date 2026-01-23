using Cysharp.Threading.Tasks;
using UnityEngine;

public class IntroScene : AbstractScene
{
    protected override int SceneIdx => 1;

    bool isFinishedDialogue = false;

    protected override async UniTask BeginGame()
    {
        await UniTask.WaitUntil(() => isFinishedDialogue);
    }

    protected override void BindObjects()
    {

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
        isFinishedDialogue = true;
    }
}
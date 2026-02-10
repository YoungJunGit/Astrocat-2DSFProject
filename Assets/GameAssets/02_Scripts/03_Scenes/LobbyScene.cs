using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;

public class LobbyScene : AbstractScene
{
    [SerializeField]
    private LobbyGenerator lobbyGenerator;

    protected override int SceneIdx => 2;

    bool isFinishedDialogue = false;

    protected override async UniTask BeginGame()
    {
        lobbyGenerator.Init();
        DialogueSystemController.OnDialogueEnd += LoadScene;
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
        lobbyGenerator.DisableCamera();
        isFinishedDialogue = true;
        DialogueSystemController.OnDialogueEnd -= LoadScene;
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueScene : AbstractScene
{
    [Header("Dialogue System")]
    [SerializeField]
    private DialogueSetting dialogueSetting;

    protected override int SceneIdx => 5;

    protected override void BindObjects()
    {
        
    }

    protected async override UniTask CreateObjects()
    {

    }

    protected async override UniTask InitializeObjects()
    {
        dialogueSetting.Init();
    }

    protected override void PrepareGame()
    {
        
    }

    protected async override UniTask BeginGame()
    {
        await dialogueSetting.ProcessDialogue();
    }
}

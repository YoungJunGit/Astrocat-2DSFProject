using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueScene : AbstractScene
{
    [Header("Dialogue System")]
    [SerializeField]
    private DialogueManager dialogueManager;

    protected override int SceneIdx { get; } = 4;

    protected override void BindObjects()
    {
        
    }

    protected async override UniTask CreateObjects()
    {

    }

    protected async override UniTask InitializeObjects()
    {
        dialogueManager.Init();
    }

    protected override void PrepareGame()
    {
        
    }

    protected async override UniTask BeginGame()
    {

    }
}

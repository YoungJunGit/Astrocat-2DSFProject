using Cysharp.Threading.Tasks;
using UnityEngine;

public class DialogueScene : AbstractScene
{
    [Header("Dialogue System")]
    [SerializeField]
    private GameObject DialogueManagerPref;

    protected override int SceneIdx { get; } = 4;

    protected override void BindObjects()
    {
        
    }

    protected async override UniTask CreateObjects()
    {
        Instantiate(DialogueManagerPref);
    }

    protected async override UniTask InitializeObjects()
    {
        
    }

    protected override void PrepareGame()
    {
        
    }

    protected async override UniTask BeginGame()
    {

    }
}

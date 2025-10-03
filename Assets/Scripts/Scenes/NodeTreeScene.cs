using Cysharp.Threading.Tasks;
using UnityEngine;

public class NodeTreeScene : AbstractScene
{
    protected override int SceneIdx { get; } = 2;
    
    [SerializeField]
    NodeMapGenerator nodeMapGenerator;
    
    protected override void BindObjects()
    {
        
    }

    protected async override UniTask InitializeObjects()
    {
        nodeMapGenerator.init();
    }

    protected async override UniTask CreateObjects()
    {
        
    }

    protected override void PrepareGame()
    {
        
    }

    protected async override UniTask BeginGame()
    {
        
    }
}

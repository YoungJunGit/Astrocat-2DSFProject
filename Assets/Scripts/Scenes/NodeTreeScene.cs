using Cysharp.Threading.Tasks;
using UnityEngine;

public class NodeTreeScene : AbstractScene
{
    protected override int SceneIdx { get; } = 2;
    
    [SerializeField]
    NodeMapGenerator nodeMapGenerator;
    [SerializeField]
    private ParticleSystem starParticle;
    [SerializeField]
    private Camera Camera3D;
    
    protected override void BindObjects()
    {
        
    }

    protected async override UniTask InitializeObjects()
    {
        nodeMapGenerator.init();
    }

    protected async override UniTask CreateObjects()
    {
        Instantiate(starParticle);
        Instantiate(Camera3D);
    }

    protected override void PrepareGame()
    {
        GameObject.Find("MainCamera").SetActive(false);
    }

    protected async override UniTask BeginGame()
    {
        
    }
}

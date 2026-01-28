using Cysharp.Threading.Tasks;
using UnityEngine;

public class NodeTreeScene : AbstractScene
{
    protected override int SceneIdx => 3;
    
    [SerializeField]
    NodeMapGenerator nodeMapGenerator;
    [SerializeField]
    private ParticleSystem starParticle;
    [SerializeField]
    private InputHandler inputHandler;
    
    protected override void BindObjects()
    {
        ServiceLocator.ForSceneOf(this)
            .Register(inputHandler)
            .Register(this as AbstractScene);
    }

    protected async override UniTask InitializeObjects()
    {
        inputHandler.Init();
        nodeMapGenerator.init();
    }

    protected async override UniTask CreateObjects()
    {
        Instantiate(starParticle);
    }

    protected override void PrepareGame()
    {
        GameObject.Find("MainCamera").SetActive(false);
    }

    protected async override UniTask BeginGame()
    {

    }
}

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : AbstractScene
{
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private HUDManager hudManager;

    protected override int SceneIdx
    {
        get { return 1; }
    }
    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);
    }

    protected override async UniTask InitializeObjects()
    {
        // load data
    }

    protected override async UniTask CreateObjects()
    {
        // Create player
        // Create enemy
        
        // Create player HUD
        // create enemy HUD
    }

    protected override void PrepareGame()
    {
        
    }

    protected override async UniTask BeginGame()
    {
        
    }
}

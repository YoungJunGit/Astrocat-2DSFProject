using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameScene : AbstractScene
{
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    
    [SerializeField] private EntityDataCreator dataCreator;
    [SerializeField] private string[] playerUnitID;
    [SerializeField] private string[] enemyUnitID;
    private List<EntityData> entityData = null;
    
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
        entityData = dataCreator.CreateEntityDataWithID(playerUnitID.ToList(), enemyUnitID.ToList());
    }

    protected override async UniTask CreateObjects()
    {
        // Create player
        List<EntityData> entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);
        foreach (EntityData playerData in entityDataList)
        {
            spawner.CreatePlayerUnit(playerData);
        }
        
        // Create enemy
        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);;
        foreach (EntityData enemyData in entityDataList)
        {
            spawner.CreatePlayerUnit(enemyData);
        }

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

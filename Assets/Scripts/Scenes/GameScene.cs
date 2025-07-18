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
    
    [Header("Data Settings")]
    [SerializeField] private EntityDataCreator dataCreator;
    [SerializeField] private string[] playerUnitID;
    [SerializeField] private string[] enemyUnitID;
    [SerializeField] private List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    [SerializeField] private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
    
    private List<EntityData> entityData = null;
    
    [Header("Game Settings")]
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
        List<EntityData> entityDataList = null;
        
        // Create player
        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);
        foreach (EntityData playerData in entityDataList)
        {
            playerUnits.Add(spawner.CreatePlayerUnit(playerData));
        }
        
        // Create enemy
        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);;
        foreach (EntityData enemyData in entityDataList)
        {
            enemyUnits.Add(spawner.CreateEnemyUnit(enemyData));
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

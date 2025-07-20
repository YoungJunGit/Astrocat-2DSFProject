using System;
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

    [Header("Data Settings")] [SerializeField]
    private EntityDataCreator dataCreator;

    [SerializeField] private string[] playerUnitID;
    [SerializeField] private string[] enemyUnitID;
    [SerializeField] private List<PlayerUnit> playerUnits = new List<PlayerUnit>();
    [SerializeField] private List<EnemyUnit> enemyUnits = new List<EnemyUnit>();

    private List<EntityData> entityData = null;

    [Header("Game Settings")] [SerializeField]
    private EntitySpawner spawner;

    [SerializeField] private HUDManager hudManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UnitPositioner unitPositioner;



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

        spawner.Init();
        hudManager.Init();
    }

    protected override async UniTask CreateObjects()
    {
        // Create Units
        List<EntityData> entityDataList = null;
        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);
        foreach (EntityData playerData in entityDataList)
        {
            playerUnits.Add(spawner.CreatePlayerUnit(playerData));
        }

        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);
        ;
        foreach (EntityData enemyData in entityDataList)
        {
            enemyUnits.Add(spawner.CreateEnemyUnit(enemyData));
        }

        // Create HUD
        foreach (var playerUnit in playerUnits)
        {
            hudManager.CreatePlayerHUD(playerUnit);
        }

        foreach (var enemyUnit in enemyUnits)
        {
            hudManager.CreateEnemyHUD(enemyUnit);
        }
    }

    protected override void PrepareGame()
    {
        // Set position for units
        // Init CombatManager
        combatManager.Init(playerUnits, enemyUnits);
    }

    protected override async UniTask BeginGame()
    {
        await combatManager.StartCombat();
    }
}
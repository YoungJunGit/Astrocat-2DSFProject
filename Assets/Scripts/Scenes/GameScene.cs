using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    [SerializeField] private TimelineSystem timeline;
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
        hudManager.Init(timeline);
    }

    protected override async UniTask CreateObjects()
    {
        // Create Units
        List<EntityData> entityDataList = null;

        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);
        foreach (var playerData in entityDataList.Select((value, index)=>(value, index)))
        {
            playerUnits.Add(spawner.CreatePlayerUnit(playerData.value, playerData.index));
        }

        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);
        foreach (var enemyData in entityDataList.Select((value, index)=>(value, index)))
        {
            enemyUnits.Add(spawner.CreateEnemyUnit(enemyData.value, enemyData.index));
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

        hudManager.CreateBanners();
    }

    protected override void PrepareGame()
    {
        // Prepare before combat start
        unitPositioner.Prepare();
        hudManager.Prepare();

        // Set Postions For Units
        unitPositioner.SetPositionForUnits(playerUnits, enemyUnits);

        // Init CombatManager
        combatManager.Init(timeline);

        // Add

        if (debugMode)
            ForDebugging();
    }

    protected override async UniTask BeginGame()
    {
        await combatManager.StartCombat();
    }

    private void ForDebugging()
    {
        if (!SceneManager.GetSceneByName("DebugingUI").isLoaded)
            SceneManager.LoadSceneAsync("DebugingUI", LoadSceneMode.Additive);
    }
}
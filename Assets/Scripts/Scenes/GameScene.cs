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

    private List<EntityData> entityData = null;

    [SerializeField] private HUDManager hudManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private TimelineSystem timeline;
    [SerializeField] private UnitManager unitManager;

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

        hudManager.Init(timeline);
        unitManager.Init();
    }

    protected override async UniTask CreateObjects()
    {
        // Create Units
        List<EntityData> entityDataList = null;

        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);
        foreach (var playerData in entityDataList.Select((value, index)=>(value, index)))
        {
            unitManager.CreatePlayerUnit(playerData.value, playerData.index);
        }

        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);
        foreach (var enemyData in entityDataList.Select((value, index)=>(value, index)))
        {
            unitManager.CreateEnemyUnit(enemyData.value, enemyData.index);
        }

        hudManager.CreateBanners();
    }

    protected override void PrepareGame()
    {
        // Prepare before combat start
        hudManager.Prepare();

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
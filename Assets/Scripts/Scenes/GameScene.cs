using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using NaughtyAttributes;
using Obvious.Soap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameScene : AbstractScene
{
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;

    [Header("Data Settings")] 
    [SerializeField] private EntityDataCreator dataCreator;
    [SerializeField] private string[] playerUnitID;
    [SerializeField] private string[] enemyUnitID;
    private List<EntityData> entityData = null;

    [Header("Manager Settings")]
    [SerializeField] private HUDManager hudManager;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private QTEManager qteManager;

    [Header("Service Locator Register")]
    [SerializeField] private UnitActionExecuter unitActionExecuter;
    [SerializeField] private DamageFactory damageFactory;

    [Header("etc")]
    [SerializeField] private TimelineSystem timelineSystem;
    [SerializeField] private InputHandler inputHandler;
    
    [Header("Tester")]
    [SerializeField] private InputTester inputTester;
    [SerializeField] private QTETester qteTester;
    [SerializeField] private CCTester ccTester;
    [SerializeField] private BackgroundChanger backgroundChanger;

    protected override int SceneIdx
    {
        get { return 2; }
    }

    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);

        ServiceLocator.ForSceneOf(this)
            .Register(unitActionExecuter as IUnitActionExecuter)
            .Register(unitManager)
            .Register(inputHandler)
            .Register(damageFactory);
    }

    protected override async UniTask InitializeObjects()
    {
        entityData = dataCreator.CreateEntityDataWithID(playerUnitID.ToList(), enemyUnitID.ToList());

        hudManager.Init();
        dialogueManager.Init();
        combatManager.Init();
        unitManager.Init();
        qteManager.Init();

        timelineSystem.Init();
        inputHandler.Init();

        unitActionExecuter.Init();
        // SoundManager.Instance.Init();

        if (debugMode)
        {
            inputTester.Init(inputHandler);
            qteTester.Init();
            backgroundChanger.Init();
        }
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

        // Create HUD
        foreach (PlayerUnit unit in unitManager.GetPlayerUnits().Cast<PlayerUnit>())
        {
            hudManager.CreatePlayerHUD(unit);
        }

        foreach (EnemyUnit unit in unitManager.GetEnemyUnits().Cast<EnemyUnit>())
        {
            hudManager.CreateEnemyHUD(unit);
        }

        // Create Banners
        timelineSystem.CreateBanners(unitManager.GetAllUnits());
    }

    protected override void PrepareGame()
    {
        // Prepare before combat start
        hudManager.Prepare();
        unitManager.Prepare();

        // Prepare CombatManager
        combatManager.Prepare(timelineSystem);

        // Add
        
        if (debugMode)
        {
            ForDebugging();
            combatManager.OnTernEnd += ccTester.SetCCOnRendomUnit;
        }
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
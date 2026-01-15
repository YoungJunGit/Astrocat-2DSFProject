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
    [Header("Data Settings")]
    [SerializeField] private DataHandler dataHandler;
    [SerializeField] private string[] playerUnitID;
    [SerializeField] private string[] enemyUnitID;
    private List<EntityData> entityData = null;

    [Header("Manager Settings")]
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private HUDManager hudManager;
    [SerializeField] private TextManager textManager;
    [SerializeField] private CombatManager combatManager;
    [SerializeField] private UnitManager unitManager;
    [SerializeField] private QTEManager qteManager;
    [SerializeField] private SelectorManager selectorManager;
    [SerializeField] private CrowdControlManager crowdControlManager;
    [SerializeField] private CombatEffectManager combatEffectManager;

    [Header("Service Locator Register")]
    [SerializeField] private UnitActionExecuter unitActionExecuter;

    [Header("etc")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private UnitActionFactory unitActionFactory;
    [SerializeField] private ParryingApplier parryingApplier;
    
    [Header("Tester")]
    [SerializeField] private InputTester inputTester;
    [SerializeField] private QTETester qteTester;
    [SerializeField] private BackgroundChanger backgroundChanger;

    protected override int SceneIdx { get; } = 3;

    protected override void BindObjects()
    {
        ServiceLocator.ForSceneOf(this)
            .Register(dataHandler)
            .Register(effectManager as IEffectManager)
            .Register(unitActionExecuter as IUnitActionExecuter)
            .Register(textManager as ICombatTextManager)
            .Register(unitManager as IUnitManager)
            .Register(selectorManager as ISelectorManager)
            .Register(inputHandler)
            .Register(unitActionFactory)
            .Register(parryingApplier as IParryingApplier)
            .Register(crowdControlManager as ICrowdControlManager)
            .Register(combatEffectManager as ICombatEffectManager);
    }

    protected override async UniTask InitializeObjects()
    {
        entityData = new EntityDataCreator().CreateEntityData(dataHandler, playerUnitID.ToList(), enemyUnitID.ToList());

        hudManager.Init();
        textManager.Init();
        combatManager.Init();
        unitManager.Init();
        qteManager.Init();
        selectorManager.Init();
        crowdControlManager.Init();
        combatEffectManager.Init();

        inputHandler.Init();

        unitActionExecuter.Init();

        parryingApplier.Init();

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

        // Create etc
        combatManager.CreateObjects();
    }

    protected override void PrepareGame()
    {
        // Prepare before combat start
        hudManager.Prepare();
        unitManager.Prepare();

        // Prepare CombatManager
        combatManager.Prepare();
        
        if (debugMode)
        {
            ForDebugging();
        }
    }

    protected override async UniTask BeginGame()
    {
        _soundService.PlayBackGround("Title_Background", true);
        await combatManager.StartCombat();
        mapDataFactory.SaveCompletedMapData();
    }

    private void ForDebugging()
    {
        if (!SceneManager.GetSceneByName("DebugingUI").isLoaded)
            SceneManager.LoadSceneAsync("DebugingUI", LoadSceneMode.Additive);
    }
}
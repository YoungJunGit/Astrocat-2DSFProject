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
        unitPositioner.Init();
    }

    protected override async UniTask CreateObjects()
    {
        // Create Units
        List<EntityData> entityDataList = null;

        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);

        Vector2 pos;

        foreach (var playerData in entityDataList.Select((value, index)=>(value, index)))
        {
            // TODO : 포지션 설정이 PrepareGame함수에서 이루어질 수 있도록 변경해야 함 - 97번줄 주석 참조
            //pos = unitPositioner.playerPositionCaculate(playerData.index + 1);
            playerUnits.Add(spawner.CreatePlayerUnit(playerData.value, playerData.index));
        }

        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);
        foreach (var enemyData in entityDataList.Select((value, index)=>(value, index)))
        {
            // TODO : 포지션 설정이 PrepareGame함수에서 이루어질 수 있도록 변경해야 함 - 97번줄 주석 참조
            //pos = unitPositioner.enemyPositionCaculate(enemyData.index + 1);
            enemyUnits.Add(spawner.CreateEnemyUnit(enemyData.value, enemyData.index));
        }

        // Create HUD
        foreach (var playerUnit in playerUnits)
        {
            hudManager.CreatePlayerHUD(playerUnit);
        }

        int index = 0;
        foreach (var enemyUnit in enemyUnits)
        {
            hudManager.CreateEnemyHUD(enemyUnit);
            index++;
        }

        hudManager.CreateBanners();
    }

    protected override void PrepareGame()
    {
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

    // 디버깅용으로만 사용할 것
    public void onPlayerDestroy() 
    {

        if (playerUnits.Count > 0 && playerUnits[0] != null)
        {
            GameObject.Destroy(playerUnits[0].gameObject);
            playerUnits.RemoveAt(0);
        }

        unitPositioner.playerDataCount = playerUnits.Count+1;

        for (int i = 0; i < playerUnits.Count; i++)
        {
            Vector2 newPosition = unitPositioner.playerPositionCaculate(i+1);

            // ?? : Init()함수 실행이 이때 되는게 맞는지?
            unitPositioner.Init();
            playerUnits[i].transform.position = newPosition;
        }
    }

    // 디버깅용으로만 사용할 것
    public void onEnemyDestroy()
    {

        if (enemyUnits.Count > 0 && enemyUnits[0] != null)
        {
            GameObject.Destroy(enemyUnits[0].gameObject);
            enemyUnits.RemoveAt(0);
        }

        unitPositioner.EnemyDataCount = enemyUnits.Count + 1;

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Vector2 newPosition = unitPositioner.enemyPositionCaculate(i + 1);

            // ?? : Init()함수 실행이 이때 되는게 맞는지?
            unitPositioner.Init();
            enemyUnits[i].transform.position = newPosition;
        }
    }
}
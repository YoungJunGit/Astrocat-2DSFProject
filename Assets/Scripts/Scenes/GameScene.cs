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

    Vector2 position;

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
        unitPositioner.Init();
    }

    protected override async UniTask CreateObjects()
    {
        // Create Units
        List<EntityData> entityDataList = null;
        entityDataList = entityData.FindAll(element => element.Side == SIDE.PLAYER);

        unitPositioner.playerDataCount = entityDataList.Count + 1;
        unitPositioner.Init();

        int index = 0;
        foreach (EntityData playerData in entityDataList)
        {
            position = unitPositioner.playerPositionCaculate(index);
            playerUnits.Add(spawner.CreatePlayerUnit(playerData, position));
            index++;
        }

        entityDataList = entityData.FindAll(element => element.Side == SIDE.ENEMY);
        index = 0;
        foreach (EntityData enemyData in entityDataList)
        {
            position = unitPositioner.enemyPositionCaculate(index);
            enemyUnits.Add(spawner.CreateEnemyUnit(enemyData, position));
            index++;
        }

        // Create HUD
        foreach (var playerUnit in playerUnits)
        {
            hudManager.CreatePlayerHUD(playerUnit);
        }

        index = 0;
        foreach (var enemyUnit in enemyUnits)
        {
            hudManager.CreateEnemyHUD(enemyUnit, index);
            index++;
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

    public void onPlayerDestroy() {

        if (playerUnits.Count > 0 && playerUnits[0] != null)
        {
            hudManager.DeletePlayerHUD(playerUnits[0]);
            GameObject.Destroy(playerUnits[0].gameObject);
            playerUnits.RemoveAt(0);
        }

        unitPositioner.playerDataCount = playerUnits.Count+1;
        unitPositioner.Init();

        for (int i = 0; i < playerUnits.Count; i++)
        {
            Vector2 newPosition = unitPositioner.playerPositionCaculate(i);
            playerUnits[i].transform.position = newPosition;
        }
    }

    public void onEnemyDestroy()
    {

        if (enemyUnits.Count > 0 && enemyUnits[0] != null)
        {
            hudManager.DeleteEnemyHUD(enemyUnits[0]);
            GameObject.Destroy(enemyUnits[0].gameObject);
            enemyUnits.RemoveAt(0);
        }

        unitPositioner.playerDataCount = enemyUnits.Count + 1;
        unitPositioner.Init();

        for (int i = 0; i < enemyUnits.Count; i++)
        {
            Vector2 newPosition = unitPositioner.enemyPositionCaculate(i);
            enemyUnits[i].transform.position = newPosition;
        }
        hudManager.repositionEnemyHUD();
    }
}
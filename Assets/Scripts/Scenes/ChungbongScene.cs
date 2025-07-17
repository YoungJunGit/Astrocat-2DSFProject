using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using Obvious.Soap;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ChungbongScene : AbstractScene
{
    [Header("Base Objects")]
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    
    [Header("Game Logic")]
    [SerializeField] private C_TimeLine timeLine;
    [SerializeField] private C_Status status;
    [SerializeField] private HUDManager hudManger;
    [SerializeField] private CombatManager combatManager;

    [Header("Spawners")]
    [SerializeField] private EntitySpawner playerSpawner;
    [SerializeField] private EntitySpawner enemySpawner;
    
    [Header("Data")]
    [SerializeField] private EntityDataCreator entityDataCreator;
    [SerializeField] private PlayerParty playerParty;
    [SerializeField] private List<string> enemyCharacterID = new List<string>();

    protected override int SceneIdx
    {
        get => 1;
    }
    
    /// <summary>
    /// Instantiate
    /// </summary>
    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);
        timeLine = Instantiate(timeLine);
        status = Instantiate(status);
        
        playerSpawner = Instantiate(playerSpawner);
        enemySpawner = Instantiate(enemySpawner);
    }

    /// <summary>
    /// Call Init(); func
    /// </summary>
    protected override async UniTask InitializeObjects()
    {  
        var timelineSystem = timeLine.GetTimeLineSystem();
        
        List<EntityData> entityDataList = entityDataCreator.CreateEntityDataWithID(playerParty.GetPlayerParty(), enemyCharacterID);

        combatManager.Initialize(entityDataList ,timelineSystem, playerSpawner, enemySpawner, hudManger);
        hudManger.Initialize();
    }

    /// <summary>
    /// Sean load, Create Poolable objects, etc.
    /// </summary>
    protected override async UniTask CreateObjects()
    {
        if (debugMode.Value)
            await SceneManager.LoadSceneAsync("DebugingUI", LoadSceneMode.Additive);
    }

    protected override void PrepareGame()
    {
    }

    protected override async UniTask BeginGame()
    {
        
    }
}
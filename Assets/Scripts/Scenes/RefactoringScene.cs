using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RefactoringScene : AbstractScene
{
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private TimeLine timeLine;
    [SerializeField] private CombatManager combatManager;
    
    [SerializeField] private CharacterData CharacterDataList;
    [SerializeField] private MonsterData MonsterDataList;
    
    protected override int SceneIdx
    {
        get => 1;
    }
    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);
        timeLine = Instantiate(timeLine);
    }

    /// <summary>
    /// Call Init(); func
    /// </summary>
    protected override async UniTask InitializeObjects()
    {
        List<CharacterDataEntity> playerData = new();
        foreach (var characterDataEntity in CharacterDataList.data)
        {
            playerData.Add(characterDataEntity);
        }

        List<MonsterDataEntity> enemyData = new();
        foreach (var monsterDataEntity in MonsterDataList.data)
        {
            enemyData.Add(monsterDataEntity);
        }
        
        var timelineSystem = timeLine.GetTimeLineSystem();
        timelineSystem.Init(playerData, enemyData);
        
        combatManager.Init(timelineSystem);
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
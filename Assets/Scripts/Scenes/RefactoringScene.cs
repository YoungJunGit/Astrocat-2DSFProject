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
    
    
    public CharacterData CharacterDataList;
    public MonsterData MonsterDataList;
    
    protected override int SceneIdx
    {
        get => 1;
    }
    protected override void BindObjects()
    {
        camera = Instantiate(camera);
        eventSystem = Instantiate(eventSystem);
    }

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
        
        
    }

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
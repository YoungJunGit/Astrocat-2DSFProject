using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BattleUIScene : AbstractScene
{
    [Header("Base Objects")]
    [SerializeField] private Camera camera;
    [SerializeField] private EventSystem eventSystem;
    
    [Header("Game Logic")]

    [Header("Spawners")]
    
    [Header("Data")]
    
    protected override int SceneIdx
    {
        get => 1;
    }
    
    /// <summary>
    /// Instantiate
    /// </summary>
    protected override void BindObjects()
    {
    }

    /// <summary>
    /// Call Init(); func
    /// </summary>
    protected override async UniTask InitializeObjects()
    {
    }

    /// <summary>
    /// Sean load, Create Poolable objects, etc.
    /// </summary>
    protected override async UniTask CreateObjects()
    {
    }

    protected override void PrepareGame()
    {
    }

    protected override async UniTask BeginGame()
    {
        
    }
}
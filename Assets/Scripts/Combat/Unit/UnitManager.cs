using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitManager", menuName = "GameScene/UnitManager", order = 2)]
class UnitManager : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList = null;

    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private UnitPositioner positioner;
    
    //[SerializeField] private List<PlayerUnit> playerUnits = new();
    //[SerializeField] private List<EnemyUnit> enemyUnits = new();

    public void Init()
    {
        spawner.Init();
        positioner.Prepare();
    }
    
    public PlayerUnit CreatePlayerUnit(EntityData entityData, int index)
    {
        var playerUnit = spawner.CreatePlayerUnit(entityData, index);
        
        //playerUnits.Add(playerUnit);
        unitList.Add(playerUnit);

        SetUnitPosition();
        
        return playerUnit;
    }

    public EnemyUnit CreateEnemyUnit(EntityData entityData, int index)
    {
        var enemyUnit = spawner.CreateEnemyUnit(entityData, index);
        
        //enemyUnits.Add(enemyUnit);
        unitList.Add(enemyUnit);
        
        SetUnitPosition();
        
        return enemyUnit;
    }

    //public List<PlayerUnit> GetAllPlayerUnits() => playerUnits;
    //public List<EnemyUnit> GetAllEnemyUnits() => enemyUnits;
    
    public PlayerUnit GetCurrentPlayerUnit()
    {
        return null;
    }

    public async UniTask<EnemyUnit> GetEnemyUnitBySelector()
    {
        return null;
    }
    
    public async UniTask<PlayerUnit> GetPlayerUnitBySelector()
    {
        return null;
    }
    
    private void SetUnitPosition()
    {
        positioner.SetPositionForUnits(unitList.GetPlayerUnits(), unitList.GetEnemyUnits());
    }
}
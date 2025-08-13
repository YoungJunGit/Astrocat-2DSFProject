using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitManager", menuName = "GameScene/UnitManager", order = 2)]
class UnitManager : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList = null;

    [SerializeField] private UnitSelector unitSelector;
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private UnitPositioner positioner;
    public void Init()
    {
        spawner.Init();
        unitSelector.Init();
    }

    public void Prepare()
    {
        foreach (BaseUnit unit in unitList)
        {
            unit.GetStat().OnDie += (stat) => SetUnitPosition();
        }
    }
    
    public PlayerUnit CreatePlayerUnit(EntityData entityData, int index)
    {
        var playerUnit = spawner.CreatePlayerUnit(entityData, index);
        
        unitList.Add(playerUnit);

        SetUnitPosition();
        
        return playerUnit;
    }

    public EnemyUnit CreateEnemyUnit(EntityData entityData, int index)
    {
        var enemyUnit = spawner.CreateEnemyUnit(entityData, index);
        
        unitList.Add(enemyUnit);
        
        SetUnitPosition();
        
        return enemyUnit;
    }

    private void CheckGameCondition() {
        if (unitList.GetPlayerUnits().Count == 0)
        {
            Debug.Log("Player Loss!!!");
            UnityEditor.EditorApplication.isPlaying = false;

            // todo => XP depend on game player give
        }
        else if (unitList.GetEnemyUnits().Count == 0)
        {
            Debug.Log("Player Win!!!");
            UnityEditor.EditorApplication.isPlaying = false;
            
            //todo => XP, Money ,Item payment
        }
    }

    public async UniTask<EnemyUnit> GetEnemyUnitBySelector()
    {
        return await unitSelector.SelectUnit(SIDE.ENEMY) as EnemyUnit;
    }
    
    public async UniTask<PlayerUnit> GetPlayerUnitBySelector()
    {
        return await unitSelector.SelectUnit(SIDE.PLAYER) as PlayerUnit;
    }

    public List<BaseUnit> GetAllUnit()
    {
        return unitList.GetUnits();
    }

    private void SetUnitPosition()
    {
        positioner.SetPositionForUnits(unitList.GetPlayerUnits(), unitList.GetEnemyUnits());
    }
}
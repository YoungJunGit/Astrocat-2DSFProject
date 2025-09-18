using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitManager", menuName = "GameScene/UnitManager", order = 2)]
public class UnitManager : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit currentUnitList = null;

    [SerializeField] private UnitSelector unitSelector;
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private UnitPositioner positioner;

    private List<BaseUnit> unitList = new List<BaseUnit>();

    public void Init()
    {
        spawner.Init();
        unitSelector.Init();
    }

    public void Prepare()
    {
        foreach (BaseUnit unit in currentUnitList)
        {
            unit.GetStat().OnDamaged += unit.OnDamaged;
            unit.GetStat().OnHealed += unit.OnHealed;
            unit.GetStat().OnDie += () => { unit.OnDie().Forget(); };
            unit.m_FinishedDying += (deadUnit) => { currentUnitList.Remove(deadUnit); SetUnitPosition(); };
        }
    }
    
    public PlayerUnit CreatePlayerUnit(EntityData entityData, int index)
    {
        var playerUnit = spawner.CreatePlayerUnit(entityData, index);
        
        currentUnitList.Add(playerUnit);
        unitList.Add(playerUnit);

        SetUnitPosition();
        
        return playerUnit;
    }

    public EnemyUnit CreateEnemyUnit(EntityData entityData, int index)
    {
        var enemyUnit = spawner.CreateEnemyUnit(entityData, index);
        
        currentUnitList.Add(enemyUnit);
        unitList.Add(enemyUnit);

        SetUnitPosition();
        
        return enemyUnit;
    }

    private void CheckGameCondition() {
        if (currentUnitList.GetPlayerUnits().Count == 0)
        {
            Debug.Log("Player Loss!!!");
            UnityEditor.EditorApplication.isPlaying = false;

            // todo => XP depend on game player give
        }
        else if (currentUnitList.GetEnemyUnits().Count == 0)
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

    public PlayerUnit GetRandomPlayerUnitBySelector()
    {
        return unitSelector.SelectRandomUnit(SIDE.PLAYER) as PlayerUnit;
    }

    public List<BaseUnit> GetAllUnit()
    {
        return currentUnitList.GetUnits();
    }

    private void SetUnitPosition()
    {
        positioner.SetPositionForUnits(currentUnitList.GetPlayerUnits(), currentUnitList.GetEnemyUnits());
    }
}
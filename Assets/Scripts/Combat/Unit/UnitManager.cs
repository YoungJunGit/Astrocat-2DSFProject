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

    private BaseUnit _selectedUnit;
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

    public async UniTask GetEnemyUnitBySelector()
    {
        _selectedUnit = await unitSelector.SelectUnit(SIDE.ENEMY);
    }
    
    public async UniTask GetPlayerUnitBySelector()
    {
        _selectedUnit = await unitSelector.SelectUnit(SIDE.PLAYER);
    }

    public void GetRandomPlayerUnitBySelector()
    {
        _selectedUnit = unitSelector.SelectRandomUnit(SIDE.PLAYER);
    }

    public BaseUnit SelectedUnit => _selectedUnit;

    public List<BaseUnit> GetAllUnit()
    {
        return currentUnitList.GetUnits();
    }

    private void SetUnitPosition()
    {
        positioner.SetPositionForUnits(currentUnitList.GetPlayerUnits(), currentUnitList.GetEnemyUnits());
    }
}
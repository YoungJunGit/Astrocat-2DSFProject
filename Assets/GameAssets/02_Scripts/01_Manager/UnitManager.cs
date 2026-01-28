using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using Unity.VisualScripting;
using UnityEngine;
using Obvious.Soap;

public interface IUnitManager
{
    public List<BaseUnit> GetEnemyUnits();
    public List<BaseUnit> GetPlayerUnits();
    public List<BaseUnit> GetUnit(SIDE side);
    public List<BaseUnit> GetAllUnits();
}

[CreateAssetMenu(fileName = "UnitManager", menuName = "SO/Combat/Manager/UnitManager", order = 0)]
public class UnitManager : ScriptableObject, IUnitManager
{
    [SerializeField] private BoolVariable saveDataMode;

    [SerializeField] private PlayerPartyData playerPartyData;
    [SerializeField] private EnemyPartyData enemyPartyData;

    [SerializeField] private ScriptableListBaseUnit currentUnitList = null;
    [SerializeField] private EntitySpawner spawner;
    [SerializeField] private UnitPositioner positioner;

    private readonly List<BaseUnit> unitList = new List<BaseUnit>();

    public string[] PlayerPartyID => playerPartyData.SelectedPlayerUnitID;
    public string[] EnemyPartyID => enemyPartyData.FinalSpawnEnemyID;

    public void Init()
    {
        spawner.Init();
        unitList.Clear();

        enemyPartyData.MakeEnemyIDWithSetting();
        if (!saveDataMode) return;
        playerPartyData.EnsureInitialized();
    }

    public void CreatePlayerUnit(EntityData entityData, int index)
    {
        PlayerSaveData saveData = null;
        if (saveDataMode)
        {
            saveData = playerPartyData.LoadPlayerData(entityData.code);
            if (saveData == null || saveData.HP <= 0) return;
        }

        var playerUnit = spawner.CreatePlayerUnit(entityData, index, saveData);

        currentUnitList.Add(playerUnit);
        unitList.Add(playerUnit);

        SetUnitPosition();
    }

    public void CreateEnemyUnit(EntityData entityData, int index)
    {
        var enemyUnit = spawner.CreateEnemyUnit(entityData, index);

        currentUnitList.Add(enemyUnit);
        unitList.Add(enemyUnit);

        SetUnitPosition();
    }

    public void Prepare()
    {
        foreach (BaseUnit unit in currentUnitList)
        {
            unit.m_FinishedDying += (deadUnit) => { currentUnitList.Remove(deadUnit); SetUnitPosition(); };
        }
    }

    public void SavePlayerPartyData()
    {
        if (saveDataMode)
        {
            List<BaseUnit> units = unitList.FindAll(element => element.GetStat().CoreStat.Side == SIDE.PLAYER);
            foreach (var unit in units)
            {
                playerPartyData.SavePlayerData(unit);
            }
        }
    }

    public List<BaseUnit> GetEnemyUnits()
    {
        return currentUnitList.GetEnemyUnits();
    }

    public List<BaseUnit> GetPlayerUnits()
    {
        return currentUnitList.GetPlayerUnits();
    }

    public List<BaseUnit> GetUnit(SIDE side)
    {
        if (side == SIDE.PLAYER)
            return currentUnitList.GetPlayerUnits();
        else if (side == SIDE.ENEMY)
            return currentUnitList.GetEnemyUnits();

        return null;
    }

    public BaseUnit GetUnitByID(string ID) => currentUnitList.ToList().Find(unit => unit.GetStat().CoreStat.ID == ID);

    public List<BaseUnit> GetAllUnits()
    {
        return currentUnitList.GetUnits();
    }

    private void SetUnitPosition()
    {
        positioner.SetPositionForUnits(GetPlayerUnits(), GetEnemyUnits());
    }
}
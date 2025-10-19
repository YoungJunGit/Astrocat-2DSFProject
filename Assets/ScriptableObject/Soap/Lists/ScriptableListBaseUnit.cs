using Obvious.Soap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "scriptable_list_" + nameof(BaseUnit), menuName = "Soap/ScriptableLists/"+ nameof(BaseUnit))]
public class ScriptableListBaseUnit : ScriptableList<BaseUnit>
{
    public List<BaseUnit> GetUnits()
    {
        return _list;
    }

    public List<BaseUnit> GetUnits(DataEnum.SIDE side)
    {
        List<BaseUnit> units = new List<BaseUnit>();
        if (side == DataEnum.SIDE.PLAYER)
        {
            foreach (BaseUnit unit in _list)
            {
                if (unit is PlayerUnit)
                    units.Add(unit as PlayerUnit);
            }
        }
        else if (side == DataEnum.SIDE.ENEMY)
        {
            foreach (BaseUnit unit in _list)
            {
                if (unit is EnemyUnit)
                    units.Add(unit as EnemyUnit);
            }
        }

        return units;
    }

    public List<BaseUnit> GetPlayerUnits()
    {
        List<BaseUnit> playerUnits = new List<BaseUnit>();
        foreach (BaseUnit unit in _list)
        {
            if (unit is PlayerUnit)
                playerUnits.Add(unit);
        }
        return playerUnits;
    }

    public List<BaseUnit> GetEnemyUnits()
    {
        List<BaseUnit> enemyUnits = new List<BaseUnit>();
        foreach (BaseUnit unit in _list)
        {
            if (unit is EnemyUnit)
                enemyUnits.Add(unit);
        }
        return enemyUnits;
    }
}

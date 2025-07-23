using System;
using UnityEngine;
using Obvious.Soap;
using System.Collections.Generic;

[CreateAssetMenu(fileName = nameof(ScriptableDictionaryUnit_HUD), menuName = "Soap/ScriptableDictionary/"+nameof(ScriptableDictionaryUnit_HUD))]
public class ScriptableDictionaryUnit_HUD : ScriptableDictionary<BaseUnit,BaseHUD>
{
    public List<BaseUnit> GetUnits()
    {
        List<BaseUnit> unitList = new List<BaseUnit>();
        foreach(BaseUnit unit in _dictionary.Keys)
        {
            unitList.Add(unit);
        }
        return unitList;
    }

    public List<PlayerUnit> GetPlayerUnits()
    {
        List<PlayerUnit> playerUnits = new List<PlayerUnit>();
        foreach (BaseUnit unit in _dictionary.Keys)
        {
            if(unit is PlayerUnit)
                playerUnits.Add(unit as PlayerUnit);
        }
        return playerUnits;
    }

    public List<EnemyUnit> GetEnemyUnits()
    {
        List<EnemyUnit> enemyUnits = new List<EnemyUnit>();
        foreach (BaseUnit unit in _dictionary.Keys)
        {
            if(unit is EnemyUnit)
                enemyUnits.Add(unit as EnemyUnit);
        }
        return enemyUnits;
    }

    public List<BaseHUD> GetHUDs()
    {
        List<BaseHUD> hudList = new List<BaseHUD>();
        foreach (BaseHUD hud in _dictionary.Values)
        {
            hudList.Add(hud);
        }
        return hudList;
    }
}
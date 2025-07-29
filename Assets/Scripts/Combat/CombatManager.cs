using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "GameScene/CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList;
    [SerializeField] private ActionSelector actionSelector;
    private BaseUnit currentTurnUnit;

    public Func<List<BaseUnit>, BaseUnit> DequeueCurrentUnit;

    private bool isStartCombat = false;
    
    public void Init(TimelineSystem timeline)
    {
        DequeueCurrentUnit += timeline.Pop;
        currentTurnUnit = timeline.PrepareCombat(unitList.GetUnits());

        foreach (BaseUnit unit in unitList)
        {
            unit.GetStat().OnDie += OnCharacterDie;
        }
        actionSelector.Init();
    }
    
    public async UniTask StartCombat()
    {
        isStartCombat = true;
        while (true)
        {
            if (currentTurnUnit is PlayerUnit)
            {
                UnitAction selectedAction = await actionSelector.SelectAction(currentTurnUnit as PlayerUnit);

                await selectedAction.Execute();
            }

            //TODO: Check is finish
            //if ()

            currentTurnUnit = DequeueCurrentUnit(unitList.GetUnits());
        }
    }

    public void OnCharacterDie(UnitStat stat)
    {
        if (currentTurnUnit.GetStat() == stat)
        {
            Debug.Log("Current Character Died!! Turn Skip!");
            currentTurnUnit = DequeueCurrentUnit(unitList.GetUnits());
        }
    }

    #region[For Debugging]
    public void DieCharacter(SIDE side, int index)
    {
        BaseUnit unit = unitList.GetUnits().Find(unit=>unit.GetStat().Priority + 1 == index
                                                     && unit.GetStat().GetData().Side == side);

        if (unit != null)
            unitList.Remove(unit);

        unit?.GetStat().OnDie(unit.GetStat());
    }

    public void BuffCharacter(SIDE side, int index, Buff buff)
    {
        BaseUnit unit = unitList.GetUnits().Find(unit => unit.GetStat().Priority + 1 == index
                                                     && unit.GetStat().GetData().Side == side);

        unit?.AddBuff(buff);
    }
    #endregion
}
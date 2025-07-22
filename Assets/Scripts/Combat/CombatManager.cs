using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "GameScene/CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;

    private BaseUnit currentTurnUnit;

    private ActionSelector actionSelector = new();
    private UnitSelector unitSelector = new();
    
    public Func<List<BaseUnit>, BaseUnit> m_EndTurn;

    private bool isStartCombat = false;
    
    public void Init(TimelineSystem timeline, List<PlayerUnit> playerUnits, List<EnemyUnit> enemyUnits)
    {
        m_EndTurn += timeline.OnEndTurn;
        currentTurnUnit = timeline.PrepareCombat(unit_HUD_Dic.GetUnits());
    }
    
    public async UniTask StartCombat()
    {
        isStartCombat = true;
        while (true)
        {
            //Debug.Log("TurnStart : " + currentTurnUnit.GetStat().GetData().Name);

            if (currentTurnUnit is PlayerUnit)
            {
                int selectedAction = await actionSelector.SelectAction();
                EnemyUnit selectedUnit = await unitSelector.SelectEnemyUnit();
            }
            else
            {
                // TODO : AI Logic for Enemy Unit
                PlayerUnit selectedUnit = await unitSelector.SelectPlayerUnit();
            }

            //TODO: Execute Action

            //TODO: Check is finish
            //if ()

            currentTurnUnit = m_EndTurn(unit_HUD_Dic.GetUnits());
        }
    }

    #region[For Debugging]
    public void DieCharacter(SIDE side, int index)
    {
        BaseUnit unit = unit_HUD_Dic.GetUnits().Find(unit=>unit.GetStat().Priority + 1 == index
                                                     && unit.GetStat().GetData().Side == side);

        unit_HUD_Dic.Remove(unit);

        unit?.GetStat().OnDie(unit.GetStat());
    }

    public void BuffCharacter(SIDE side, int index, Buff buff)
    {
        BaseUnit unit = unit_HUD_Dic.GetUnits().Find(unit => unit.GetStat().Priority + 1 == index
                                                     && unit.GetStat().GetData().Side == side);

        unit?.AddBuff(buff);
    }
    #endregion
}
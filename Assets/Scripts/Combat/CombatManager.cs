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
    
    public void Init(TimelineSystem timeline)
    {
        m_EndTurn += timeline.Pop;
        currentTurnUnit = timeline.PrepareCombat(unit_HUD_Dic.GetUnits());

        foreach (BaseUnit unit in unit_HUD_Dic.Keys)
        {
            unit.GetStat().OnDie += OnCharacterDie;
        }
    }
    
    public async UniTask StartCombat()
    {
        isStartCombat = true;
        while (true)
        {
            //Debug.Log("TurnStart : " + currentTurnUnit.GetStat().GetData().Name);

            if (currentTurnUnit is PlayerUnit)
            {
                UnitAction selectedAction = await actionSelector.SelectAction(currentTurnUnit as PlayerUnit);

                await selectedAction.Execute();
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

    public void OnCharacterDie(UnitStat stat)
    {
        if (currentTurnUnit.GetStat() == stat)
        {
            Debug.Log("Current Character Died!! Turn Skip!");
            currentTurnUnit = m_EndTurn(unit_HUD_Dic.GetUnits());
        }
    }
}
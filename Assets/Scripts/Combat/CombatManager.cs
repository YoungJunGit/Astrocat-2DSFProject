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
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private UnitManager unitManager;
    private BaseUnit currentTurnUnit;

    public Func<List<BaseUnit>, BaseUnit> DequeueCurrentUnit;
    public Action OnTernEnd;

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
        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectAction))
        {
            isStartCombat = true;
            while (true)
            {
                Debug.Log($"{currentTurnUnit.GetStat().GetData().Name}'s turn");
                await UniTask.WaitForSeconds(1);

                if (currentTurnUnit is PlayerUnit)
                {
                    IUnitAction selectedAction = await actionSelector.SelectAction(currentTurnUnit as PlayerUnit);

                    await selectedAction.Execute();
                }
                else if (currentTurnUnit is EnemyUnit)
                {
                    // TODO : Enemy Action
                }
                ApplyCrowdControl();
                
                //TODO: Check is finish
                //if ()

                currentTurnUnit = DequeueCurrentUnit(unitList.GetUnits());
                
                OnTernEnd?.Invoke();
            }
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

    public void ApplyCrowdControl()
    {
        var units = unitManager.GetAllUnit();
        
        foreach (var unit in units)
        {
            unit.GetCrowdControlManager().ApplyCrowdControl();
        }
    }
}
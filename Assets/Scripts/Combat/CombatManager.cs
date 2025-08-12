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

    public BaseUnit GetCurrentTurnUnit() => currentTurnUnit;

    public async UniTask StartCombat()
    {
        isStartCombat = true;
        while (true)
        {
            Debug.Log($"{currentTurnUnit.GetStat().GetData().Name}'s turn");
            await UniTask.WaitForSeconds(1);

            IUnitAction selectedAction = null;
            if (currentTurnUnit is PlayerUnit)
            {
                await UniTask.WaitForSeconds(1);
                selectedAction = await actionSelector.SelectAction(currentTurnUnit as PlayerUnit);
            }
            else if (currentTurnUnit is EnemyUnit)
            {
                selectedAction = await actionSelector.SelectAction(currentTurnUnit as EnemyUnit);
            }

            if (selectedAction != null)
            {
                await selectedAction.Execute();
            }

            OnTernEnd?.Invoke();
            ApplyCrowdControl();

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

    public void ApplyCrowdControl()
    {
        var units = unitManager.GetAllUnit();
        
        foreach (var unit in units)
        {
            unit.GetCrowdControlManager().ApplyCrowdControl();
        }
    }
}
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
    [SerializeField] private EventHandler combatEventHandler;
    [SerializeField] private UnitManager unitManager;

    private BaseUnit currentTurnUnit;
    private TimelineSystem _timeline;

    private EventRegistry<List<BaseUnit>, BaseUnit> DequeueCurrentUnit = new();
    public Action OnTernEnd;

    public bool executed;
    
    private IUnitActionExecuter actionExecuter;

    public void Init(TimelineSystem timeline)
    {
        _timeline = timeline;
        DequeueCurrentUnit.Register(_timeline.Pop);
        currentTurnUnit = _timeline.PrepareCombat(unitList.GetUnits());

        foreach (BaseUnit unit in unitList)
        {
            unit.m_FinishedDying += OnCharacterDie;
        }

        actionSelector.Init();
        
        ServiceLocator.For(this)
            .Get(out actionExecuter);
    }

    public BaseUnit GetCurrentTurnUnit() => currentTurnUnit;

    public async UniTask StartCombat()
    {
        while (unitList.GetUnits(SIDE.ENEMY).Count != 0 && unitList.GetUnits(SIDE.PLAYER).Count != 0)
        {
            Debug.Log($"{currentTurnUnit.GetStat().GetData().Name}'s turn");

            if (_timeline.CurrentTurnBanner.GetState() == EntityBanner.BannerState.NORMAL)
            {
                IUnitAction selectedAction = null;

                if (currentTurnUnit is PlayerUnit player)
                {
                    selectedAction = await actionSelector.SelectAction(player);
                }
                else if (currentTurnUnit is EnemyUnit enemy)
                {
                    selectedAction = await actionSelector.SelectAction(enemy);
                }

                await actionExecuter.ExecuteRequest(currentTurnUnit, selectedAction);
            }

            OnTernEnd?.Invoke();
            ApplyCrowdControl();

            await UniTask.WaitUntil(() => combatEventHandler.IsEventEmpty());
            await UniTask.WaitForSeconds(1);

            currentTurnUnit = DequeueCurrentUnit.Call(unitList.GetUnits());
        }

        // TODO: Check whether the enemy or the player wins
        // if()
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        if (currentTurnUnit == unit)
        {
            Debug.Log("Current Character Died!! Turn Skip!");
            _timeline.OnCharacterDie(unit);
            currentTurnUnit = DequeueCurrentUnit.Call(unitList.GetUnits());
        }
    }

    public void OnFainting()
    {
        //_timeline.Actions.FaintingButton();
    }

    public void OnExtraTurn()
    {
        //_timeline.Actions.ExtraButton();
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
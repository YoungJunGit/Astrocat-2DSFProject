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

    public Func<List<BaseUnit>, BaseUnit> DequeueCurrentUnit;
    public Action OnTernEnd;

    private bool isStartCombat = false;
    public bool executed;
    
    private IUnitActionExecuter actionExecuter;

    public void Init(TimelineSystem timeline)
    {
        _timeline = timeline;
        DequeueCurrentUnit += timeline.Pop;
        currentTurnUnit = timeline.PrepareCombat(unitList.GetUnits());

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
        isStartCombat = true;
        while (true)
        {
            Debug.Log($"{currentTurnUnit.GetStat().GetData().Name}'s turn");

            if (_timeline.timelineUI.GetCurrentTurnBanner().GetState() == EntityBanner.BannerState.NORMAL)
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

            //TODO: Check is finish
            //if ()
            currentTurnUnit = DequeueCurrentUnit(unitList.GetUnits());

            //await ProcessCurrentTurnAsync();
        }
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        if (currentTurnUnit == unit)
        {
            Debug.Log("Current Character Died!! Turn Skip!");
            _timeline.OnCharacterDie(unit);
            currentTurnUnit = DequeueCurrentUnit(unitList.GetUnits());
        }
    }

    public void OnFainting()
    {
        _timeline.GetActions().FaintingButton();
    }

    public void OnExtraTurn()
    {
        _timeline.GetActions().ExtraButton();
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
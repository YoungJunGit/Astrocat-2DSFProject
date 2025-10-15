using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "GameScene/CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private ActionSelector actionSelector;
    [SerializeField] private EventHandler combatEventHandler;
    [SerializeField] private TimelineManager timelineManagerPrefab;

    private UnitManager unitManager;
    private TimelineManager _timelineManager;
    private BaseUnit currentTurnUnit;

    private EventRegistry<List<BaseUnit>, BaseUnit> DequeueCurrentUnit = new();
    public Action OnTernEnd;

    public bool executed;

    private IUnitActionExecuter actionExecuter;
    private ISoundService _soundService;

    public void Init()
    {
        _timelineManager = Instantiate(timelineManagerPrefab);

        ServiceLocator.For(this)
                      .Get(out actionExecuter)
                      .Get(out unitManager);

        actionSelector.Init();
        _timelineManager.Init();
    }

    public void CreateObjects()
    {
        _timelineManager.CreateTimeline(unitManager.GetAllUnits());
    }

    public void Prepare()
    {
        _timelineManager.Prepare(unitManager.GetAllUnits());
        DequeueCurrentUnit.Register(_timelineManager.Pop);

        foreach (BaseUnit unit in unitManager.GetAllUnits())
        {
            unit.m_FinishedDying += OnCharacterDie;
        }

        actionSelector.Init();
        
        ServiceLocator.For(this)
            .Get(out actionExecuter)
            .Get(out _soundService);


    }

    public async UniTask StartCombat()
    {
        while (unitManager.GetEnemyUnits().Count != 0 && unitManager.GetPlayerUnits().Count != 0)
        {
            currentTurnUnit = DequeueCurrentUnit.Call(unitManager.GetAllUnits());

            Debug.Log($"{currentTurnUnit.GetStat().GetData().Name}'s turn");

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

            OnTernEnd?.Invoke();
            ApplyCrowdControl();

            await UniTask.WaitUntil(() => combatEventHandler.IsEventEmpty());
            await UniTask.WaitForSeconds(1);
        }

        // TODO: Check whether the enemy or the player wins
        // if()
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        _timelineManager.DeleteBanners(unit);
        if (currentTurnUnit == unit)
            currentTurnUnit = DequeueCurrentUnit.Call(unitManager.GetAllUnits());
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
        var units = unitManager.GetAllUnits();

        foreach (var unit in units)
        {
            unit.GetCrowdControlManager().ApplyCrowdControl();
        }
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

public class CombatSelectionContext
{
    public IUnitAction Action;
    public ITarget<BaseUnit> Target;
}

[CreateAssetMenu(fileName = "CombatManager", menuName = "GameScene/CombatManager", order = 1)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private EventHandler combatEventHandler;
    [SerializeField] private TimelineManager timelineManagerPrefab;

    private UnitManager _unitManager;
    private TimelineManager _timelineManager;
    private BaseUnit currentTurnUnit;

    private EventRegistry<List<BaseUnit>, BaseUnit> DequeueCurrentUnit = new();
    public Action OnTernEnd;

    public bool executed;

    private ISelectorManager _selectorManager;
    private IUnitActionExecuter _actionExecuter;
    private ISoundService _soundService;

    public void Init()
    {
        _timelineManager = Instantiate(timelineManagerPrefab);

        ServiceLocator.For(this)
                      .Get(out _actionExecuter)
                      .Get(out _unitManager)
                      .Get(out _selectorManager)
                      .Get(out _soundService);

        _timelineManager.Init();
    }

    public void CreateObjects()
    {
        _timelineManager.CreateTimeline(_unitManager.GetAllUnits());
    }

    public void Prepare()
    {
        _timelineManager.Prepare(_unitManager.GetAllUnits());
        DequeueCurrentUnit.Register(_timelineManager.Pop);

        foreach (BaseUnit unit in _unitManager.GetAllUnits())
        {
            unit.m_FinishedDying += OnCharacterDie;
        }
    }

    public async UniTask StartCombat()
    {
        while (_unitManager.GetEnemyUnits().Count != 0 && _unitManager.GetPlayerUnits().Count != 0)
        {
            currentTurnUnit = DequeueCurrentUnit.Call(_unitManager.GetAllUnits());

            Debug.Log($"{currentTurnUnit.GetStat().Name}'s turn");

            var context = new CombatSelectionContext();
            // Step 1 : Add Selections
            _selectorManager.AddSelectorExecuter(new ActionSelectorExecutor(currentTurnUnit, (action) => context.Action = action));
            _selectorManager.AddSelectorExecuter(new UnitSelectorExecutor(currentTurnUnit, context, (bag) => context.Target = bag));

            // Step 2 : Execute Selections
            await _selectorManager.ExecuteAll();

            // Step 3 : Execute Action to Target
            await _actionExecuter.ExecuteRequest(currentTurnUnit, context.Action, context.Target);

            OnTernEnd?.Invoke();

            await UniTask.WaitUntil(() => combatEventHandler.IsEventEmpty());
            await UniTask.WaitForSeconds(1);
        }

        DequeueCurrentUnit.UnregisterAll();
        // TODO: Check whether the enemy or the player wins
        // if()
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        _timelineManager.DeleteBanners(unit);
        if (currentTurnUnit == unit)
            currentTurnUnit = DequeueCurrentUnit.Call(_unitManager.GetAllUnits());
    }

    public void OnFainting()
    {
        //_timeline.Actions.FaintingButton();
    }

    public void OnExtraTurn()
    {
        //_timeline.Actions.ExtraButton();
    }
}
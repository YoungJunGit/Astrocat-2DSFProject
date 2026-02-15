using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "CombatManager", menuName = "SO/Combat/Manager/CombatManager", order = 0)]
public class CombatManager : ScriptableObject
{
    [SerializeField] private TimelineManager timelineManagerPrefab;

    private TimelineManager _timelineManager;
    private BaseUnit currentTurnUnit;

    private IUnitManager _unitManager;
    private IUnitActionExecuter _actionExecuter;
    private ICombatTextManager _textManager;
    private ISelectorManager _selectorManager;
    private ISoundService _soundService;

    private bool finishedCombat;
    public bool IsPlayerLoose { get; private set; }

    public void Init()
    {
        _timelineManager = Instantiate(timelineManagerPrefab);
        finishedCombat = false;
        IsPlayerLoose = false;

        ServiceLocator.For(this)
                      .Get(out _unitManager)
                      .Get(out _actionExecuter)
                      .Get(out _textManager)
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

        foreach (BaseUnit unit in _unitManager.GetAllUnits())
        {
            unit.m_FinishedDying += OnCharacterDie;
        }
    }

    public async UniTask StartCombat()
    {
        while (_unitManager.GetEnemyUnits().Count != 0 && _unitManager.GetPlayerUnits().Count != 0)
        {
            currentTurnUnit = _timelineManager.Pop(_unitManager.GetAllUnits());

            currentTurnUnit.CEUnit.OnStartTurn();
            await UniTask.WaitUntil(() => EventHandler.IsEventEmpty());

            if (finishedCombat)
                continue;

            Debug.Log($"{currentTurnUnit.GetStat().CoreStat.Name}'s turn");
            await UniTask.WaitUntil(() => !_textManager.IsTextOn);

            // ForDebugControlEffect(currentTurnUnit, ELEMENT_TYPE.PHYSICAL);
            _selectorManager.UpdateActionSelector(currentTurnUnit);

            if (currentTurnUnit.CCUnit.EffectsCountDic[ELEMENT_TYPE.PHYSICAL].CurrentValue <= 0)
            {
                // Step 1 : Add Action Selection
                IUnitActionProperty property = null;
                IUnitActionInvoker invoker = null;
                _selectorManager.AddSelectorExecuter(
                    new ActionSelectorExecutor(
                        currentTurnUnit, 
                        (action) =>
                        {
                            invoker = action;
                            property = action;
                        }
                    )
                );

                // Step 2 : Add Target Selection
                ITarget<BaseUnit> target = null;
                _selectorManager.AddSelectorExecuter(
                    new UnitSelectorExecutor(
                        currentTurnUnit, 
                        () => property, 
                        (bag) =>
                        {
                        target = bag;
                        }
                    )
                );

                // Step 3 : Execute Selections
                await _selectorManager.ExecuteAll();

                // Step 4 : Execute Action to Target
                await _actionExecuter.ExecuteRequest(currentTurnUnit, invoker, property, target);
            }

            await UniTask.WaitUntil(() => EventHandler.IsEventEmpty());
            await UniTask.WaitForSeconds(1);

            currentTurnUnit.TurnUpdate();
        }
        // TODO: Check whether the enemy or the player wins
        // if()
    }

    public void OnCharacterDie(BaseUnit unit)
    {
        _timelineManager.DeleteBanners(unit);

        if (_unitManager.GetEnemyUnits().Count == 0 || _unitManager.GetPlayerUnits().Count == 0)
        {
            if(_unitManager.GetPlayerUnits().Count == 0)
            {
                IsPlayerLoose = true;
            }
            finishedCombat = true;
            return;
        }

        if (currentTurnUnit == unit)
            currentTurnUnit = _timelineManager.Pop(_unitManager.GetAllUnits());
    }
}
using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using UnityEngine;

public interface ISelectorExecutor
{
    public Type SelectorType { get; }
    public UniTask<bool> ExecuteWith(BaseSelector selector);
}

public abstract class SelectorExecutor<TSelector> : ISelectorExecutor where TSelector : BaseSelector
{
    public Type SelectorType => typeof(TSelector);
    public async UniTask<bool> ExecuteWith(BaseSelector selector) => await Execute((TSelector)selector);
    public abstract UniTask<bool> Execute(TSelector selector);
}

public class ActionSelectorExecutor : SelectorExecutor<ActionSelector>
{
    private readonly BaseUnit _currentUnit;
    private readonly Action<IUnitAction> _onSelected;

    public ActionSelectorExecutor(BaseUnit unit, Action<IUnitAction> onSelected)
    {
        _currentUnit = unit;
        _onSelected = onSelected;
    }

    public override async UniTask<bool> Execute(ActionSelector selector)
    {
        if (_currentUnit is PlayerUnit player)
            await selector.SelectAction(player, _onSelected);
        else if (_currentUnit is EnemyUnit enemy)
            selector.SelectAction(enemy, _onSelected);

        return true;
    }
}

public class UnitSelectorExecutor : SelectorExecutor<UnitSelector>
{
    private readonly BaseUnit _currentUnit;
    private readonly CombatSelectionContext _context;
    private readonly Action<ITarget<BaseUnit>> _onSelected;

    public UnitSelectorExecutor(BaseUnit unit, CombatSelectionContext context, Action<ITarget<BaseUnit>> onSelected)
    {
        _currentUnit = unit;
        _context = context;
        _onSelected = onSelected;
    }

    public override async UniTask<bool> Execute(UnitSelector selector)
    {
        ITarget<BaseUnit> target = TargetFactory.CreateTarget(_context.Action.Action_Type);
        ITargetStrategy targetStrategy;

        // Force Attack
        Func<BaseUnit, bool> forceFilter = null;
        Action onForceAttack = null;
        if (_currentUnit.GetStat().ModifierStat.TauntInfo.Count > 0)
        {
            if (_context.Action.Action_Type == TARGET_TYPE.SINGLE || _context.Action.Action_Type == TARGET_TYPE.RANDOM)
            {
                forceFilter = (unit) => unit.GetStat().CoreStat.ID != _currentUnit.GetStat().ModifierStat.TauntInfo.ForcedTargetID;
                var tauntInfo = _currentUnit.GetStat().ModifierStat.TauntInfo;
                onForceAttack = () => {
                    _currentUnit.GetStat().ModifierStat.TauntInfo = (Mathf.Max(tauntInfo.Count - 1, 0), tauntInfo.ForcedTargetID);
                    
                    if(_currentUnit.GetStat().ModifierStat.TauntInfo.Count <= 0)
                        _currentUnit.GetStat().ModifierStat.TauntInfo = (0, string.Empty);
                };
            }
        }

        targetStrategy = TargetStrategyFactory.CreateTargetStrategy(_context.Action.Action_Type, _context.Action.Target_Filter, forceFilter);

        // Self Attack
        if (_context.Action.Target_Type == SIDE.NONE)
            return true;

        bool isSelected = false;
        if (_currentUnit is PlayerUnit)
        {
            isSelected = await selector.SelectTarget(target, targetStrategy, _context.Action.Target_Type);
        }
        else if (_currentUnit is EnemyUnit)
        {
            targetStrategy = TargetStrategyFactory.CreateTargetStrategy(TARGET_TYPE.RANDOM, _context.Action.Target_Filter, forceFilter);
            isSelected = selector.SelectRandomTarget(target, targetStrategy);
        }

        _onSelected?.Invoke(target);

        if(isSelected)
            onForceAttack?.Invoke();

        return isSelected;
    }
}

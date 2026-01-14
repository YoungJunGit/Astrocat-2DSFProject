using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DataEnum;
using TMPro;
using Utils;

[CreateAssetMenu(fileName = "UnitSelector", menuName = "GameScene/UnitSelector", order = 1)]
public class UnitSelector : BaseSelector
{
    [SerializeField] private UnitSelectorObject unitSelectArrowPrefab;
    private UnitSelectorController controller;
    private IUnitManager _unitManager;
    private ISoundService _soundService;

    private List<UnitSelectorObject> arrowList = new();
    private ITarget<BaseUnit> _bag;
    private ITargetStrategy _strategy;

    private SIDE _side;
    private bool isConfirmed;
    private bool isCancled;

    public override void Init()
    {
        InputHandler inputHandler;
        ServiceLocator.For(this)
            .Get(out inputHandler)
            .Get(out _unitManager)
            .Get(out _soundService);

        _side = SIDE.NONE;
        isConfirmed = false;
        arrowList.Clear();

        controller = new UnitSelectorController(
            inputHandler,
            _unitManager,
            () => { ConfirmSelection(_bag, _strategy); },
            () => { isCancled = true; DestroyUnitSelectArrow(); },
            (value) => { SetUnitSelectArrow(_bag, _strategy, value); }
        );
    }

    public async UniTask<bool> SelectTarget(ITarget<BaseUnit> bag, ITargetStrategy strategy, SIDE side)
    {
        _bag = bag;
        _strategy = strategy;
        _side = side;
        isConfirmed = false;
        isCancled = false;

        switch (strategy)
        {
            case SingleTargetStrategy:
            case SplashTargetStrategy:
                using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
                {
                    controller.Prepare(_unitManager.GetUnit(side).Count, side);
                    controller.OnStartSelect(side, strategy.Filters);
                    await UniTask.WaitUntil(() => isConfirmed == true || isCancled == true);
                    controller.OnEndSelect(side);
                }
                break;
            case AllTargetStrategy:
                using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
                {
                    controller.Prepare(_unitManager.GetUnit(side).Count, side);
                    controller.InputHandler.OnSelectUnitTouch += () => ConfirmSelection(_bag, _strategy, true);
                    await UniTask.WaitUntil(() => isConfirmed == true || isCancled == true);
                }
                break;
            case RandomTargetStratgy:
                strategy.SelectTarget(_unitManager.GetUnit(_side), bag);
                break;
        }

        return !isCancled;
    }

    public bool SelectRandomTarget(ITarget<BaseUnit> bag, ITargetStrategy strategy)
    {
        strategy.SelectTarget(_unitManager.GetPlayerUnits(), bag);

        if (bag.Targets.Count == 0)
            return false;

        return true;
    }

    private void SetUnitSelectArrow(ITarget<BaseUnit> bag, ITargetStrategy strategy, int targetIndex)
    {
        _soundService.PlayEffectSound("Click");
        DestroyUnitSelectArrow();
        strategy.SelectTarget(_unitManager.GetUnit(_side), bag, targetIndex);
        CreateUnitSelectArrow(_bag, _strategy);
    }

    private void CreateUnitSelectArrow(ITarget<BaseUnit> bag, ITargetStrategy strategy)
    {
        foreach (var target in bag.Targets)
        {
            UnitSelectorObject arrow = Instantiate(unitSelectArrowPrefab, target.Attachments.GetUnitSelectArrowPos(), false);
            bool IsSelectable = !TargetFilterUtility.IsFiltered(strategy.Filters, target);
            arrow.Init(_side, IsSelectable);
            arrowList.Add(arrow);
        }
    }

    private void DestroyUnitSelectArrow()
    {
        foreach (var arrow in arrowList)
        {
            Destroy(arrow.gameObject);
        }
        arrowList.Clear();
    }

    private void ConfirmSelection(ITarget<BaseUnit> bag, ITargetStrategy strategy, bool isTouch = false)
    {
        if(isTouch)
        {
            if(!RaycastExtensions.RaycastMouse(LayerMask.GetMask("Character"), out BaseUnit unit))
                return;

            if(!(unit.GetStat().CoreStat.Side == _side))
                return;
        }

        if (strategy is SingleTargetStrategy)
        {
            var unit = TargetExtensions.SingleOrDefaultFast(bag);
            bool IsSelectable = !TargetFilterUtility.IsFiltered(strategy.Filters, unit);

            if (!IsSelectable)
                return;
        }
        else if (strategy is AllTargetStrategy or SplashTargetStrategy)
        {
            bool IsSelectable = false;
            foreach (var unit in bag.Targets)
            {
                IsSelectable = !TargetFilterUtility.IsFiltered(strategy.Filters, unit);
                
                if (IsSelectable)
                    break;
            }
            
            if(!IsSelectable)
                return;
        }

        isConfirmed = true;
        DestroyUnitSelectArrow();
    }
}
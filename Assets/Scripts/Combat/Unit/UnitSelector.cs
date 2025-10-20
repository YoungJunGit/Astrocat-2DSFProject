using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DataEnum;
using TMPro;

[CreateAssetMenu(fileName = "UnitSelector", menuName = "GameScene/UnitSelector", order = 1)]
class UnitSelector : ScriptableObject
{
    [SerializeField] private UnitSelectorController controller;
    [SerializeField] private UnitSelectorObject unitSelectArrowPrefab;
    private ScriptableListBaseUnit _units;
    private List<UnitSelectorObject> arrowList = new();

    private UnitSelectorObject unitSelectArrow;
    private ITarget<BaseUnit> _bag;
    private ITargetStrategy _strategy;
    private SIDE _side;
    private bool isConfirmed;

    public void Init(ScriptableListBaseUnit units)
    {
        _units      = units;
        _side       = SIDE.NONE;
        isConfirmed = false;
        arrowList.Clear();
        controller.Initialize(() => isConfirmed = true, 
            (index) => unitSelectArrow.transform.SetParent(_units.GetUnits(_side)[index].attachments.GetUnitSelectArrowPos(), false));
    }

    public async UniTask SelectTarget(ITarget<BaseUnit> bag, ITargetStrategy strategy, SIDE side)
    {
        _bag        = bag;
        _strategy   = strategy;
        _side       = side;
        isConfirmed = false;
        controller.UpdateIndex(_units.GetUnits(side).Count, side);

        switch (strategy)
        {
            case SingleTargetStrategy:
            case SplashTargetStrategy:
                using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
                {
                    CreateUnitSelectArrow(bag, strategy, controller.GetSelectionIndex(_side));
                    controller.Prepare();
                    controller.OnStartSelect(side);
                    await UniTask.WaitUntil(() => isConfirmed == true);
                    controller.OnEndSelect(side);
                    DestroyUnitSelectArrow();
                }
                break;
            case AllTargetStrategy:
                using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
                {
                    CreateUnitSelectArrow(bag, strategy, controller.GetSelectionIndex(_side));
                    controller.Prepare();
                    await UniTask.WaitUntil(() => isConfirmed == true);
                    DestroyUnitSelectArrow();
                }
                break;
            case RandomTargetStratgy:
                strategy.SelectTarget(_units.GetUnits(_side), bag);
                break;
        }
    }

    public void SelectRandomTarget(ITarget<BaseUnit> bag, ITargetStrategy strategy)
    {
        strategy.SelectTarget(_units.GetPlayerUnits(), bag);
    }

    public async UniTask<BaseUnit> SelectUnit(SIDE side)
    {
        _side = side;
        isConfirmed = false;
        controller.UpdateIndex(_units.GetUnits(side).Count, side);

        int selectIndex = controller.GetSelectionIndex(side);
        unitSelectArrow = Instantiate(unitSelectArrowPrefab, _units.GetUnits(side)[selectIndex].attachments.GetUnitSelectArrowPos(), false);
        unitSelectArrow.Init(side);

        using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
        {
            controller.Prepare();
            controller.OnStartSelect(side);
            await UniTask.WaitUntil(() => isConfirmed == true);
            controller.OnEndSelect(side);
        }

        Destroy(unitSelectArrow.gameObject);

        return _units.GetUnits(side)[controller.GetSelectionIndex(side)];
    }

    public BaseUnit SelectRandomUnit(SIDE side)
    {
        int randomIndex = Random.Range(0, _units.GetUnits(side).Count);
        return _units.GetUnits(side)[randomIndex];
    }

    private void SetUnitSelectArrow(int targetIndex)
    {
        DestroyUnitSelectArrow();
        CreateUnitSelectArrow(_bag, _strategy, targetIndex);
    }

    private void CreateUnitSelectArrow(ITarget<BaseUnit> bag, ITargetStrategy strategy, int targetIndex)
    {
        strategy.SelectTarget(_units.GetUnits(_side), bag, targetIndex);

        foreach(var target in bag.Targets)
        {
            UnitSelectorObject arrow = Instantiate(unitSelectArrowPrefab, target.attachments.GetUnitSelectArrowPos(), false);
            bool IsSelectable = strategy.Filter == null || !strategy.Filter(target);
            arrow.Init_Temp(_side, IsSelectable);
            arrowList.Add(arrow);
        }
    }

    private void DestroyUnitSelectArrow()
    {
        foreach(var arrow in arrowList)
        {
            Destroy(arrow.gameObject);
        }
        arrowList.Clear();
    }
}
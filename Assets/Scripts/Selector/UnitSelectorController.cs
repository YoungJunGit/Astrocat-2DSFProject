using System;
using DataEnum;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class UnitSelectorController
{
    public InputHandler InputHandler => _inputHandler;
    private InputHandler _inputHandler;
    private IUnitManager _unitManager;

    private Action confirm;
    private Action cancle;
    private Action<int> select;

    private int _selectedUnitIndex;
    private int _previousEnemySelectionIndex;
    private int _previousPlayerSelectionIndex;

    private int _maxUnitCount;

    private SIDE targetSide = SIDE.NONE;
    private Func<BaseUnit, bool>[] _filters;

    public UnitSelectorController(InputHandler inputHandler, IUnitManager unitManager, Action confirm, Action cancle, Action<int> select)
    {
        _inputHandler = inputHandler;
        _unitManager = unitManager;

        this.confirm = confirm;
        this.cancle = cancle;
        this.select = select;
    }

    public void Prepare(int maxUnitCount, SIDE side)
    {
        _inputHandler.OnSelectUnitSelectionConfirm += () => confirm();
        _inputHandler.OnSelectUnitSelectionCancle += () => cancle();

        _maxUnitCount = maxUnitCount;

        if (side == SIDE.ENEMY)
        {
            _previousEnemySelectionIndex = Mathf.Clamp(_previousEnemySelectionIndex, 0, maxUnitCount - 1);
            select(_previousEnemySelectionIndex);
        }
        else if (side == SIDE.PLAYER)
        {
            _previousPlayerSelectionIndex = Mathf.Clamp(_previousPlayerSelectionIndex, 0, maxUnitCount - 1);
            select(_previousPlayerSelectionIndex);
        }
    }

    public void OnStartSelect(SIDE side, Func<BaseUnit, bool>[] filters)
    {
        targetSide = side;
        _filters = filters;

        if (side == SIDE.ENEMY)
        {
            _selectedUnitIndex = _previousEnemySelectionIndex;
            _inputHandler.OnSelectUnitEnemySelectionMove += OnUnitSelect;
        }
        else if (side == SIDE.PLAYER)
        {
            _selectedUnitIndex = _previousPlayerSelectionIndex;
            _inputHandler.OnSelectUnitPlayerSelectionMove += OnUnitSelect;
        }

        _inputHandler.OnSelectUnitTouch += OnUnitTouch;
    }

    public void OnEndSelect(SIDE side)
    {
        targetSide = SIDE.NONE;
        _filters = null;

        if (side == SIDE.ENEMY)
            _previousEnemySelectionIndex = _selectedUnitIndex;
        else if (side == SIDE.PLAYER)
            _previousPlayerSelectionIndex = _selectedUnitIndex;
    }

    private void OnUnitTouch()
    {
        if (targetSide == SIDE.NONE) return;

        if(RaycastExtensions.RaycastMouse(LayerMask.GetMask("Character"), out BaseUnit unit))
        {
            var unitList = _unitManager.GetUnit(targetSide);

            int idx = unitList.IndexOf(unit);
            if (idx < 0)
                return;

            bool selectable = !TargetFilterUtility.IsFiltered(_filters, unit);

            if (idx != _selectedUnitIndex)
            {
                _selectedUnitIndex = idx;
                select(_selectedUnitIndex);
                return;
            }

            if (selectable)
                confirm();
        }
    }

    private void OnUnitSelect(int value)
    {
        int temp = _selectedUnitIndex;
        _selectedUnitIndex = Mathf.Clamp(_selectedUnitIndex + value, 0, _maxUnitCount - 1);

        if (_selectedUnitIndex != temp)
            select(_selectedUnitIndex);
    }
}

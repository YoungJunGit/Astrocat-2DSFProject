using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputHandler", menuName = "Core/InputHandler", order = 1)]
public class InputHandler : ScriptableObject, UserInputAction.ISelectUnitActions, UserInputAction.ISelectActionActions, UserInputAction.IQTEActions
{
    public enum InputState
    {
        None,
        SelectUnit,
        SelectAction,
        QTE
    }
    private InputState _currentInputState = InputState.None;
    private UserInputAction _userInputAction;
    
    public Action OnSelectUnitEnemySelectionMove;
    public Action OnSelectUnitPlayerSelectionMove;
    public Action OnSelectUnitOnClick;
    public Action OnSelectUnitSelectionConfirm;
    
    public Action OnSelectActionBaseAttack;
    public Action OnSelectActionSkillSelect;
    public Action OnSelectActionUseItem;
    
    public Action OnQTEButtonA;

    public void SetInputState(InputState inputState)
    {
        switch (inputState)
        {
            case InputState.None:
                _userInputAction.Disable();
                break;
            case InputState.SelectUnit:
                _userInputAction.Disable();
                _userInputAction.SelectUnit.Enable();
                break;
            case InputState.SelectAction:
                _userInputAction.Disable();
                _userInputAction.SelectAction.Enable();
                break;
            case InputState.QTE:
                _userInputAction.Disable();
                _userInputAction.QTE.Enable();
                break;
        }
    }
    
    public void Init()
    {
        if (_userInputAction == null)
        {
            _userInputAction = new UserInputAction();
            _userInputAction.SelectUnit.SetCallbacks(this);
            _userInputAction.SelectAction.SetCallbacks(this);
            _userInputAction.QTE.SetCallbacks(this);
        }

        SetInputState(InputState.None);
    }

    public void OnEnemySelectionMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectUnitEnemySelectionMove.Invoke();
    }

    public void OnPlayerSelectionMove(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectUnitPlayerSelectionMove.Invoke();
    }

    public void OnOnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectUnitOnClick.Invoke();
    }

    public void OnSelectionConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectUnitSelectionConfirm.Invoke();
    }

    public void OnBaseAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectActionBaseAttack.Invoke();
    }

    public void OnSkillSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectActionSkillSelect.Invoke();
    }

    public void OnUseItem(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnSelectActionUseItem.Invoke();
    }

    public void OnButtonA(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnQTEButtonA.Invoke();
    }
}
using System;
using UnityEngine;

public class InputTester : MonoBehaviour, IUpdateObserver
{
    
    [SerializeField] private InputHandler.InputState _inputState = InputHandler.InputState.None;
    [SerializeField] private bool updateInputState = false;
    
    private InputHandler _inputHandler;

    public void Init(InputHandler inputHandler)
    {
        _inputHandler = inputHandler;
        
        _inputHandler.OnSelectUnitEnemySelectionMove += () => Debug.Log("Enemy Selection Move");
        _inputHandler.OnSelectUnitPlayerSelectionMove += () => Debug.Log("Player Selection Move");
        _inputHandler.OnSelectUnitOnClick += () => Debug.Log("Unit On Click");
        _inputHandler.OnSelectUnitSelectionConfirm += () => Debug.Log("Unit Selection Confirm");
        
        _inputHandler.OnSelectActionBaseAttack += () => Debug.Log("Base Attack Selected");
        _inputHandler.OnSelectActionSkillSelect += () => Debug.Log("Skill Select Selected");
        _inputHandler.OnSelectActionUseItem += () => Debug.Log("Use Item Selected");
        
        _inputHandler.OnQTEButtonA += () => Debug.Log("QTE Button A Pressed");
        
        UpdatePublisher.SubscribeObserver(this);
    }

    public void ObserverUpdate(float dt)
    {
        if (updateInputState)
            _inputHandler.CurrentInputState = _inputState;
        
        _inputState = _inputHandler.CurrentInputState;
    }
}

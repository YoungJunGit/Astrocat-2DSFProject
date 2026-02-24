
using System;
using System.Threading;
using DataEnum;
using UnityEngine;

[CreateAssetMenu(fileName = "ParryingApplier", menuName = "SO/Combat/Unit/ParryingApplier", order = 5)]
public class ParryingApplier : IParryingApplier
{
    private ISingleTargetContext _context;
    private InputDisposer _inputDisposer;
    private InputHandler _inputHandler;

    private bool _hasParried;
    private bool _isParryActive;
    
    public ParryingApplier(ISingleTargetContext context) 
    {
        _context = context;

        var inputHandler = context.InputHandler;
        _inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.Parry);

        _inputHandler = inputHandler;
        _inputHandler.OnParry += StartParry;
        
        context.Target.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.PARRY_START, () => { _isParryActive = true; });
        context.Target.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.PARRY_END, () => { _isParryActive = false; });
    }
    
    private async void StartParry()
    {
        if (_hasParried) return;
        
        _hasParried = true;
        await _context.Target.AnimationHandler.ChangeAnimationAsync(ANIMATION.PARRY);
        await _context.Target.AnimationHandler.ChangeAnimationAsync(ANIMATION.IDLE);
    }

    public bool JudgeParrying()
    {
        // TODO : Judge parrying
        // TODO : Cancle Action
        // TODO : Start Counter Attack

        return _isParryActive;
    }

    public void Dispose()
    {
        _inputHandler.OnParry -= StartParry;
        _inputDisposer?.Dispose();
    }
}

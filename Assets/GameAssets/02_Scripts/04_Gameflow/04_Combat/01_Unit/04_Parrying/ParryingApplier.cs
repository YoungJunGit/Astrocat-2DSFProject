
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
    
    private bool _isParrySuccess;
    
    public ParryingApplier(ISingleTargetContext context) 
    {
        _context = context;

        var inputHandler = context.InputHandler;
        _inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.Parry);

        _inputHandler = inputHandler;
        _inputHandler.OnParry += StartParry;
        
        context.Target.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.PARRY_START, () => { _isParrySuccess = true; });
        context.Target.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.PARRY_END, () => { _isParrySuccess = false; });
    }
    
    private async void StartParry()
    {
        await _context.Target.AnimationHandler.ChangeAnimationAsync(ANIMATION.PARRY);
        await _context.Target.AnimationHandler.ChangeAnimationAsync(ANIMATION.IDLE);
    }

    public bool JudgeParrying()
    {
        // TODO : Judge parrying
        // TODO : Cancle Action
        // TODO : Start Counter Attack

        return _isParrySuccess;
    }

    public void Dispose()
    {
        _inputHandler.OnParry -= StartParry;
        _inputDisposer?.Dispose();
    }
}

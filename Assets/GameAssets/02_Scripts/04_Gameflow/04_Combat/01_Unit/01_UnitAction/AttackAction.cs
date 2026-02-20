using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Threading;
using UnityEngine;

sealed class SelfAttackAction : BaseUnitAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.NONE;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override int SPCost => 0;
    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var selfAttackEvent = new SelfAttackEvent();
        
        var cc = context.Caster.CCUnit.GetNonStackCC(ELEMENT_TYPE.VOID);
        var attackIncreaseRate = selfAttackEvent.CalculateAttackIncreaseRate(cc);
        var modifer = selfAttackEvent.CreateTemporaryModifier(attackIncreaseRate);

        context.Caster.GetStat().ModifierStat.Mediator.AddModifier(modifer);

        selfAttackEvent.DamageEvent<DamageCalculatorStrange>(context.Caster, context.Caster);

        modifer.Dispose();
    }
}

abstract class BaseAttackAction : BaseUnitAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; }
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override int SPCost => 1;
    public BaseAttackAction(SIDE side) { Target_Type = side; }
}

class MeleeAttackAction : BaseAttackAction
{
    public MeleeAttackAction(SIDE side) : base(side) { }
    
    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var input = context.InputHandler;
        var inputDisposer = new InputDisposer(input, InputHandler.InputState.Parry);
        var meleeAttackEvent = new MeleeAttackEvent();
        
        CancellationTokenSource cts = new ();
        
        Action onParry = () => cts.Cancel();
        input.OnParry += onParry;
        
        var ct = cts.Token;
        
        meleeAttackEvent.CalculateMovePositions(context.Caster, context.Target);

        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.MOVE,
            () => { meleeAttackEvent.Move(context.Caster, isRetreat: false); });

        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.ATTACK,
            () =>
            {
                meleeAttackEvent.DamageEvent_Element<DamageCalculatorNormal, ElementGaugeCalculator>(context.Caster,
                    context.Target);
            });
        
        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.MOVE,
            () => { meleeAttackEvent.Move(context.Caster, isRetreat: true); });
        
        try
        {
            await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.MOVE);
            ct.ThrowIfCancellationRequested();

            await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.ATTACK);
            ct.ThrowIfCancellationRequested();
        }
        catch (OperationCanceledException)
        {
            Debug.Log(context.Caster.name + "'s Attack is Parried!");
            
            //await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.PARRY);
        }
        finally
        {
            await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.RETREAT);
            context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
            context.Caster.AnimationEventHandler.ClearAnimationEvent();
            
            input.OnParry -= onParry;
            inputDisposer.Dispose();
        }
    }
}

class RangeAttackAction : BaseAttackAction
{
    public RangeAttackAction(SIDE side) : base(side) { }

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var input = context.InputHandler;
        var inputDisposer = new InputDisposer(input, InputHandler.InputState.Parry);
        var rangeAttackEvent = new RangeAttackEvent();
        
        CancellationTokenSource cts = new ();
        
        Action onParry = () => cts.Cancel();
        input.OnParry += onParry;
        
        bool isDamaged = false;

        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.ATTACK, () =>
        {
            rangeAttackEvent.ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName, context.Caster, context.Target, () => isDamaged = true);
        });

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.ATTACK);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isDamaged);

        inputDisposer.Dispose();
    }
}
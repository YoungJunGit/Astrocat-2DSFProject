using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using DG.Tweening;
using Language.Lua;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public interface ISkillAction
{
    public SkillData Data { get; }
    public void SetData(SkillData data);
}

public abstract class SkillAction<TContext> : BaseUnitAction<TContext>, ISkillAction where TContext : IUnitActionContext
{
    public SkillData Data => _data;
    private SkillData _data;
    public void SetData(SkillData data) => _data = data;
    public override int SPCost => -_data.Skill_Cost_SP;
    public abstract string SkillName { get; }
}

public sealed class Skill_Taunt : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "Taunt";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Target.GetStat().ModifierStat.SetTaunt(Data.Skill_Duration_Turn, context.Caster.GetStat().CoreStat.ID);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        await context.TextManager.ShowTauntText(context.Target);
    }
}

public sealed class Skill_AreaBurst : SkillAction<IMultiTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.ALL;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "AreaBurst";

    public override async UniTask AsyncOperateAction(IMultiTargetContext context)
    {
        var @event = new AreaBurstEvent();

        int count = 0;
        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
            @event.ExecuteShootBullet(Data, context, () => count++);
        });

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => count == context.Targets.Count);
    }
}

public sealed class Skill_TripleBurst : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "TripleBurst";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var @event = new TripleBurstEvent();

        bool isLastDamaged = false;
        for(int i = 0; i < Data.Skill_Hit_Count; i++)
        {   
            Action onLastDamaged = null;
            if(i == Data.Skill_Hit_Count - 1) onLastDamaged = () => isLastDamaged = true;
            context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
            {
                @event.ExecuteShootBullet(Data, context, onLastDamaged);
            });
        }

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isLastDamaged);
    }
}

public sealed class Skill_FirstAid : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.PLAYER;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "FirstAid";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        context.SoundService.PlayEffectSound("HealEffectSound");
        var @event = new CommonActionEvent();

        @event.SkillHealEvent<HealCalculatorPercentageHealth>(context.Caster, context.Target, (float)Data.Skill_ATK_Rate);
        var effect = await context.EffectManager.PlayEffectAsync("RecoveryEffect", context.Target.transform);

        if (effect != null)
            Object.Destroy(effect.gameObject);
    }
}

public sealed class Skill_ForceSuppression : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.RANDOM;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "ForceSuppression";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var @event = new ForceSuppressionEvent();

        bool isLastDamaged = false;
        for (int i = 0; i < Data.Skill_Hit_Count; i++)
        {
            Action onLastDamaged = null;
            if (i == Data.Skill_Hit_Count - 1) onLastDamaged = () => isLastDamaged = true;
            context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
            {
                @event.ExecuteShootBullet(Data, context, onLastDamaged);
            });
        }

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isLastDamaged);
    }
}

public sealed class Skill_SPSupply : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.PLAYER;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "SPSupply";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
                context.Target.AddSP((int)Data.Skill_ATK_Rate);
        });

        BaseEffect effect = null;
        context.EffectManager.PlayEffectAsync("SPSupplyEffect", context.Target.transform).ContinueWith(result => effect = result).Forget();
        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();

        if (effect != null)
            Object.Destroy(effect.gameObject);
    }
}

public sealed class Skill_ContaminationShot : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "ContaminationShot";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var @event = new ContaminationShotEvent();

        bool isDamaged = false;

        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
            @event.ExecuteShootBullet(Data, SkillName, context, () => isDamaged = true);
        });

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isDamaged);
    }
}

public sealed class Skill_RecoveryProtocol : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.PLAYER;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "RecoveryProtocol";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        context.SoundService.PlayEffectSound("HealEffectSound");
        var @event = new CommonActionEvent();

        @event.SkillHealEvent<HealCalculatorPercentageHealth>(context.Caster, context.Target, (float)Data.Skill_ATK_Rate);
        var effect = await context.EffectManager.PlayEffectAsync("RecoveryEffect", context.Target.transform);

        if(effect != null)
            Object.Destroy(effect.gameObject);
    }
}

public sealed class Skill_PrecisionShot : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "PrecisionShot";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var @event = new PrecisionShotEvent();

        bool isDamaged = false;

        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
            @event.ExecuteShootBullet(Data, SkillName, context, () => isDamaged = true);
        });

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isDamaged);
    }
}

public sealed class Skill_NanoRestore : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "NanoRestore";

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var damageEvent = new NanoRestoreEvent();

        bool isDamaged = false;
        float damageValue = 0.0f;
        context.Caster.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
            damageEvent.ExecuteShootBullet(Data, context, (result) => 
            {
                damageValue = result;
                isDamaged = true;
            });
        });

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isDamaged);

        context.SoundService.PlayEffectSound("HealEffectSound");
        var healEvent = new CommonActionEvent();

        healEvent.SkillHealEvent<HealCalculatorPercentageAttack>(context.Caster, context.Caster, (float)Data.Skill_ATK_Rate, damageValue);
        var effect = await context.EffectManager.PlayEffectAsync("RecoveryEffect", context.Caster.transform);

        if (effect != null)
            Object.Destroy(effect.gameObject);
    }
}

public sealed class Skill_PowerBooster : SkillAction<ISingleTargetContext>
{
    public override SIDE Target_Type { get; } = SIDE.PLAYER;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;
    public override string SkillName => "PowerBooster";

    private float moveDuration = 5.0f;

    public override async UniTask AsyncOperateAction(ISingleTargetContext context)
    {
        var buffEvent = new CommonActionEvent();
        var effectInfo = new EffectInfo($"{SkillName}_Attack", (float)Data.Skill_ATK_Rate, Data.Skill_Duration_Turn);
        var effectContext = new EffectContext(Target: context.Target, Caster: context.Caster);
        var @event = new PowerBoosterEvent();

        bool isInjected = false;
        context.Caster.SupporterUnit.AnimationEventHandler.AddAnimationEvent(ANIMATION_EVENT.SKILL, () =>
        {
            @event.FireInjection
            (
                context.ProjectileManager,
                context,
                SkillName,
                () => buffEvent.SkillBuffEvent(context, effectInfo, effectContext, "player_Sniper_PowerBooster"),
                () => isInjected = true
            );
        });

        @event.CalculateMovePositions(context.Caster.SupporterUnit.transform, context.Target.Attachments.GetDroneInjectionFiringPos().transform);
        await context.Caster.SupporterUnit.AnimationHandler.ChangeAnimationAsync(ANIMATION.MOVE, fadeTime: 0.1f);
        await @event.Move(context.Caster.SupporterUnit.transform, moveDuration, false);
        await context.Caster.SupporterUnit.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName, fadeTime: 0.25f);
        await context.Caster.SupporterUnit.AnimationHandler.ChangeAnimationAsync(ANIMATION.MOVE, fadeTime: 0.1f);
        await @event.Move(context.Caster.SupporterUnit.transform, moveDuration, true);
        await context.Caster.SupporterUnit.AnimationHandler.ChangeAnimationAsync(ANIMATION.IDLE);

        await UniTask.WaitUntil(() => isInjected);
    }
}
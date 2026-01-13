using Cysharp.Threading.Tasks;
using DataEntity;
using DataEnum;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

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
            @event.ShootBullets((float)Data.Skill_ATK_Rate, context, () => count++);
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
                @event.ShootBullet(Data, context, onLastDamaged);
            });
        }

        await context.Caster.AnimationHandler.ChangeAnimationAsync(ANIMATION.SKILL, SkillName);
        context.Caster.AnimationHandler.ChangeAnimation(ANIMATION.IDLE);
        context.Caster.AnimationEventHandler.ClearAnimationEvent();
        await UniTask.WaitUntil(() => isLastDamaged);
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
        context.SoundService.PlayEffectSound("player_Rifleman_Skill_RecoveryProtocol");
        await context.EffectManager.PlayEffectAsync("RecoveryEffect", context.Target.transform);
    }
}
using DataEnum;
using System;
using UnityEngine;
using DG.Tweening;
using Object = UnityEngine.Object;
using DataEntity;
using Sirenix.OdinInspector.Modules.UnityMathematics.Editor;
using NUnit.Framework;
using Cysharp.Threading.Tasks;

public class CommonActionEvent
{
    public float DamageEvent<TCalculator>(BaseUnit caster, BaseUnit target, float skillRate = 1.0f)
        where TCalculator : IDamageCalculator, new()
    {
        DamageResult damage = DamageFactory.CreateDamage<DamageCalculatorNormal>(
            new DamageContext(
                caster, 
                target, 
                skillRate
            )
        );
        target.GetDamage(damage);
        return damage.DamageValue;
    }

    // Temp
    public void DamageEvent_Element<TDamageCalculator, TElementCalculator>(BaseUnit caster, BaseUnit target)
        where TDamageCalculator : IDamageCalculator, new()
        where TElementCalculator : IElementGaugeCalculator, new()
    {
        DamageResult damage = DamageFactory.CreateDamage<TDamageCalculator>(
            new DamageContext(
                caster, 
                target
            )
        );
        ElementGaugeResult elementGauge = ElementGaugeFactory.CreateElementGauge<ElementGaugeCalculator>(
            new ElementGaugeContext(
                caster, 
                target, 
                caster.My_Temp_Type, 
                SKILL_ELEMENT_RATE.STANDARD
            )
        );
        target.GetDamage(damage, elementGauge);
    }

    public float SkillDamageEvent<TDamageCalculator, TElementCalculator>(BaseUnit caster, BaseUnit target, ELEMENT_TYPE skillType, SKILL_ELEMENT_RATE rateType, float skillRate)
        where TDamageCalculator : IDamageCalculator, new()
        where TElementCalculator : IElementGaugeCalculator, new()
    {
        DamageResult damage = DamageFactory.CreateDamage<TDamageCalculator>(
            new DamageContext(
                caster,
                target,
                skillRate
            )
        );
        ElementGaugeResult elementGauge = ElementGaugeFactory.CreateElementGauge<ElementGaugeCalculator>(
            new ElementGaugeContext(
                caster,
                target,
                skillType,
                rateType
            )
        );
        target.GetDamage(damage, elementGauge);
        return damage.DamageValue;
    }

    public void SkillHealEvent<THealCalculator>(BaseUnit caster, BaseUnit target, float skillRate, float subValue = 0.0f) 
        where THealCalculator : IHealCalulator, new()
    {
        float heal = HealFactory.CreateHeal<THealCalculator>(
            new HealContext(
                caster,
                target,
                skillRate,
                subValue
            )
        );
        target.GetHeal(heal);
    }

    public void SkillBuffEvent(IUnitActionContext context, EffectInfo Info, EffectContext Context, string audioName)
    {
        context.SoundService.PlayEffectSound(audioName);
        context.CombatEffectManager.AddCombatEffect(Info.Name, Info, new EffectContext(Target: Context.Target, Caster: Context.Caster));
    }
}

public sealed class SelfAttackEvent : CommonActionEvent
{
    public float CalculateAttackIncreaseRate(ICrowdControl cc)
    {
        float attackIncreaseRate = cc is Corrode corrodeCC ?
            (float)cc.CCData.Element_Status_Value[1] * corrodeCC.Count :
            (float)cc.CCData.Element_Status_Value[1];

        return attackIncreaseRate;
    }

    public BasicStatModifier<float> CreateTemporaryModifier(float increaseRate)
    {
        return new BasicStatModifier<float>(BUFF_TYPE.ATTACK, UPDATE_TYPE.NONE, (v) => v + increaseRate);
    }
}


public sealed class MeleeAttackEvent : CommonActionEvent
{
    private Vector2 startPosition;
    private Vector2 endPosition;

    public void CalculateMovePositions(BaseUnit Caster, BaseUnit Target)
    {
        // Save Position
        startPosition = (Vector2)Caster.transform.position;

        // Identify target's postition
        float xOffset = Caster.Attachments.GetHitBox().size.x / 2;
        Vector2 offset = Caster is PlayerUnit ? new Vector2(-xOffset, 0f) : new Vector2(xOffset, 0f);
        endPosition = (Vector2)Target.Attachments.GetMeleeHitPos().position + offset;
    }

    public void Move(BaseUnit unit, bool isRetreat)
    {
        AnimationClip currentClip = unit.AnimationHandler.Anim.GetCurrentAnimatorClipInfo(0)[0].clip;
        AnimationEvent[] events = currentClip.events;

        AnimationEvent startEvent = Array.Find(events, element => element.functionName == "StartMovePosition");
        AnimationEvent endEvent = Array.Find(events, element => element.functionName == "EndMovePosition");

        if (startEvent != null && endEvent != null)
        {
            float duration = endEvent.time - startEvent.time;
            Vector2 pos = isRetreat ? startPosition : endPosition;
            unit.transform.DOMove(pos, duration).SetEase(Ease.InOutSine);
        }
    }
}

public class RangeAttackEvent : CommonActionEvent
{
    public void ShootBullet(IProjectileManager manager, string assetName, BaseUnit Caster, BaseUnit Target, Action onDamaged)
    {
        BaseBullet bullet = manager.CreateProjectile<BaseBullet>(assetName, Caster.Attachments.GetBulletSpawnPos().transform.position);

        if (bullet != null)
        {
            bullet.Initialize(Target.Attachments.GetHitBox(), () =>
            {
                DamageEvent_Element<DamageCalculatorNormal, ElementGaugeCalculator>(Caster, Target);
                onDamaged?.Invoke();
            });
        }
    }

    public void ShootBullet(IProjectileManager manager, string assetName, BaseUnit Caster, BaseUnit Target, Action onDamaged, SkillData data = null)
    {
        BaseBullet bullet = manager.CreateProjectile<BaseBullet>(assetName, Caster.Attachments.GetBulletSpawnPos().transform.position);

        Action damageAction;
        if(data.Element_Type == ELEMENT_TYPE.NONE)
        {
            damageAction = () => DamageEvent<DamageCalculatorNormal>(Caster, Target, (float)data.Skill_ATK_Rate);
        }
        else
        {
            damageAction = () => SkillDamageEvent<DamageCalculatorNormal, ElementGaugeCalculator>(Caster, Target, data.Element_Type, data.Skill_Element_Rate, (float)data.Skill_ATK_Rate);
        }

        if (bullet != null)
        {
            bullet.Initialize(Target.Attachments.GetHitBox(), () =>
            {
                damageAction();
                onDamaged?.Invoke();
            });
        }
    }

    public void ShootBulletReturnDamage(IProjectileManager manager, string assetName, BaseUnit Caster, BaseUnit Target, Action<float> onDamaged, SkillData data = null, float overrideSkillRate = -1f)
    {
        BaseBullet bullet = manager.CreateProjectile<BaseBullet>(assetName, Caster.Attachments.GetBulletSpawnPos().transform.position);

        Func<float> damageAction;
        if (data.Element_Type == ELEMENT_TYPE.NONE)
        {
            damageAction = () => 
            {
                float skillRate = overrideSkillRate <= 0f ? (float)data.Skill_ATK_Rate : overrideSkillRate;
                return DamageEvent<DamageCalculatorNormal>(Caster, Target, skillRate);
            };
        }
        else
        {
            damageAction = () => 
            {
                float skillRate = overrideSkillRate <= 0f ? (float)data.Skill_ATK_Rate : overrideSkillRate;
                return SkillDamageEvent<DamageCalculatorNormal, ElementGaugeCalculator>(Caster, Target, data.Element_Type, data.Skill_Element_Rate, skillRate);
            };
        }

        if (bullet != null)
        {
            bullet.Initialize(Target.Attachments.GetHitBox(), () =>
            {
                onDamaged?.Invoke(damageAction());
            });
        }
    }
}

public sealed class AreaBurstEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, IMultiTargetContext context, Action onDamaged)
    {
        foreach (var target in context.Targets)
        {
            ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName, context.Caster, target, onDamaged, data);
        }
    }
}

public sealed class TripleBurstEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, ISingleTargetContext context, Action onDamaged)
    {
        ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName, context.Caster, context.Target, onDamaged, data);
    }
}

public sealed class ForceSuppressionEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, ISingleTargetContext context, Action onDamaged)
    {
        ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName, context.Caster, context.Target, onDamaged, data);
    }
}

public sealed class ContaminationShotEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, string skillName, ISingleTargetContext context, Action onDamaged)
    {
        ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName + "_" + skillName, context.Caster, context.Target, onDamaged, data);
    }
}

public sealed class PrecisionShotEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, string skillName, ISingleTargetContext context, Action onDamaged)
    {
        ShootBullet(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName + "_" + skillName, context.Caster, context.Target, onDamaged, data);
    }
}

public sealed class NanoRestoreEvent : RangeAttackEvent
{
    public void ExecuteShootBullet(SkillData data, ISingleTargetContext context, Action<float> onDamaged)
    {
        ShootBulletReturnDamage(context.ProjectileManager, context.Caster.GetStat().CoreStat.AssetFileName, context.Caster, context.Target, onDamaged, data, 1.0f);
    }
}

public sealed class PowerBoosterEvent : CommonActionEvent
{
    private Vector2 startPosition;
    private Vector2 endPosition;

    private bool isRight = false;

    public void CalculateMovePositions(Transform start, Transform end)
    {
        // Save Position
        startPosition = start.position;
        endPosition = end.position;

        // Determine Rotation
        Transform basis = start.root;
        Vector3 localStart = basis.InverseTransformPoint(start.position);
        Vector3 localEnd = basis.InverseTransformPoint(end.position);
        isRight = localEnd.x >= localStart.x;
    }

    public async UniTask Move(Transform unit, float moveSpeed, bool isRetreat)
    {
        var scale = unit.localScale;
        scale.x = Mathf.Abs(scale.x) * (isRight ? 1 : -1);
        unit.localScale = scale;

        if (!isRetreat)
        {
            await unit.transform.DOMove(endPosition, moveSpeed)
                .SetSpeedBased()
                .SetEase(Ease.InOutSine);
        }
        else
        {
            await unit.transform.DOMove(startPosition, moveSpeed)
                .SetSpeedBased()
                .SetEase(Ease.InOutSine);
        }

        if(!isRight)
        {
            scale.x = 1;
            unit.localScale = scale;
        }
        
        isRight = !isRight;
    }

    public void FireInjection(IProjectileManager projectileManager, ISingleTargetContext context, string skillName, Action buffEvent, Action onFinished)
    {
        BaseInjection bullet = projectileManager.CreateProjectile<BaseInjection>
        (
            skillName,
            context.Caster.SupporterUnit.Attachments.GetBulletSpawnPos().transform.position
        );

        if (bullet != null)
        {
            bullet.Initialize(context.Target.Attachments.GetDroneInjectionBox(), buffEvent, onFinished);
        }
    }
}

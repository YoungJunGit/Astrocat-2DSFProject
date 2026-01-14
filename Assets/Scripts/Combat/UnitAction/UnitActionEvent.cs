using DataEnum;
using System;
using UnityEngine;
using DG.Tweening;
using Object = UnityEngine.Object;
using DataEntity;

public abstract class UnitActionEvent
{
    public void DamageEvent<TCalculator>(BaseUnit caster, BaseUnit target, float skillRate = 1.0f)
        where TCalculator : IDamageCalculator, new()
    {
        IDamageInfo damage = DamageFactory.CreateDamage<TCalculator>(caster, target, skillRate);
        target.GetDamage(damage);
    }

    // Temp
    public void DamageEvent_Element<TDamageCalculator, TElementCalculator>(BaseUnit caster, BaseUnit target)
        where TDamageCalculator : IDamageCalculator, new()
        where TElementCalculator : IElementGaugeCalculator, new()
    {
        IDamageInfo damage = DamageFactory.CreateDamage<TDamageCalculator, TElementCalculator>(caster, target, caster.My_Temp_Type, SKILL_ELEMENT_RATE.STANDARD, 1.0f);
        target.GetDamage(damage);
    }

    public void SkillDamageEvent<TDamageCalculator, TElementCalculator>(BaseUnit caster, BaseUnit target, ELEMENT_TYPE skillType, SKILL_ELEMENT_RATE rateType, float skillRate)
        where TDamageCalculator : IDamageCalculator, new()
        where TElementCalculator : IElementGaugeCalculator, new()
    {
        IDamageInfo damage = DamageFactory.CreateDamage<TDamageCalculator, TElementCalculator>(caster, target, skillType, rateType, skillRate);
        target.GetDamage(damage);
    }
}

public sealed class SelfAttackEvent : UnitActionEvent
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


public sealed class MeleeAttackEvent : UnitActionEvent
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

public sealed class RangeAttackEvent : UnitActionEvent
{
    public void ShootBullet(ISingleTargetContext context, Action onDamaged)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().CoreStat.AssetFileName);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.Attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        if (bullet != null)
        {
            bullet.Initialize(context.Target.Attachments.GetHitBox(), () =>
            {
                DamageEvent_Element<NormalDamageCalculator, NormalElementGaugeCalculator>(context.Caster, context.Target);
                onDamaged();
            });
        }
    }
}

public sealed class AreaBurstEvent : UnitActionEvent
{
    public void ShootBullets(float skillRate, IMultiTargetContext context, Action onDamaged)
    {
        foreach (var target in context.Targets)
        {
            GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().CoreStat.AssetFileName);
            BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.Attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
            if (bullet != null)
            {
                bullet.Initialize(target.Attachments.GetHitBox(), () =>
                {
                    DamageEvent<NormalDamageCalculator>(context.Caster, target, skillRate);
                    onDamaged();
                });
            }
        }
    }
}

public sealed class TripleBurstEvent : UnitActionEvent
{
    public void ShootBullet(SkillData data, ISingleTargetContext context, Action onDamaged)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().CoreStat.AssetFileName);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.Attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        if (bullet != null)
        {
            bullet.Initialize(context.Target.Attachments.GetHitBox(), () =>
            {
                SkillDamageEvent<NormalDamageCalculator, NormalElementGaugeCalculator>(context.Caster, context.Target, data.Element_Type, data.Skill_Element_Rate, (float)data.Skill_ATK_Rate);
                onDamaged?.Invoke();
            });
        }
    }
}
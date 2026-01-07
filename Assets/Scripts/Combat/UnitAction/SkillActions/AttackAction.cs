using Cysharp.Threading.Tasks;
using DataEnum;
using Michsky.UI.Shift;
using NUnit.Framework;
using R3;
using System;
using System.Threading;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

class SelfAttackAction : IUnitAction
{
    public SIDE Target_Type { get; }
    public TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public Func<BaseUnit, bool> Target_Filter { get; } = null;
    public async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        await unitAction.ShowSelfAttackMessage(context);

        var cc = context.Caster.CCUnit.GetNonStackCC(ELEMENT_TYPE.VOID);

        float attackIncreaseRate = cc is Corrode corrodeCC
            ? (float)cc.CCData.Element_Status_Value[1] * corrodeCC.Count
            : (float)cc.CCData.Element_Status_Value[1];

        Debug.Log(attackIncreaseRate);
        var modifer = new BasicStatModifier<float>(BUFF_TYPE.ATTACK, UPDATE_TYPE.NONE, (v) => v + attackIncreaseRate);
        context.Caster.GetStat().ModifierStat.Mediator.AddModifier(modifer);

        IDamageInfo damage = DamageFactory.CreateDamage<StrangeDamageCalculator>(context.Caster, context.Caster);
        context.Caster.GetDamage(damage);

        modifer.Dispose();
    }
}

class BaseAttackAction : IUnitAction
{
    public SIDE Target_Type { get; }
    public virtual TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;

    public BaseAttackAction(SIDE side) { Target_Type = side; }

    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        context.Caster.OnAttack();
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.CombatInfo.isFinishedAction, cancellationToken: cancellationToken.Token);
        Debug.Log($"{context.Caster.GetStat().CoreStat.Name} : Action was finished.");
    }
}

class MeleeAttackAction : BaseAttackAction
{
    public MeleeAttackAction(SIDE side) : base(side) { }

    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        if (context.Caster is EnemyUnit)
            await unitAction.ShowAttackWarningMessage(context);

        if (unitAction.TryGetSingle(context, out var target))
        {
            var inputDisposer = new InputDisposer(context.InputHandler, InputHandler.InputState.Parry);

            // Save Position
            context.Caster.CombatInfo.startPos = (Vector2)context.Caster.transform.position;

            // Identify target's postition
            float xOffset = context.Caster.Attachments.GetHitBox().size.x / 2;
            Vector2 offset = context.Caster is PlayerUnit ? new Vector2(-xOffset, 0f) : new Vector2(xOffset, 0f);
            context.Caster.CombatInfo.targetPos = (Vector2)target.Attachments.GetMeleeHitPos().position + offset;

            context.Caster.GetAnimationHandler().Attack += () => { unitAction.DamageEvent(context.Caster, target); };
            context.Caster.CombatInfo.actionList.Add("FinishedAction", () => { unitAction.OnFinishedAction(context); });
            context.Caster.GetAnimationHandler().ChangeAnimation(ANIMATION.MOVE);

            try
            {
                await base.Execute(context, unitAction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"{context.Caster.GetStat().CoreStat.Name} : Action was canceled.");
            }

            inputDisposer.Dispose();
        }
    }
}

class RangeAttackAction : BaseAttackAction 
{
    public RangeAttackAction(SIDE side) : base(side) { }

    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        if (context.Caster is EnemyUnit)
            await unitAction.ShowAttackWarningMessage(context);

        if (unitAction.TryGetSingle(context, out var target))
        {
            BaseBullet bullet = null;
            context.Caster.GetAnimationHandler().Attack += () => { bullet = ShootBullet(context, unitAction, target); };
            context.Caster.GetAnimationHandler().ChangeAnimation(ANIMATION.ATTACK);

            try
            {
                await base.Execute(context, unitAction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                bullet?.Dispose();
                Debug.Log($"{context.Caster.GetStat().CoreStat.Name} : Action was canceled.");
            }
        }
    }

    private BaseBullet ShootBullet(IUnitActionContext context, IUnitActionEvent unitAction, BaseUnit target)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().CoreStat.AssetFileName);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.Attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        if (bullet != null)
        {
            bullet.Initialize(target.Attachments.GetHitBox(), () => { unitAction.DamageEvent(context.Caster, target); unitAction.OnFinishedAction(context); });
            return bullet;
        }
        return null;
    }
}
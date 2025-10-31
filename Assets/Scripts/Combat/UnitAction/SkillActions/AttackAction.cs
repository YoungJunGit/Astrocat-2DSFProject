using Cysharp.Threading.Tasks;
using DataEnum;
using DataHashAnim;
using Michsky.UI.Shift;
using NUnit.Framework;
using System;
using System.Threading;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

class BaseAttackAction : IUnitAction
{
    public SIDE Target_Type { get; }
    public virtual ACTION_TARGET_TYPE Action_Type { get; } = ACTION_TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;

    public BaseAttackAction(SIDE side) { Target_Type = side; }

    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction, cancellationToken: cancellationToken.Token);
        Debug.Log($"{context.Caster.GetStat().Name} : Action was finished.");
    }
}

class MeleeAttack : BaseAttackAction
{
    public MeleeAttack(SIDE side) : base(side) { }

    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        if (context.Caster is EnemyUnit)
            await unitAction.ShowAttackMessage(context);

        BaseUnit target;
        if (unitAction.TryGetSingle(context, out target))
        {
            var inputDisposer = new InputDisposer(context.InputHandler, InputHandler.InputState.Parry);

            // Save Position
            context.Caster.combatInfo.startPos = (Vector2)context.Caster.transform.position;

            // Identify target's postition
            float xOffset = context.Caster.attachments.GetHitBox().size.x / 2;
            Vector2 offset = context.Caster is PlayerUnit ? new Vector2(xOffset, 0f) : new Vector2(-xOffset, 0f);
            context.Caster.combatInfo.targetPos = (Vector2)target.attachments.GetMeleeHitPos().position + offset;

            context.Caster.GetAnimationHandler().Attack += () => { unitAction.DamageEvent(context, target); };
            context.Caster.combatInfo.actionList.Add("FinishedAction", () => { unitAction.OnFinishedAction(context); });
            context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.MOVE);

            try
            {
                await base.Execute(context, unitAction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"{context.Caster.GetStat().Name} : Action was canceled.");
            }

            inputDisposer.Dispose();
        }
    }
}

class RangeAttack : BaseAttackAction 
{
    public RangeAttack(SIDE side) : base(side) { }

    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        if (context.Caster is EnemyUnit)
            await unitAction.ShowAttackMessage(context);

        BaseUnit target;
        if (unitAction.TryGetSingle(context, out target))
        {
            BaseBullet bullet = null;
            context.Caster.GetAnimationHandler().Attack += () => { bullet = ShootBullet(context, unitAction, target); };
            context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.ATTACK);

            try
            {
                await base.Execute(context, unitAction, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                bullet?.Dispose();
                Debug.Log($"{context.Caster.GetStat().Name} : Action was canceled.");
            }
        }
    }

    private BaseBullet ShootBullet(IUnitActionContext context, IUnitActionEvent unitAction, BaseUnit target)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().AssetFileName);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        if (bullet != null)
        {
            context.SoundService.PlayEffectSound("Player_Shoot");
            bullet.Initialize(target.attachments.GetHitBox(), () => { unitAction.DamageEvent(context, target); unitAction.OnFinishedAction(context); });
            return bullet;
        }
        return null;
    }
}
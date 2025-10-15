using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataHashAnim;
using Michsky.UI.Shift;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

class BaseAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction, cancellationToken: cancellationToken.Token);
        Debug.Log($"{context.Caster.GetStat().Name} : Action was finished.");
    }
}

class MeleeAttack : BaseAttackAction
{
    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        var inputDisposer = new InputDisposer(context.InputHandler, InputHandler.InputState.Parry);

        // Save Position
        context.Caster.combatInfo.startPos = (Vector2)context.Caster.transform.position;

        // Identify target's postition
        float xOffset = context.Caster.attachments.GetHitBox().size.x / 2;
        Vector2 offset = context.Caster is PlayerUnit ? new Vector2(xOffset, 0f) : new Vector2(-xOffset, 0f);
        context.Caster.combatInfo.targetPos = (Vector2)context.unitManager.SelectedUnit.attachments.GetMeleeHitPos().position + offset;

        context.Caster.GetAnimationHandler().attack += () => { unitAction.DamageEvent(context); };
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

class RangeAttack : BaseAttackAction 
{
    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        BaseBullet bullet = null;
        context.Caster.GetAnimationHandler().attack += () => { bullet = ShootBullet(context, unitAction); };
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

    private BaseBullet ShootBullet(IUnitActionContext context, IUnitActionEvent unitAction)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().GetData().Asset_File);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(context.unitManager.SelectedUnit.attachments.GetHitBox(), () => { unitAction.DamageEvent(context); unitAction.OnFinishedAction(context); });
        return bullet;
    }
}
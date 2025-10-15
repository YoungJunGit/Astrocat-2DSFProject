using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DataHashAnim;
using Michsky.UI.Shift;
using UnityEngine;
using Object = UnityEngine.Object;

class BaseAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, CancellationTokenSource cancellationToken = default)
    {
        context.OnStartAction();
        
        Debug.Log($"{context.Caster.GetStat().Name} : Action was finished.");
    }
}

class MeleeAttack : BaseAttackAction
{
    public override async UniTask Execute(IUnitActionContext context, CancellationTokenSource cancellationToken = default)
    {
        var inputDisposer = new InputDisposer(context.InputHandler, InputHandler.InputState.Parry);
        
        try
        {
            context.Caster.GetAnimationHandler().attack += context.DamageEvent;
            context.Caster.combatInfo.actionList.Add("FinishedAction", context.OnFinishedAction);
            context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.MOVE);
            
            await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction, cancellationToken: cancellationToken.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"{context.Caster.GetStat().Name} : Action was canceled.");
        }
        
        inputDisposer.Dispose();
        

        await base.Execute(context, cancellationToken);
    }
}

class RangeAttack : BaseAttackAction 
{
    public override async UniTask Execute(IUnitActionContext context, CancellationTokenSource cancellationToken = default)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().GetData().Asset_File);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(context.unitManager.SelectedUnit.attachments.GetHitBox(), () => { context.DamageEvent(); context.OnFinishedAction(); });
        
        context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.ATTACK);
        
        context.Caster.GetAnimationHandler().attack += async () =>
        {
            try
            {
                await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction, cancellationToken: cancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
                bullet.Dispose();
                Debug.Log($"{context.Caster.GetStat().Name} : Action was canceled.");
            }
        };
        
        await base.Execute(context);
    }
}
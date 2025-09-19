
using Cysharp.Threading.Tasks;
using DataHashAnim;
using Michsky.UI.Shift;
using UnityEngine;

class BaseAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context)
    {
        context.OnStartAction();

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}

class MeleeAttack : BaseAttackAction
{
    public override async UniTask Execute(IUnitActionContext context)
    {
        context.Caster.GetAnimationHandler().attack += context.DamageEvent;

        // Save Position
        context.Caster.combatInfo.startPos = (Vector2)context.Caster.transform.position;

        // Identify target's postition
        float xOffset = context.Caster.attachments.GetHitBox().size.x / 2;
        Vector2 offset = context.Caster is PlayerUnit ? new Vector2(xOffset, 0f) : new Vector2(-xOffset, 0f);
        context.Caster.combatInfo.targetPos = (Vector2)context.unitManager.SelectedUnit.attachments.GetMeleeHitPos().position + offset;

        context.Caster.combatInfo.actionList.Add("FinishedAction", context.OnFinishedAction);
        context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.MOVE);

        await base.Execute(context);
    }
}

class RangeAttack : BaseAttackAction 
{
    public override async UniTask Execute(IUnitActionContext context)
    {
        context.Caster.GetAnimationHandler().attack += () => { ShootBullet(context); };
        context.Caster.GetAnimationHandler().ChangeAnimation(AnimCombat.ATTACK);

        await base.Execute(context);
    }

    private void ShootBullet(IUnitActionContext context)
    {
        GameObject bulletPrefab = AssetLoader.LoadBulletPrefabAsset(context.Caster.GetStat().GetData().Asset_File);
        BaseBullet bullet = Object.Instantiate(bulletPrefab, context.Caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(context.unitManager.SelectedUnit.attachments.GetHitBox(), () => { context.DamageEvent(); context.OnFinishedAction(); });
    }
}
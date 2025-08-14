using Cysharp.Threading.Tasks;
using System;
using DataEnum;
using DataHashAnim;
using UnityEngine;

interface IUnitAction
{
    public UniTask Execute();
}

class BaseAttackAction : IUnitAction
{
    protected BaseUnit _caster;
    protected BaseUnit _target;

    public BaseAttackAction(BaseUnit caster, BaseUnit target)
    {
        _caster = caster;
        _target = target;
    }
    
    public virtual async UniTask Execute()
    {
        _caster.combatInfo.isFinishedAction = false;

        await UniTask.WaitUntil(() => _caster.combatInfo.isFinishedAction);
    }

    protected void DamageEvent()
    {
        _target.GetStat().GetDamaged((float)_caster.GetStat().GetData().Default_Attack);
    }

    protected void FinishedAction()
    {
        _caster.combatInfo.isFinishedAction = true;
    }
}

class MeleeAttack : BaseAttackAction
{
    public MeleeAttack(BaseUnit caster, BaseUnit target) : base(caster, target) { }

    public override async UniTask Execute()
    {
        _caster.animHandler.attack += DamageEvent;

        // Save Position
        _caster.combatInfo.startPos = (Vector2)_caster.transform.position;

        // Identify target's postition
        float xOffset = _caster.attachments.GetHitBox().size.x + _target.attachments.GetHitBox().size.x;
        Vector2 offset = _caster is PlayerUnit ? new Vector2(xOffset, 0f) : new Vector2(-xOffset, 0f);
        _caster.combatInfo.targetPos = (Vector2)_target.transform.position + offset;

        _caster.combatInfo.actionList.Add("FinishedAction", FinishedAction);
        _caster.animHandler.ChangeAnimation(AnimCombat.MOVE);

        await base.Execute();
    }
}

class RangeAttack : BaseAttackAction 
{
    private GameObject bulletPrefab;
    public RangeAttack(BaseUnit caster, BaseUnit target) : base(caster, target) 
    {
        bulletPrefab = AssetLoader.LoadBulletPrefabAsset(caster.GetStat().GetData().Asset_File);
    }

    public override async UniTask Execute()
    {
        _caster.animHandler.attack += ShootBullet;
        _caster.animHandler.ChangeAnimation(AnimCombat.ATTACK);

        await base.Execute();
    }

    private void ShootBullet()
    {
        BaseBullet bullet = UnityEngine.Object.Instantiate(bulletPrefab, _caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(_target.attachments.GetHitBox(), () => { DamageEvent(); FinishedAction(); });
    }
}
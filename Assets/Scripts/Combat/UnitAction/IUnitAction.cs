using Cysharp.Threading.Tasks;
using System.Net.Mail;
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
        if (_caster == null || _target == null)
        {
            Debug.LogError("Caster or target is not set.");
            return;
        }

        _caster.Attack(_target);
    }

    protected void DamageEvent()
    {
        _target.GetStat().GetDamaged((float)_caster.GetStat().GetData().Default_Attack);
    }
}

class MeleeAttack : BaseAttackAction
{
    public MeleeAttack(BaseUnit caster, BaseUnit target) : base(caster, target) { }

    public override async UniTask Execute()
    {
        base.Execute();
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
        _caster.animEventHandler.attack += ShootBullet;
        base.Execute();
    }

    public void ShootBullet()
    {
        BaseBullet bullet = Object.Instantiate(bulletPrefab, _caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(_target.attachments.GetHitBox(), DamageEvent);
    }
}
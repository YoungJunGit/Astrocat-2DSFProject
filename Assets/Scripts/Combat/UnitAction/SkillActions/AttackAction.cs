
using Cysharp.Threading.Tasks;
using DataHashAnim;
using UnityEngine;

class BaseAttackAction : IUnitAction
{
    protected BaseUnit _caster;
    protected BaseUnit _target;

    public BaseAttackAction(BaseUnit caster, BaseUnit target)
    {
        _caster = caster;
        _target = target;
    }
    
    public virtual async UniTask Execute(IUnitActionContext context)
    {
        _caster.combatInfo.isFinishedAction = false;
        _caster.attachments.GetSpriteRenderer().sortingLayerName = "Actor";

        await UniTask.WaitUntil(() => _caster.combatInfo.isFinishedAction);
    }

    protected void DamageEvent()
    {
        DamageContainer damage = AssetLoader.GetDamageFactory().CreateNormalDamage((float)_caster.GetStat().GetData().Default_Attack, _target.attachments.GetHitBox().bounds);
        _target.GetStat().GetDamaged(damage.Value, damage.Critical);
    }

    protected void FinishedAction()
    {
        _caster.combatInfo.isFinishedAction = true;
        _caster.attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }
}

class MeleeAttack : BaseAttackAction
{
    public MeleeAttack(BaseUnit caster, BaseUnit target) : base(caster, target) { }

    public override async UniTask Execute(IUnitActionContext context)
    {
        _caster.mainAnimHandler.attack += DamageEvent;

        // Save Position
        _caster.combatInfo.startPos = (Vector2)_caster.transform.position;

        // Identify target's postition
        float xOffset = _caster.attachments.GetHitBox().size.x / 2;
        Vector2 offset = _caster is PlayerUnit ? new Vector2(xOffset, 0f) : new Vector2(-xOffset, 0f);
        _caster.combatInfo.targetPos = (Vector2)_target.attachments.GetMeleeHitPos().position + offset;

        _caster.combatInfo.actionList.Add("FinishedAction", FinishedAction);
        _caster.mainAnimHandler.ChangeAnimation(AnimCombat.MOVE);

        await base.Execute(context);
    }
}

class RangeAttack : BaseAttackAction 
{
    private GameObject bulletPrefab;
    public RangeAttack(BaseUnit caster, BaseUnit target) : base(caster, target) 
    {
        bulletPrefab = AssetLoader.LoadBulletPrefabAsset(caster.GetStat().GetData().Asset_File);
    }

    public override async UniTask Execute(IUnitActionContext context)
    {
        _caster.mainAnimHandler.attack += ShootBullet;
        _caster.mainAnimHandler.ChangeAnimation(AnimCombat.ATTACK);

        await base.Execute(context);
    }

    private void ShootBullet()
    {
        BaseBullet bullet = UnityEngine.Object.Instantiate(bulletPrefab, _caster.attachments.GetBulletSpawnPos().transform.position, Quaternion.identity).GetComponent<BaseBullet>();
        bullet.Initialize(_target.attachments.GetHitBox(), () => { DamageEvent(); FinishedAction(); });
    }
}
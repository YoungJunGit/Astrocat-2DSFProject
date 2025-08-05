using UnityEngine;

public class PlayerUnit_Range : PlayerUnit, IRange
{
    [SerializeField] private BaseBullet BulletPrefab;
    public void ShootBullet(BaseUnit unit)
    {
        BaseBullet bullet = Instantiate(BulletPrefab, attachments.GetBulletSpawnPos().transform.position, Quaternion.identity);
        bullet.Initialize(unit.attachments.GetHitBoxPosition(), GetStat().Priority - unit.GetStat().Priority);
    }

    public override void Attack(BaseUnit unit)
    {
        animEventHandler.attack += () => ShootBullet(unit);
        base.Attack(unit);
    }
}

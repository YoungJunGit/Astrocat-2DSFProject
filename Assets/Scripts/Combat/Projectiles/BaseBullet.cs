using NaughtyAttributes;
using UnityEngine;
using System;

public class BaseBullet : BaseProjectile, IDisposable
{
    private Collider2D targetCollider;
    private Action damage = delegate { };

    public void Initialize(Collider2D collider, Action damage)
    {
        targetCollider = collider;
        this.damage = damage;

        SetVelocity(collider.transform);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetCollider == collision && collision.tag == Setting.TargetTag)
        {
            damage.Invoke();
            Dispose();
        }
    }

    public override void Dispose()
    {
        Destroy(gameObject);
    }
}

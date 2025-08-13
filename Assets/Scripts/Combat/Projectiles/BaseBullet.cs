using NaughtyAttributes;
using UnityEngine;
using System;

public class BaseBullet : MonoBehaviour
{
    [SerializeField, Expandable] 
    private BulletSetting setting;

    private Collider2D targetCollider;
    private Action damage;

    public void Initialize(Collider2D collider, Action damage)
    {
        targetCollider = collider;
        this.damage = damage;

        // Look at target
        Vector2 pos = (Vector2)collider.transform.position - (Vector2)transform.position;
        float rotZ = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Set Velocity
        Vector2 normalizePos = pos.normalized;
        GetComponent<Rigidbody2D>().linearVelocity = normalizePos * setting.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetCollider == collision && collision.tag == setting.TargetTag)
        {
            damage();
            Destroy(gameObject);
        }
    }
}

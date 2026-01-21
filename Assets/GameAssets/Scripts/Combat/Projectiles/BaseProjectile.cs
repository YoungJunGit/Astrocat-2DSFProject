using System;
using NaughtyAttributes;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour, IDisposable
{
    [SerializeField, Expandable]
    protected BulletSetting Setting;

    public virtual void SetVelocity(Transform target)
    {
        Vector2 pos = (Vector2)target.position - (Vector2)transform.position;
        float rotZ = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Set Velocity
        Vector2 normalizePos = pos.normalized;
        GetComponent<Rigidbody2D>().linearVelocity = normalizePos * Setting.Speed;
    }

    public abstract void Dispose();
}
using NaughtyAttributes;
using UnityEngine;

public class BaseBullet : MonoBehaviour
{
    [SerializeField, Expandable] 
    private BulletSetting setting;

    private Transform targetTransform;

    public void Initialize(Transform tr, int depth)
    {
        targetTransform = tr;

        // Look at target
        Vector2 pos = (Vector2)tr.position - (Vector2)transform.position;
        float rotZ = Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        // Set Velocity
        Vector2 normalizePos = ((Vector2)tr.position - (Vector2)transform.position).normalized;
        GetComponent<Rigidbody2D>().linearVelocity = normalizePos * setting.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetTransform == collision.GetComponentInParent<Transform>() && collision.tag == setting.TargetTag)
        {
            Destroy(gameObject);
        }
    }
}

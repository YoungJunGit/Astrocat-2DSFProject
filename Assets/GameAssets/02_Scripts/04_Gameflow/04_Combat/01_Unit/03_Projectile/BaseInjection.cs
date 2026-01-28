using UnityEngine;
using System;
using DG.Tweening;

public class BaseInjection : BaseProjectile, IDisposable
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float fadeDuration = 0.5f;

    private Collider2D targetCollider;
    private Action onInjected = delegate { };
    private Action onFinished = delegate { };

    public void Initialize(Collider2D collider, Action onInjected, Action onFinished)
    {
        targetCollider = collider;
        this.onInjected = onInjected;
        this.onFinished = onFinished;

        SetVelocity(collider.transform);
    }

    public override void SetVelocity(Transform target)
    {
        GetComponent<Rigidbody2D>().linearVelocity = (Vector2)transform.right * Setting.Speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (targetCollider == collision && collision.tag == Setting.TargetTag)
        {
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            animator.SetTrigger("bArrived");
        }
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void OnInjected()
    {
        onInjected.Invoke();
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void OnFalling()
    {
        spriteRenderer.sortingLayerName = "Bullet";
    }

    /// <summary>
    /// Animation Event
    /// </summary>
    public void OnAnimationFinished()
    {
        spriteRenderer.DOFade(0.0f, fadeDuration)
            .SetEase(Ease.Linear)
            .OnComplete(
                () => {
                    onFinished.Invoke();
                    Dispose();
                }
            );
    }

    public override void Dispose()
    {
        Destroy(gameObject);
    }
}
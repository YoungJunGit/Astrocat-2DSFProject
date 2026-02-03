using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField, Required]
    private SpriteRenderer spriteRenderer;

    [SerializeField, Required, HideIf("IsSupporterUnit")]
    private Transform collidersRoot;

    [SerializeField, Required]
    private Transform positionsRoot;

    private Dictionary<AttachType, Component> _cache;

    public void Init()
    {
        _cache = new();

        if (spriteRenderer != null)
            _cache[AttachType.SpriteRenderer] = spriteRenderer;

        if (collidersRoot != null)
            CacheFromRoot<Collider2D>(collidersRoot);

        if (positionsRoot != null)
            CacheFromRoot<Transform>(positionsRoot);
    }

    private void CacheFromRoot<T>(Transform root) where T : Component
    {
        foreach (var attachment in root.GetComponentsInChildren<UnitAttachment>(true))
        {
            if (attachment.TryGetComponent<T>(out var comp))
            {
                _cache[attachment.Type] = comp;
            }
        }
    }

    public T Get<T>(AttachType type) where T : Component => _cache.TryGetValue(type, out var comp) ? comp as T : null;

    private bool IsSupporterUnit => GetComponent<SupporterUnit>() != null;
}

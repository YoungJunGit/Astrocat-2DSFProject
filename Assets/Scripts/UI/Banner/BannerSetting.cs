using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BannerSetting", menuName = "GameScene/Timeline/BannerSetting", order = 1)]
public class BannerSetting : ScriptableObject
{
    [Serializable]
    public class CustomAnchor
    {
        public Vector2 max;
        public Vector2 min;
    }

    [SerializeField] private EntityBanner   _bannerPrefab;
    [SerializeField] private int            _maxBannerIndex;
    [SerializeField] private Vector2        _initialPos;
    [SerializeField] private float          _distance;
    [SerializeField] private CustomAnchor   _anchor;

    public EntityBanner BannerPrefab    => _bannerPrefab;
    public int MaxBannerIndex           => _maxBannerIndex;
    public Vector2 InitialPos           => _initialPos;
    public float Distance               => _distance;
    public CustomAnchor Anchor          => _anchor;
}

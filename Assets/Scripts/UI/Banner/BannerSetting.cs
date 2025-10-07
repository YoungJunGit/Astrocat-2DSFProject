using Obvious.Soap;
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

    [SerializeField] private IntVariable    _maxBannerIndex;
    [SerializeField] private Vector2        _initialPos;
    [SerializeField] private float          _distance;
    [SerializeField] private CustomAnchor   _anchor;

    public int MaxBannerIndex            => _maxBannerIndex.Value;
    public Vector2 InitialPos            => _initialPos;
    public Vector2 FinalPos              => new Vector2((_initialPos.x * 2) + _distance * _maxBannerIndex, _initialPos.y);
    public float Distance                => _distance;
    public CustomAnchor Anchor           => _anchor;
    public Vector2 CurrentPos(int index) => new Vector2((_initialPos.x * 2) + _distance * Mathf.Clamp(index, 1, _maxBannerIndex), _initialPos.y);
}

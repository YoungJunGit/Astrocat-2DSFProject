using Obvious.Soap;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BannerSetting", menuName = "SO/UI/Timeline/BannerSetting", order = 1)]
public class BannerSetting : ScriptableObject
{
    [SerializeField] private IntVariable    _maxBannerIndex;
    [SerializeField] private Vector2        _initialPos;
    [SerializeField] private float          _distance;
    [SerializeField] private float          _moveDuration;

    public int MaxBannerIndex            => _maxBannerIndex.Value;
    public Vector2 InitialPos            => _initialPos;
    public Vector2 FinalPos              => new Vector2((_initialPos.x * 2) + _distance * _maxBannerIndex.Value, _initialPos.y);
    public Vector2 CurrentPos(int index) => new Vector2((_initialPos.x * 2) + _distance * Mathf.Clamp(index, 1, _maxBannerIndex.Value), _initialPos.y);
    public float MoveDuration => _moveDuration;
}

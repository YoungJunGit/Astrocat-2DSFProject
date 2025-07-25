using System;
using UnityEngine;

[CreateAssetMenu(fileName = "BannerLocationSetting", menuName = "Banner/BannerLocationSetting", order = 1)]
public class BannerLocationSetting : ScriptableObject
{
    [Serializable]
    public class CustomAnchor
    {
        public Vector2 max;
        public Vector2 min;
    }

    [SerializeField] private Vector2 _initialPos;
    [SerializeField] private float _distance;
    [SerializeField] private CustomAnchor _anchor;

    public Vector2 InitialPos   { get { return _initialPos; } }
    public float Distance       { get { return _distance; } }
    public CustomAnchor Anchor  { get { return _anchor; } }
}

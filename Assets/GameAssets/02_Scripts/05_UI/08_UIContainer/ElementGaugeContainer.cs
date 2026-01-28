using AYellowpaper.SerializedCollections;
using DataEnum;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementGaugeContainer", menuName = "SO/UI/Container/ElementGaugeContainer", order = 2)]
public class ElementGaugeContainer : ScriptableObject
{
    [SerializedDictionary("Element Type", "Gauge Sprite")]
    public SerializedDictionary<ELEMENT_TYPE, Sprite> GaugeSpriteList;

    [SerializedDictionary("Element Type", "Glow Color")]
    public SerializedDictionary<ELEMENT_TYPE, Color> GlowColorList;

    [SerializeField]
    public AnimationCurve customEase;
}
using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ValueContainer", menuName = "GameScene/Container/ValueContainer")]
public class ValueContainer : ScriptableObject
{
    [SerializeField] 
    private DamageHealBuffValue value_Prefab;
    [SerializeField] 
    private GameObject          digit_Prefab;
    [SerializeField] 
    private Sprite[]            damage_Sprites;
    [SerializeField, CurveRange(0, 0, 1, 1)] 
    private AnimationCurve      jumpEase;
    [SerializeField, CurveRange(0, 0, 1, 1)] 
    private AnimationCurve      fadeEase;
    [SerializeField]
    private Vector3             scaleValue;
    [SerializeField] 
    private float               jumpValue;

    public DamageHealBuffValue Value_Prefab    => value_Prefab;
    public GameObject          Digit_Prefab    => digit_Prefab;
    public Sprite[]            Damage_Sprites  => damage_Sprites;
    public AnimationCurve      JumpEase        => jumpEase;
    public AnimationCurve      FadeEase        => fadeEase;
    public Vector3             ScaleValue      => scaleValue;
    public float               JumpValue       => jumpValue;
}

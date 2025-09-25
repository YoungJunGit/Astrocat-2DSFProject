using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageContainer", menuName = "GameScene/Damage/DamageContainer")]
public class DamageContainer : ScriptableObject
{
    [SerializeField] 
    private GameObject     value_Prefab;
    [SerializeField] 
    private GameObject     digit_Prefab;
    [SerializeField] 
    private Sprite[]       damage_Sprites;
    [SerializeField, CurveRange(0, 0, 1, 1)] 
    private AnimationCurve jumpEase;
    [SerializeField, CurveRange(0, 0, 1, 1)] 
    private AnimationCurve fadeEase;
    [SerializeField] 
    private float          jumpValue;

    public GameObject       Value_Prefab    => value_Prefab;
    public GameObject       Digit_Prefab    => digit_Prefab;
    public Sprite[]         Damage_Sprites  => damage_Sprites;
    public AnimationCurve   JumpEase        => jumpEase;
    public AnimationCurve   FadeEase        => fadeEase;
    public float            JumpValue       => jumpValue;
}

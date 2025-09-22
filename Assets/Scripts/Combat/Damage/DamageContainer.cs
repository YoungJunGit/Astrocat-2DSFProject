using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageContainer", menuName = "GameScene/Damage/DamageContainer")]
public class DamageContainer : ScriptableObject
{
    [ShowInInspector] public GameObject Value_Prefab { get; set; }
    [ShowInInspector] public GameObject Digit_Prefab { get; set; }
    [ShowInInspector] public Sprite[] Damage_Sprites { get; set; }
    [ShowInInspector] public AnimationCurve JumpEase { get; set; }
    [ShowInInspector] public AnimationCurve FadeEase { get; set; }
    [ShowInInspector] public float JumpValue         { get; set; }
}

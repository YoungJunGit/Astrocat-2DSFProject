using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor.Animations;
using System.Linq;

[CreateAssetMenu(fileName = "BackgroundSetting", menuName = "Backgrounds/BackgroundSetting")]
public class BackgroundSetting : ScriptableObject
{
    [ShowInInspector, HorizontalGroup("Main", Width = 0.7f, MarginRight = 0.1f)]
    private string Name;
    [ShowInInspector, HorizontalGroup("Main")]
    private bool UseAnim;

    [TabGroup("tabs", "Sprite", SdfIconType.MapFill, TextColor = "green", TabLayouting = TabLayouting.Shrink)]
    [SerializeField] private List<Sprite> background_sprites;
    [TabGroup("tabs", "Animation", SdfIconType.CodeSquare, TextColor = "blue"), ShowIf("UseAnim")]
    [SerializeField] private List<RuntimeAnimatorController> background_anims;

    public Sprite GetBackgroundSprite(int index) => background_sprites[index];
    public RuntimeAnimatorController GetBackgroundAnimator(int index) => background_anims.ElementAtOrDefault(index);
    public string GetName() => Name;
    public int BackgroundCount => background_sprites.Count;
}

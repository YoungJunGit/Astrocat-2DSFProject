using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using System.Linq;

[CreateAssetMenu(fileName = "Setting", menuName = "SO/UI/Background/Setting", order = 2)]
public class BackgroundSetting : ScriptableObject
{
    [SerializeField]
    private string Name;

    [TabGroup("tabs", "Sprite", SdfIconType.MapFill, TextColor = "green", TabLayouting = TabLayouting.Shrink)]
    [SerializeField] 
    private List<Sprite> background_sprites;

    [TabGroup("tabs", "Animation", SdfIconType.CodeSquare, TextColor = "blue")]
    [SerializeField] 
    private List<RuntimeAnimatorController> background_anims;

    public Sprite GetBackgroundSprite(int index) => background_sprites[index];
    public RuntimeAnimatorController GetBackgroundAnimator(int index) => background_anims.ElementAtOrDefault(index);
    public string GetName() => Name;
    public int BackgroundCount => background_sprites.Count;
}

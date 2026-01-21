using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "WarningTextSetting", menuName = "UI/TextSetting/WarningTextSetting", order = 1)]
public class WarningTextSetting : BaseTextSetting
{
    [BoxGroup("Tween Option")]
    public bool TweenWidth = false;

    [BoxGroup("Tween Option")]
    public bool TweenHeight = true;

    [BoxGroup("Tween Option")]
    public bool TweenFade = true;


    [FoldoutGroup("Warning Box Setting")]
    public float HeightOffset = 20.0f;

    [FoldoutGroup("Warning Box Setting")]
    public float WidthOffset = 40.0f;

    [FoldoutGroup("Warning Box Setting")]
    public float MinimumHegithValue = 120.0f;

    [FoldoutGroup("Warning Box Setting")]
    public float MinimumWidthValue = 500.0f;


    [FoldoutGroup("Tween Setting"), ShowIf("@this.TweenWidth || this.TweenHeight")]
    public float BoxTweenDuration = 0.25f;

    [FoldoutGroup("Tween Setting"), ShowIf("@this.TweenFade")]
    public FadeSetting FadeSetting;
}
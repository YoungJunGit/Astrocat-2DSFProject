using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

[Serializable]
public class FadeSetting
{
    [Flags]
    public enum FADE_SETTING
    {
        NONE = 0,
        FADEOUT = 1 << 0,
        FADEIN = 1 << 1,
        ALL = FADEIN | FADEOUT,
    }

    [EnumToggleButtons, SerializeField] 
    private FADE_SETTING fade_Setting;

    private bool ShowFadeIn => (fade_Setting & FADE_SETTING.FADEIN) != 0;
    private bool ShowFadeOut => (fade_Setting & FADE_SETTING.FADEOUT) != 0;
    private bool ShowAll => fade_Setting == FADE_SETTING.ALL;

    [BoxGroup("FadeIn Setting")]
    [ShowIf("@ShowFadeIn"), SerializeField]
    private float fadeInDuration = 1f;

    [BoxGroup("FadeIn Setting")]
    [ShowIf("@ShowFadeIn"), SerializeField]
    private Ease fadeInCurve = Ease.Linear;

    [BoxGroup("FadeOut Setting")]
    [ShowIf("@ShowFadeOut"), SerializeField]
    private float fadeOutDuration = 1f;

    [BoxGroup("FadeOut Setting")]
    [ShowIf("@ShowFadeOut"), SerializeField]
    private Ease fadeOutCurve = Ease.Linear;

    [BoxGroup("Delay Setting")]
    [ShowIf("@(Fade_Setting != FADE_SETTING.NONE)"), SerializeField]
    private float startDelay = 0f;

    [BoxGroup("Delay Setting")]
    [ShowIf("@ShowAll"), SerializeField]
    private float fadeInOutDelay = 3f;

    [BoxGroup("Loop Setting")]
    [ShowIf("@ShowAll"), PropertyTooltip("If this set to -1, loop infinitely"), SerializeField]
    private int loops = 1;

    public FADE_SETTING Fade_Setting => fade_Setting;
    public float FadeInDuration => fadeInDuration;
    public Ease FadeInCurve => fadeInCurve;
    public float FadeOutDuration => fadeOutDuration;
    public Ease FadeOutCurve => fadeOutCurve;
    public float StartDelay => startDelay;
    public float FadeInOutDelay => fadeInOutDelay;
    public int Loops => loops;
}
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    [Flags]
    enum FADE_SETTING
    {
        NONE = 0,
        FADEOUT = 1 << 0,
        FADEIN = 1 << 1,
        ALL = FADEIN | FADEOUT,
    }

    [SerializeField] bool autoStart = false;
    [EnumToggleButtons, SerializeField] FADE_SETTING Fade_Setting;

    private bool ShowFadeIn => (Fade_Setting & FADE_SETTING.FADEIN) != 0;
    private bool ShowFadeOut => (Fade_Setting & FADE_SETTING.FADEOUT) != 0;
    private bool ShowAll => Fade_Setting == FADE_SETTING.ALL;

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

    private CanvasGroup ui;
    private Tween _running;

    private void Start()
    {
        if (autoStart)
            FadeAnimation();
    }

    public Tween FadeAnimation(float offSetStartDelay = 0f, Action OnFinishEvent = null)
    {
        ui = GetComponent<CanvasGroup>();

        TweenParams tweenParams = new TweenParams()
            .SetDelay(startDelay + offSetStartDelay)
            .SetLoops(loops)
            .OnComplete(() => { OnFinishEvent?.Invoke(); });

        bool doIn = Fade_Setting.HasFlag(FADE_SETTING.FADEIN);
        bool doOut = Fade_Setting.HasFlag(FADE_SETTING.FADEOUT);

        if (doOut && !doIn)
        {
            ui.alpha = 0f;
            _running = ui.DOFade(1f, fadeOutDuration)
                         .SetEase(fadeOutCurve)
                         .SetAs(tweenParams);
        }
        else if(!doOut && doIn)
        {
            ui.alpha = 1f;
            _running = ui.DOFade(0f, fadeInDuration)
                         .SetEase(fadeInCurve)
                         .SetAs(tweenParams);
        }
        else if(doOut && doIn)
        {
            ui.alpha = 0f;
            _running = DOTween.Sequence()
                              .Append(ui.DOFade(1f, fadeOutDuration).SetEase(fadeOutCurve))
                              .AppendInterval(fadeInOutDelay)
                              .Append(ui.DOFade(0f, fadeInDuration).SetEase(fadeInCurve))
                              .SetAs(tweenParams);
        }

        return _running;
    }

    private void OnDestroy()
    {
        if (_running != null)
            _running.Kill();
    }
}

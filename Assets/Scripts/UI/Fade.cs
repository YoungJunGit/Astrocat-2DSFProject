using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static FadeSetting;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    [SerializeField]
    bool autoStart = false;

    [SerializeField]
    private bool OverrideSetting = false;

    [HideIf("@OverrideSetting"), SerializeField]
    FadeSetting Setting;

    private CanvasGroup ui;

    private void Start()
    {
        if (autoStart)
            FadeAnimation();
    }

    public Tween FadeAnimation(Action OnFinishEvent = null, FadeSetting setting = null)
    {
        if(OverrideSetting == true)
        {
            if(setting != null)
            {
                Setting = setting;
            }
            else
            {
                Debug.LogWarning("Fade Failed!!!");
                return null;
            }
        }

        ui = GetComponent<CanvasGroup>();

        TweenParams tweenParams = new TweenParams()
            .SetDelay(Setting.StartDelay)
            .SetLoops(Setting.Loops)
            .OnComplete(() => { OnFinishEvent?.Invoke(); });

        bool doIn = Setting.Fade_Setting.HasFlag(FADE_SETTING.FADEIN);
        bool doOut = Setting.Fade_Setting.HasFlag(FADE_SETTING.FADEOUT);

        Tween running = null;
        if (doOut && !doIn)
        {
            ui.alpha = 0f;
            running = ui.DOFade(1f, Setting.FadeOutDuration)
                .SetEase(Setting.FadeOutCurve)
                .SetAs(tweenParams);
        }
        else if(!doOut && doIn)
        {
            ui.alpha = 1f;
            running = ui.DOFade(0f, Setting.FadeInDuration)
                .SetEase(Setting.FadeInCurve)
                .SetAs(tweenParams);
        }
        else if(doOut && doIn)
        {
            ui.alpha = 0f;
            running = DOTween.Sequence()
                .Append(ui.DOFade(1f, Setting.FadeOutDuration).SetEase(Setting.FadeOutCurve))
                .AppendInterval(Setting.FadeInOutDelay)
                .Append(ui.DOFade(0f, Setting.FadeInDuration).SetEase(Setting.FadeInCurve))
                .SetAs(tweenParams)
                .SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }

        return running;
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;

[RequireComponent(typeof(CanvasGroup))]
public class Fade : MonoBehaviour
{
    [SerializeField] private bool fadeIn = true;

    [ShowIf("fadeIn"), BoxGroup("FadeIn Setting"), SerializeField] private float fadeInDuration = 1f;
    [ShowIf("fadeIn"), BoxGroup("FadeIn Setting"), SerializeField] private Ease fadeInCurve = Ease.Linear;

    [SerializeField] private bool fadeOut = true;

    [ShowIf("fadeOut"), BoxGroup("FadeOut Setting"), SerializeField] private float fadeOutDuration = 1f;
    [ShowIf("fadeOut"), BoxGroup("FadeOut Setting"), SerializeField] private Ease fadeOutCurve = Ease.Linear;

    [BoxGroup("Delay Setting"), SerializeField] 
    private float startDelay = 0f;
    [ShowIf(EConditionOperator.And, "fadeIn", "fadeOut"), BoxGroup("Delay Setting"), SerializeField] 
    private float fadeInOutDelay = 3f;

    [ShowIf(EConditionOperator.And, "fadeIn", "fadeOut"), BoxGroup("Loop Setting"), InfoBox("If this set to -1, loop infinitely"), SerializeField] 
    private int loops = 1;

    private CanvasGroup ui;

    public async UniTask<bool> FadeAnimation(Action OnFinishEvent = null, float offSetStartDelay = 0f)
    {
        ui = GetComponent<CanvasGroup>();
        bool isComplete = false;

        TweenParams tweenParams = new TweenParams().SetDelay(startDelay + offSetStartDelay).SetLoops(loops)
                                                   .OnComplete(() => isComplete = true);

        if(fadeOut && !fadeIn)
        {
            ui.alpha = 0f;
            ui.DOFade(1f, fadeOutDuration).SetEase(fadeOutCurve).SetAs(tweenParams);
        }
        else if(!fadeOut && fadeIn)
        {
            ui.alpha = 1f;
            ui.DOFade(0f, fadeInDuration).SetEase(fadeInCurve).SetAs(tweenParams);
        }
        else if(fadeOut && fadeIn)
        {
            ui.alpha = 0f;
            DOTween.Sequence().Append(ui.DOFade(1f, fadeOutDuration).SetEase(fadeOutCurve))
                              .AppendInterval(fadeInOutDelay)
                              .Append(ui.DOFade(0f, fadeInDuration).SetEase(fadeInCurve))
                              .SetAs(tweenParams);
        }

        await UniTask.WaitUntil(() => isComplete);
        OnFinishEvent?.Invoke();
        return true;
    }
}

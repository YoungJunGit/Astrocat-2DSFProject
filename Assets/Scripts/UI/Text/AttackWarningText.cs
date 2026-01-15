using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class AttackWarningText : BaseText
{
    [SerializeField] bool animateText = false;

    [ShowIf("@animateText"), SerializeField] 
    float textDuration = 1.0f;

    [SerializeField] FadeSetting fadeSetting;

    public async override UniTask<bool> ShowText(InputHandler inputHandler)
    {
        bool isFadeComplete = false;
        Tween textTween = null;
        Tween fadeTween = null;

        if (animateText)
        {
            string text = textComp.text;
            textComp.text = "";
            textTween = textComp.DOText(text, textDuration);
        }

        fadeTween = GetComponent<Fade>().FadeAnimation(() => isFadeComplete = true, fadeSetting);

        using (new InputDisposer(inputHandler, InputHandler.InputState.Skip))
        {
            if (textTween != null)
                inputHandler.OnSkipSkip += () => { textTween.Complete(); };

            if (fadeTween != null)
                inputHandler.OnSkipSkip += () => { fadeTween.Goto(0f, true); inputHandler.DisposeOnSkipActions(); };

            await UniTask.WaitUntil(() => isFadeComplete);
        }

        return isFadeComplete;
    }
}

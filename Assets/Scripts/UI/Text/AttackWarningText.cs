using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AttackWarningText : BaseText
{
    [SerializeField] bool animateText = false;
    [SerializeField] float duration = 1.0f;
    public async override UniTask<bool> ShowText(InputHandler inputHandler)
    {
        var InputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.Skip);

        if (animateText)
        {
            string text = textComp.text;
            textComp.text = "";
            Tween t = textComp.DOText(text, duration);

            inputHandler.OnSkipSkip += () => { t.Complete(); inputHandler.DisposeOnSkipActions(); };
        }

        bool isFadeComplete = false;
        GetComponent<Fade>().FadeAnimation(duration, () => isFadeComplete = true);

        await UniTask.WaitUntil(() => isFadeComplete);

        InputDisposer.Dispose();

        return isFadeComplete;
    }
}

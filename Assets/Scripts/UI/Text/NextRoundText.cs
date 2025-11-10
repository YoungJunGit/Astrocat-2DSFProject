using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NextRoundText : BaseText
{
    [SerializeField] float duration = 2.0f;
    public async override UniTask<bool> ShowText(InputHandler inputHandler)
    {
        bool isFadeComplete = false;
        Tween t = GetComponent<Fade>().FadeAnimation(duration, () => isFadeComplete = true);

        using (new InputDisposer(inputHandler, InputHandler.InputState.Skip))
        {
            inputHandler.OnSkipSkip += () => { t.Goto(0f, true); inputHandler.DisposeOnSkipActions(); };
            await UniTask.WaitUntil(() => isFadeComplete);
        }

        return isFadeComplete;
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class NextRoundText : BaseText
{
    [SerializeField] FadeSetting fadeSetting;

    public async override UniTask<bool> ShowText(InputHandler inputHandler)
    {
        bool isFadeComplete = false;
        Tween t = GetComponent<Fade>().FadeAnimation(() => isFadeComplete = true, fadeSetting);

        using (new InputDisposer(inputHandler, InputHandler.InputState.Skip))
        {
            inputHandler.OnSkipSkip += () => { t.Complete(); inputHandler.DisposeOnSkipActions(); };
            await UniTask.WaitUntil(() => isFadeComplete);
        }

        return isFadeComplete;
    }
}

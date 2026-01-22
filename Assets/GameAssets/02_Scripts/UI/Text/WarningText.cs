using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class WarningText : BaseText<WarningTextSetting>
{
    public enum Direction
    {
        Left = 0,
        Right,
        Bottom,
        Top
    }

    [SerializeField, FoldoutGroup("Component", expanded: false)]
    Fade fade;

    [SerializeField, FoldoutGroup("Component")]
    RectTransform owner;

    [SerializeField, FoldoutGroup("Component")]
    RectTransform mask;

    [SerializeField, FoldoutGroup("Component")]
    RectTransform text;

    public override async UniTask ShowText(WarningTextSetting setting, InputHandler inputHandler)
    {
        CanvasGroup ui = GetComponent<CanvasGroup>();
        ui.alpha = 0f;
        bool playBoxTween = setting.TweenHeight || setting.TweenWidth;
        bool playFadeTween = setting.TweenFade && fade != null && setting.FadeSetting != null;

        Vector2 textSize = await GetUISizeAsync(text);

        float height = textSize.y + CalculateStretchOffsets(mask, Direction.Bottom, Direction.Top) + setting.HeightOffset;
        float width = textSize.x + CalculateStretchOffsets(mask, Direction.Left, Direction.Right) + setting.WidthOffset;

        float finalHeight = Mathf.Max(height, setting.MinimumHegithValue);
        float finalWidth = Mathf.Max(width, setting.MinimumWidthValue);

        SetHeight(owner, finalHeight);
        SetWidth(owner, finalWidth);

        Vector2 targetSize = owner.sizeDelta;

        if (setting.TweenHeight)
        {
            targetSize.y = finalHeight;
            SetHeight(owner, 0.0f);
        }

        if (setting.TweenWidth)
        {
            targetSize.x = finalWidth;
            SetWidth(owner, 0.0f);
        }

        InputDisposer disposer = new InputDisposer(inputHandler, InputHandler.InputState.Skip);
        if (playBoxTween || playFadeTween)
        {
            Sequence seq = DOTween.Sequence();
            if (playBoxTween)
            {
                ui.alpha = 1f;
                seq.Append(owner.DOSizeDelta(targetSize, setting.BoxTweenDuration).SetEase(Ease.Linear).SetLink(gameObject));
            }

            if (playFadeTween)
            {
                seq.Append(fade.FadeAnimation(setting.FadeSetting));
            }
            
            float boxDuration = playBoxTween ? setting.BoxTweenDuration : 0f;
            float fadeDelay = playFadeTween ? setting.FadeSetting.StartDelay + setting.FadeSetting.FadeOutDuration + setting.FadeSetting.FadeInOutDelay : 0f;

            inputHandler.OnSkipSkip += () => 
            { 
                seq.Goto(boxDuration + fadeDelay, true);

                inputHandler.DisposeOnSkipActions();
            };

            await seq.AsyncWaitForCompletion();
        }
        else
        {
            ui.alpha = 1f;
            CancellationTokenSource source = new();
            inputHandler.OnSkipSkip += () => source.Cancel();
            await UniTask.WaitForSeconds(3f, cancellationToken: source.Token).SuppressCancellationThrow();
        }
        disposer.Dispose();
        ui.alpha = 0f;
    }

    public async UniTask<bool> ShowText2()
    {

        return true;
    }

    private async UniTask<Vector2> GetUISizeAsync(RectTransform rt)
    {
        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
        return rt.rect.size;
    }

    private float CalculateStretchOffsets(RectTransform rt, params Direction[] directions)
    {
        float offset = 0.0f;

        foreach (var dir in directions)
        {
            switch (dir)
            {
                case Direction.Left:
                    offset += rt.offsetMin.x;
                    break;
                case Direction.Right:
                    offset -= rt.offsetMax.x;
                    break;
                case Direction.Bottom:
                    offset += rt.offsetMin.y;
                    break;
                case Direction.Top:
                    offset -= rt.offsetMax.y;
                    break;
            }
        }

        return offset;
    }

    private void SetHeight(RectTransform rt, float height)
    {
        var size = rt.sizeDelta;
        size.y = height;
        rt.sizeDelta = size;
    }

    private void SetWidth(RectTransform rt, float width)
    {
        var size = rt.sizeDelta;
        size.x = width;
        rt.sizeDelta = size;
    }
}
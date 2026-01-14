using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class RecoveryFactor : MonoBehaviour
{
    [SerializeField]
    float moveValue = 1.5f;

    [SerializeField]
    float duration = 1.0f;

    [SerializeField]
    Vector2 tweenSize;

    public async UniTask Init()
    {
        var size = Random.Range(tweenSize.x, tweenSize.y);

        transform.localScale = new Vector2(0.2f, 0.2f);

        Tween t1 = transform.DOScale(Vector2.one * size, duration)
            .SetEase(Ease.Linear);

        Tween t2 = transform.DOLocalMoveY(moveValue, 1.0f)
            .SetEase(Ease.Linear);

        await UniTask.WhenAll(t1.AsyncWaitForCompletion().AsUniTask(), t2.AsyncWaitForCompletion().AsUniTask());
    }
}
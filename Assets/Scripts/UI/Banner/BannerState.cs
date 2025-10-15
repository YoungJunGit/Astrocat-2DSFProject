using DG.Tweening;
using UnityEngine;

public interface IBannerState
{
    void PlayState(Transform transform);
}

public class BannerCurrent : IBannerState
{
    public void PlayState(Transform transform)
    {
        transform.DOScale(Vector3.one * 1.2f, 0.4f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.Linear);
    }
}

public class BannerDestroy : IBannerState
{
    public void PlayState(Transform transform)
    {
        transform.DOScale(Vector3.zero, 0.4f)
                 .SetEase(Ease.Linear)
                 .OnComplete(() =>
                 {
                    transform.DOKill();
                    Object.Destroy(transform.gameObject);
                 });
    }
}

public class BannerFaint : IBannerState
{
    public void PlayState(Transform transform)
    {
        
    }
}
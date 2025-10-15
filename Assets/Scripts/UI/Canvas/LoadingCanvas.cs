using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private Image _loadingImage;
    [SerializeField] private GameObject _loading;
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private GameObject _loadingCircle;
    [SerializeField] private GameObject _FinishedLoadingText;
    [SerializeField] private Fade _glowEffect;

    public void Fade(Action onFinishedFade)
    {
        _loadingImage.DOFade(1f, 0.5f).OnComplete(() => {
            _loading.SetActive(true);
            onFinishedFade.Invoke();
        });
    }

    public async UniTask Loading(AsyncOperation asyncOperation)
    {
        while (!asyncOperation.isDone)
        {
            await UniTask.Yield();
            _loadingBar.value = asyncOperation.progress;

            if (asyncOperation.progress >= 0.9f)
            {
                DOTween.KillAll();
                break;
            }
        }
    }

    public void OnLoadingComplete()
    {
        _loadingBar.value = 1f;
        _loadingCircle.SetActive(false);
        _FinishedLoadingText.SetActive(true);
        _glowEffect.FadeAnimation().Forget();
    }
}

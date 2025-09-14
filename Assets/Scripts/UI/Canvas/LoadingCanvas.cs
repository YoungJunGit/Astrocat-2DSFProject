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

    public void Fade(Action<Slider> onFinishedFade)
    {
        _loadingImage.DOFade(1f, 0.5f).OnComplete(() => {
            _loading.SetActive(true);
            onFinishedFade.Invoke(_loadingBar);
        });
    }

    public void OnLoadingComplete()
    {
        Debug.Log("1");
        _loadingCircle.SetActive(false);
        _FinishedLoadingText.SetActive(true);
        _glowEffect.FadeAnimation().Forget();
    }
}

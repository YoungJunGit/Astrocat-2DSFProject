using UnityEngine;
using UnityEngine.UI;

public class QTEUI : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private GameObject qteUI;
    [SerializeField] private Image leftTimeGageImage;

    private float _maxTime;
    private float _currentTime = 0f;

    public void StartQTE(float time, Vector2 position = default)
    {
        UpdatePublisher.SubscribeObserver(this);
        qteUI.SetActive(true);
        _maxTime = time;
        _currentTime = time;
        qteUI.transform.position = position;
    }

    public void EndQTE()
    {
        UpdatePublisher.DiscribeObserver(this);
        _currentTime = 0;
        _maxTime = 0;
        qteUI.SetActive(false);
        qteUI.transform.position = Vector2.zero;
    }


    public void ObserverUpdate(float dt)
    {
        leftTimeGageImage.fillAmount = _currentTime / _maxTime;
    }
}
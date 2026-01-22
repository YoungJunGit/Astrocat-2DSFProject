using UnityEngine;
using UnityEngine.UI;

public class QTEUI : MonoBehaviour
{
    [SerializeField] private GameObject qteUI;
    [SerializeField] private Image leftTimeGageImage;

    private float _maxTime;
    private float _currentTime = 0f;

    public void Init()
    {
        EndQte();
    }

    public void StartQte(float time, Vector2 position = default)
    {
        qteUI.SetActive(true);
        _maxTime = time;
        _currentTime = time;
        qteUI.transform.position = position;
    }

    public void EndQte()
    {
        _currentTime = 0;
        _maxTime = 0;
        qteUI.SetActive(false);
        qteUI.transform.position = Vector2.zero;
    }
    
    public void UpdateQteGage(float dt)
    {
        if (_currentTime <= 0)
            return;

        _currentTime -= dt;
        leftTimeGageImage.fillAmount = _currentTime / _maxTime;
    }
}
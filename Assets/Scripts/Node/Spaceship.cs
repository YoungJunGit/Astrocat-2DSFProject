using UnityEngine;

public class Spaceship : MonoBehaviour, IUpdateObserver
{
    [SerializeField] private float rotateSpeed = 1.2f;
    private void Start()
    {
        UpdatePublisher.SubscribeObserver(this);
    }

    public void ObserverUpdate(float dt)
    {
        transform.Rotate(0, 360 * Time.deltaTime * rotateSpeed, 0);
    }

    private void OnDisable()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}

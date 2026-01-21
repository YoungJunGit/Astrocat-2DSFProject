using UnityEngine;

public class BackgroundChanger : MonoBehaviour, IUpdateObserver
{
    BackgroundManager backgroundManager;

    public void Init()
    {
        ServiceLocator.For(this).Get(out backgroundManager);
        UpdatePublisher.SubscribeObserver(this);
    }

    public void ObserverUpdate(float dt)
    {
        int inputNum;

        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                inputNum = i;
                backgroundManager.ChangeBackground(inputNum);
                break;
            }
        }
    }

    private void OnDestroy()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}

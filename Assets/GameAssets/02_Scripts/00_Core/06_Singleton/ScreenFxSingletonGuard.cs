using UnityEngine;

public class ScreenFxSingletonGuard : MonoBehaviour
{
    private static ScreenFxSingletonGuard _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
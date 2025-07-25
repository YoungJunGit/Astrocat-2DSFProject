using System;
using UnityEngine;

[CreateAssetMenu(fileName = "InputHandler", menuName = "Core/InputHandler", order = 1)]
class InputHandler : ScriptableObject, IUpdateObserver
{
    private void Awake()
    {
        UpdatePublisher.SubscribeObserver(this);
    }

    public Action<KeyCode> OnInputButtonDown;
    public void ObserverUpdate(float dt)
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    OnInputButtonDown?.Invoke(key);
                }
            }
        }
    }
}
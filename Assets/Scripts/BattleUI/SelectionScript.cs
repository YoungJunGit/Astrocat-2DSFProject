using UnityEngine;
using System;

public class SelectionScript : MonoBehaviour
{
    private Action<string> onSelection;

    public void Initialize(Action<string> callback)
    {
        onSelection = callback;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            onSelection?.Invoke("BasicAttack");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            onSelection?.Invoke("Skill");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            onSelection?.Invoke("UseItem");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            onSelection?.Invoke("Signature Move");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectionScript : MonoBehaviour
{
    private Action<string> onSelection;

    public void Initialize(Action<string> callback)
    {
        onSelection = callback;

        transform.Find("BasicAttack")?.GetComponent<Button>()?.onClick.AddListener(() => onSelection?.Invoke("BasicAttack"));
        transform.Find("Skill")?.GetComponent<Button>()?.onClick.AddListener(() => onSelection?.Invoke("Skill"));
        transform.Find("UseItem")?.GetComponent<Button>()?.onClick.AddListener(() => onSelection?.Invoke("UseItem"));
    }
}

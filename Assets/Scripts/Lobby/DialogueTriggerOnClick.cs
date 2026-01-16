using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.InkSupport;

public class DialogueTriggerOnClick : MonoBehaviour
{
    private DialogueSystemInkTrigger inkTrigger;

    private void Awake()
    {
        // 같은 오브젝트에 붙어있는 Dialogue System Trigger 가져오기
        inkTrigger = GetComponent<DialogueSystemInkTrigger>();
    }

    private void OnMouseDown()
    {
        Debug.Log("[DialogueTriggerOnClick] Click detected!");
        if (inkTrigger == null)
        {
            Debug.LogWarning("[DialogueTriggerOnClick] DialogueSystemInkTrigger not found!");
            return;
        }

        // Dialogue System Trigger의 On Use 호출
        inkTrigger.OnUse();
    }
}

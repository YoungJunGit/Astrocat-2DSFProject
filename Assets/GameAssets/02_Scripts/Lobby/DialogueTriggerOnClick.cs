using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.InkSupport;

public class DialogueTriggerOnClick : MonoBehaviour
{
    private DialogueSystemInkTrigger inkTrigger;
    private DialogueSystemTrigger dialogueTrigger;

    private void Awake()
    {
        inkTrigger = GetComponent<DialogueSystemInkTrigger>();
        dialogueTrigger = GetComponent<DialogueSystemTrigger>();

        Debug.Log($"[DialogueTriggerOnClick] Awake. inkTrigger={(inkTrigger ? inkTrigger.name : "NULL")}, dialogueTrigger={(dialogueTrigger ? dialogueTrigger.name : "NULL")}");
    }

    private void OnMouseDown()
    {
        Debug.Log("[DialogueTriggerOnClick] Click detected!");

        Debug.Log($"[DialogueTriggerOnClick] ConversationActive={DialogueManager.isConversationActive} | lastConv='{DialogueManager.lastConversationStarted}' id={DialogueManager.lastConversationID}");

        // 대화중이면 일단 막고 확인
        if (DialogueManager.isConversationActive)
        {
            Debug.LogWarning("[DialogueTriggerOnClick] 이미 대화 중이라 OnUse 실행 안 함");
            return;
        }

        if (inkTrigger == null && dialogueTrigger == null)
        {
            Debug.LogError("[DialogueTriggerOnClick] 둘 다 NULL. 같은 오브젝트에 Trigger가 없음");
            return;
        }

        if (inkTrigger != null)
        {
            Debug.Log("[DialogueTriggerOnClick] Calling inkTrigger.OnUse()");
            inkTrigger.OnUse();
            return;
        }

        Debug.Log("[DialogueTriggerOnClick] Calling dialogueTrigger.OnUse()");
        dialogueTrigger.OnUse();
    }
}

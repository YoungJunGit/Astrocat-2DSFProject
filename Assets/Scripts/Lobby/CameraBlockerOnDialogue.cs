using UnityEngine;

public class CameraBlockerOnDialogue : MonoBehaviour
{
    // Dialogue System이 대화 시작 시 actor/conversant에게 보내는 메시지
    private void OnConversationStart(Transform actor)
    {
        CameraMove.Instance?.SetBlockInput(true);
    }

    // Dialogue System이 대화 종료 시 actor/conversant에게 보내는 메시지
    private void OnConversationEnd(Transform actor)
    {
        CameraMove.Instance?.SetBlockInput(false);
    }
}

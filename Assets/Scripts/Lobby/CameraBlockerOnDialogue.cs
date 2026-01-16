using UnityEngine;
using PixelCrushers.DialogueSystem;

public class CameraBlockerOnDialogue : MonoBehaviour
{
    // Dialogue System이 대화 시작 시 actor/conversant에게 보내는 메시지
    public void OnConversationStart(Transform actor)
    {
        ServiceLocator.For(this).Get(out ICameraMove cameraMove);
        cameraMove?.SetBlockInput(true);
    }

    // Dialogue System이 대화 종료 시 actor/conversant에게 보내는 메시지
    public void OnConversationEnd(Transform actor)
    {
        ServiceLocator.For(this).Get(out ICameraMove cameraMove);
        cameraMove?.SetBlockInput(false);
    }
}

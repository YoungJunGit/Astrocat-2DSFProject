using UnityEngine;
using PixelCrushers.DialogueSystem;
public class DialogueTriggerOnClick : MonoBehaviour
{
    private void OnMouseDown()
    {
        Debug.Log($"[CLICK DETECTED] {gameObject.name}");
    }
}

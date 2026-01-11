using Cysharp.Threading.Tasks;
using PixelCrushers.DialogueSystem;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Dialogue Manager", menuName = "Scriptable Objects/DialogueManager")]
public class DialogueSetting : ScriptableObject
{
    [Header("Dialogue Manager")]
    [SerializeField]
    private GameObject dialogueManagerPref;

    [Header("actors")]
    [SerializeField]
    private GameObject riflemanPref;
    [SerializeField]
    private GameObject firebatPref;

    private GameObject pref;
    private bool isDialogueOver;

    public void Init()
    {
        pref = Instantiate(dialogueManagerPref);
        Instantiate(riflemanPref);
        Instantiate(firebatPref);
        isDialogueOver = false;
    }

    public async void SetDialogueState(bool check)
    {
        await new WaitForSecondsRealtime(2.5f);
        isDialogueOver = check;
    }

    public async UniTask ProcessDialogue()
    {
        DialogueDatabase database = dialogueManagerPref.GetComponent<DialogueSystemController>().initialDatabase;
        if(database != null )
        {
            await new WaitUntil(() => isDialogueOver);
            Destroy(pref);
        }
    }
}

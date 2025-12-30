using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Manager", menuName = "Scriptable Objects/DialogueManager")]
public class DialogueManager : ScriptableObject
{
    [Header("Dialogue Manager")]
    [SerializeField]
    private GameObject dialogueManagerPref;

    [Header("actors")]
    [SerializeField]
    private GameObject riflemanPref;
    [SerializeField]
    private GameObject firebatPref;

    public void Init()
    {
        Instantiate(dialogueManagerPref);
        Instantiate(riflemanPref);
        Instantiate(firebatPref);
    }
}

using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "DialogueManager", menuName = "GameScene/DialogueManager", order = 1)]
public class DialogueManager : ScriptableObject
{
    [SerializeField] private DialogueCanvas dialogueCanvasPref;
    private DialogueCanvas dialogueCanvas;

    public void Init()
    {
        dialogueCanvas = Instantiate(dialogueCanvasPref);
    }

    #region[Combat Dialogue]
    public async UniTask<bool> ShowAttackWarningDialogue(BaseUnit unit)
    {
        Dialogue dialogue = dialogueCanvas.GetComponentInChildren<AttackWarningDialogue>(true);

        dialogue.textComp.text = dialogue.textComp.text.Replace("{Name}", unit.GetStat().GetData().Name);
        string attackType = unit.GetUnitType() == DataEnum.UNIT_TYPE.MELEE ? "[Melee]" : "[Range]";
        dialogue.textComp.text = dialogue.textComp.text.Replace("{AttackType}", attackType);

        await dialogue.ShowDialogue();

        dialogue.textComp.text = dialogue.textComp.text.Replace(unit.GetStat().GetData().Name, "{Name}");
        dialogue.textComp.text = dialogue.textComp.text.Replace(attackType, "{AttackType}");

        return true;
    }
    #endregion
}

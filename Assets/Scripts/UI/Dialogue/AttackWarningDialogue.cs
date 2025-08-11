using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class AttackWarningDialogue : Dialogue
{
    [SerializeField] float duration = 1.0f;
    public async override UniTask<bool> ShowDialogue()
    {
        string text = textComp.text;
        textComp.text = "";
        textComp.DOText(text, duration);

        bool isFadeComplete = await GetComponent<Fade>().FadeAnimation(null, duration);

        return isFadeComplete;
    }
}

using Cysharp.Threading.Tasks;
using DataHashAnim;
using DG.Tweening;
using System.Net.Mail;
using UnityEngine;

[RequireComponent(typeof(UnitAttachments))]
public class SupporterUnit : MonoBehaviour
{
    [SerializeField]
    private AnimationHandler _supporterAnimationHandler;

    [HideInInspector]
    public UnitAttachments supporterAttachments;

    public void Initialize()
    {
        _supporterAnimationHandler.Init();
        supporterAttachments = GetComponent<UnitAttachments>();
    }

    public void OnDamaged()
    {
        supporterAttachments.GetSpriteRenderer().color = Color.red;
        supporterAttachments.GetSpriteRenderer().DOBlendableColor(Color.white, 0.25f);
    }

    public async UniTask OnDie(UnitCombatInfo combatInfo)
    {
        supporterAttachments.GetSpriteRenderer().sortingLayerName = "Actor";

        using (var eventDisposer = new EventDisposer(new CombatEvent("SupporterDeathEvent")))
        {
            bool isFinishedEvent = false;

            _supporterAnimationHandler.ChangeAnimation(AnimCombat.DEATH, 0.25f);
            combatInfo.actionList.Add("OnFinishedDeath_Supporter", () =>
            {
                supporterAttachments.GetSpriteRenderer().DOFade(0, 0.5f).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    isFinishedEvent = true;
                });
            }
            );

            await UniTask.WaitUntil(() => isFinishedEvent);
        }

        supporterAttachments.GetSpriteRenderer().sortingLayerName = "Character";
    }
}

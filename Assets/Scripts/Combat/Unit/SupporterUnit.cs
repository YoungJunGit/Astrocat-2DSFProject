using Cysharp.Threading.Tasks;
using DataHashAnim;
using DG.Tweening;
using System.Net.Mail;
using UnityEngine;

[RequireComponent(typeof(UnitAttachments))]
public class SupporterUnit : MonoBehaviour
{
    [SerializeField] private string unitName = "Drone";
    [SerializeField] private AnimationHandler _supporterAnimationHandler;
    [SerializeField] private float hitBlendAmount = 0.75f;
    [SerializeField] private float hitBlendDuration = 0.25f;

    [HideInInspector]
    public UnitAttachments supporterAttachments;

    private const string HIT_BLEND_TWEEN_ID = "HIT_BLEND";

    public void Initialize()
    {
        _supporterAnimationHandler.Init();
        supporterAttachments = GetComponent<UnitAttachments>();
        supporterAttachments.GetSpriteRenderer().material = new Material(supporterAttachments.GetSpriteRenderer().material);
    }

    public void OnDamaged()
    {
        var material = supporterAttachments.GetSpriteRenderer().material;
        DOTween.Kill(unitName + HIT_BLEND_TWEEN_ID);
        material.SetFloat("_HitEffectBlend", hitBlendAmount);
        material.DOFloat(0.0f, "_HitEffectBlend", hitBlendDuration)
            .SetEase(Ease.Linear)
            .SetId(unitName + HIT_BLEND_TWEEN_ID);
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

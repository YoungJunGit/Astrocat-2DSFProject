using Cysharp.Threading.Tasks;
using DataEnum;
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
        ServiceLocator.For(this).Get(out IEffectManager effectManager);
        effectManager.PlayHitEffect(supporterAttachments.GetSpriteRenderer().material, unitName);
    }

    public async UniTask OnDie(UnitCombatInfo combatInfo)
    {
        supporterAttachments.GetSpriteRenderer().sortingLayerName = "Actor";

        using (var eventDisposer = new EventDisposer(new CombatEvent("SupporterDeathEvent")))
        {
            await _supporterAnimationHandler.ChangeAnimationAsync(ANIMATION.DEATH, fadeTime: 0.25f);
            gameObject.SetActive(false);
        }

        supporterAttachments.GetSpriteRenderer().sortingLayerName = "Character";
    }
}

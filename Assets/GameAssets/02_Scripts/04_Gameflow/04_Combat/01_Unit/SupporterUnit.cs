using Cysharp.Threading.Tasks;
using DataEnum;
using DG.Tweening;
using System.Net.Mail;
using UnityEngine;

[RequireComponent(typeof(UnitAttachments))]
public class SupporterUnit : MonoBehaviour
{
    [SerializeField] private AnimationHandler _animationHandler;
    public AnimationHandler AnimationHandler => _animationHandler;
    [SerializeField] private AnimationEventHandler _animatinEventHandler;
    public AnimationEventHandler AnimationEventHandler => _animatinEventHandler;
    [SerializeField] private UnitAttachments _attachments;
    public UnitAttachments Attachments => _attachments;

    [SerializeField] private string unitName = "Drone";
    [SerializeField] private float hitBlendAmount = 0.75f;
    [SerializeField] private float hitBlendDuration = 0.25f;

    IEffectManager _effectManager;

    public void Initialize()
    {
        ServiceLocator.For(this)
            .Get(out _effectManager);

        _animationHandler.Init();
        _animatinEventHandler.Init();
        _attachments.GetSpriteRenderer().material = new Material(_attachments.GetSpriteRenderer().material);
    }

    public void OnDamaged()
    {
        _effectManager.PlayHitEffect(_attachments.GetSpriteRenderer().material, gameObject.GetHashCode().ToString());
    }

    public async UniTask OnDie(UnitCombatInfo combatInfo)
    {
        using (var eventDisposer = new EventDisposer(new CombatEvent("SupporterDeathEvent")))
        {
            await _animationHandler.ChangeAnimationAsync(ANIMATION.DEATH, fadeTime: 0.25f);
            gameObject.SetActive(false);
        }
    }
}

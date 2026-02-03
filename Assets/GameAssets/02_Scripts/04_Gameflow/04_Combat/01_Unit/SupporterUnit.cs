using Cysharp.Threading.Tasks;
using DataEnum;
using DG.Tweening;
using System.Net.Mail;
using UnityEngine;

[RequireComponent(typeof(UnitAttachments))]
public class SupporterUnit : MonoBehaviour
{
    [SerializeField] private UnitAttachments _attachments;
    [SerializeField] private AnimationHandler _animationHandler;
    [SerializeField] private AnimationEventHandler _animatinEventHandler;

    public UnitAttachments Attachments => _attachments;
    public AnimationHandler AnimationHandler => _animationHandler;
    public AnimationEventHandler AnimationEventHandler => _animatinEventHandler;

    [SerializeField] private string unitName = "Drone";
    [SerializeField] private float hitBlendAmount = 0.75f;
    [SerializeField] private float hitBlendDuration = 0.25f;

    IEffectManager _effectManager;

    public void Initialize()
    {
        ServiceLocator.For(this)
            .Get(out _effectManager);

        _attachments.Init();
        _animationHandler.Init();
        _animatinEventHandler.Init();

        _attachments.Get<SpriteRenderer>(AttachType.SpriteRenderer).material = 
            new Material(_attachments.Get<SpriteRenderer>(AttachType.SpriteRenderer).material);
    }

    public void OnDamaged()
    {
        _effectManager.PlayHitEffect(_attachments.Get<SpriteRenderer>(AttachType.SpriteRenderer).material, gameObject.GetHashCode().ToString());
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

using DataEntity;
using DataEnum;
using DataHashAnim;
using UnityEngine;
using System;
using System.Linq;
using ObservableCollections;
using R3;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;

[RequireComponent(typeof(UnitAttachments))]
public class BaseUnit : MonoBehaviour
{
    [SerializeField, Required]
    private AnimationHandler _animHandler;
    [SerializeField, ShowIf("HasSupporter"), Required]
    protected SupporterUnit _supporterUnit;

    [SerializeField] private UNIT_TYPE unit_Type;
    [SerializeField] private bool HasSupporter = false;

    private UnitStat _stat;
    private ISoundService _soundService;
    private ICombatTextManager _textManager;

    [HideInInspector] 
    public UnitAttachments attachments;
    public UnitCombatInfo combatInfo;
    public CrowdControlUnit crowdControlUnit;
    public Action<BaseUnit> m_FinishedDying;

    DisposableBag d;

    public virtual void Initialize(EntityData data, int index)
    {
        _stat       = new UnitStat(data, index);
        combatInfo  = new UnitCombatInfo();
        crowdControlUnit = new CrowdControlUnit();
        attachments = GetComponent<UnitAttachments>();
        _animHandler.Init();

        ServiceLocator.For(this)
            .Get(out _soundService)
            .Get(out _textManager);

        // Add들만 합치기 (키 포함)
        var addStream = crowdControlUnit.EffectDictionary.Select(kv => kv.Value.ObserveAdd().Select(ev => new { Element = kv.Key, ev.Value, ev.Index })).Merge();

        // Remove들만 합치기 (키 포함)
        var removeStream = crowdControlUnit.EffectDictionary.Select(kv => kv.Value.ObserveRemove().Select(ev => new { Element = kv.Key, ev.Value, ev.Index })).Merge();

        var removeSub = removeStream.Subscribe(ev =>
            {
                Debug.Log($"Removed CC\nElement : {ev.Element}, Name : {ev.Value.GetType()}, Index : {ev.Index}");
            }
        ).AddTo(this); 

        if (HasSupporter)
            _supporterUnit.Initialize();
    }

    public void OnDamaged(IDamage damage)
    {
        attachments.GetSpriteRenderer().color = Color.red;
        attachments.GetSpriteRenderer().DOBlendableColor(Color.white, 0.25f);

        if(this is PlayerUnit)
            _animHandler.ChangeAnimation(AnimCombat.HIT);

        if(HasSupporter)
        {
            _supporterUnit.OnDamaged();
        }

        _textManager.OnDamage(this, damage);
    }

    public void OnHealed(float value)
    {
        // TODO : Heal Logic
    }

    public async virtual UniTask OnDie()
    {
        _animHandler.ChangeAnimation(AnimCombat.DEATH);

        if(HasSupporter)
        {
            _supporterUnit.OnDie(combatInfo).Forget();
        }
        if (this is PlayerUnit) {
            _soundService.PlayEffectSound("Die");
            _soundService.PlayEffectSound("Hover", 2f);
        }
        else _soundService.PlayEffectSound("Die");
    }

    public AnimationHandler GetAnimationHandler()       => _animHandler;
    public UnitStat GetStat()                           => _stat;
    public UNIT_TYPE GetUnitType()                      => unit_Type;
}
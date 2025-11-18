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
public abstract class BaseUnit : MonoBehaviour
{
    [SerializeField, Required]
    private AnimationHandler _animHandler;
    [SerializeField, ShowIf("HasSupporter"), Required]
    protected SupporterUnit _supporterUnit;

    [SerializeField] private UNIT_TYPE _unitType;
    [SerializeField] private bool HasSupporter = false;

    protected UnitStat _stat;
    protected ISoundService _soundService;
    protected ICombatTextManager _textManager;

    [HideInInspector] 
    public UnitAttachments Attachments;
    public UnitCombatInfo CombatInfo;
    public CrowdControlUnit crowdControlUnit;
    public CombatEffectUnit combatEffectUnit;
    public Action<BaseUnit> m_FinishedDying;

    DisposableBag d;

    public void Initialize(EntityData data, int priority)
    {
        CombatInfo       = new UnitCombatInfo();
        crowdControlUnit = new CrowdControlUnit();
        combatEffectUnit = new CombatEffectUnit();
        Attachments      = GetComponent<UnitAttachments>();

        _stat            = new UnitStat(data, priority);

        _animHandler.Init();

        ServiceLocator.For(this)
            .Get(out _soundService)
            .Get(out _textManager);

        // Replace들만 합치기 (키 포함)
        var replaceStream = crowdControlUnit.EffectDictionary.Select(kv => kv.Value.ObserveReplace()).Merge();

        

        if (HasSupporter)
            _supporterUnit.Initialize();
    }

    public void OnDamaged(IDamage damage)
    {
        Attachments.GetSpriteRenderer().color = Color.red;
        Attachments.GetSpriteRenderer().DOBlendableColor(Color.white, 0.25f);

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
        if (HasSupporter)
            _supporterUnit.OnDie(CombatInfo).Forget();

        PlayDeathSound();

        using (var eventDisposer = new EventDisposer(new CombatEvent("DeathEvent")))
        {
            Attachments.GetSpriteRenderer().sortingLayerName = "Actor";

            bool isFinishedEvent = false;
            _animHandler.ChangeAnimation(AnimCombat.DEATH);
            CombatInfo.actionList.Add("OnFinishedDeath",
                () => OnFinshedDeath(() => isFinishedEvent = true)
            );

            await UniTask.WaitUntil(() => isFinishedEvent);
            Attachments.GetSpriteRenderer().sortingLayerName = "Character";
        }
    }

    public AnimationHandler GetAnimationHandler()       => _animHandler;
    public UnitStat GetStat()                           => _stat;
    public UNIT_TYPE GetUnitType()                      => _unitType;

    public abstract void OnFinshedDeath(Action done);
    public abstract void PlayDeathSound();
}
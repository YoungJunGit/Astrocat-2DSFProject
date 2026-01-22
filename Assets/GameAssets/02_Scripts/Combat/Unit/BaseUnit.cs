using DataEntity;
using DataEnum;
using UnityEngine;
using System;
using System.Linq;
using ObservableCollections;
using R3;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public interface IUpdatable
{
    public void OnRoundUpdate();
    public void OnTurnUpdate();
}

[RequireComponent(typeof(UnitAttachments))]
public abstract class BaseUnit : MonoBehaviour, IUpdateTimeline
{
    [SerializeField, Required, FoldoutGroup("Animation")]
    private AnimationHandler _animHandler;
    public AnimationHandler AnimationHandler => _animHandler;

    [SerializeField, Required, FoldoutGroup("Animation")]
    private AnimationEventHandler _animEventHandler;
    public IAnimationEventHandler AnimationEventHandler => _animEventHandler;

    [SerializeField, ShowIf("HasSupporter"), Required]
    protected SupporterUnit _supporterUnit;
    public SupporterUnit SupporterUnit => _supporterUnit;

    [SerializeField] private UNIT_TYPE _unitType;
    [SerializeField] private float hitBlendAmount = 0.75f;
    [SerializeField] private float hitBlendDuration = 0.25f;
    [SerializeField] private bool HasSupporter = false;

    private UnitAttachments _attachments;
    public UnitAttachments Attachments => _attachments;

    protected UnitStat _stat;
    protected ISoundService _soundService;
    protected IEffectManager _effectManager;
    protected ICombatTextManager _textManager;
    protected ICrowdControlManager _crowdControlManager;

    public UnitCombatInfo CombatInfo;
    public CrowdControlUnit CCUnit;
    public CombatEffectUnit CEUnit;

    public Action<BaseUnit> m_FinishedDying;
    private readonly List<IUpdatable> _updatableList = new();

    // temp
    public ELEMENT_TYPE My_Temp_Type;

    private const string HIT_BLEND_TWEEN_ID = "HIT_BLEND";

    public void Initialize(EntityData data, int priority)
    {
        _attachments = GetComponent<UnitAttachments>();
        CombatInfo = new UnitCombatInfo();
        CCUnit = new CrowdControlUnit();
        CEUnit = new CombatEffectUnit();
        _stat = new UnitStat(data, priority);

        My_Temp_Type = data.Temp_Type;
        Attachments.GetSpriteRenderer().material = new Material(Attachments.GetSpriteRenderer().material);

        _animHandler.Init();
        _animEventHandler.Init();

        ServiceLocator.For(this)
            .Get(out _soundService)
            .Get(out _textManager)
            .Get(out _effectManager)
            .Get(out _crowdControlManager);

        TimelinePublisher.SubscribeObserver(this);

        _updatableList.Add(_stat.ModifierStat.Mediator);
        _updatableList.Add(CCUnit);

        if (HasSupporter)
            _supporterUnit.Initialize();
    }

    public void GetDamage(DamageResult damageInfo, ElementGaugeResult elementGaugeResult = null)
    {
        if(_stat.HP <= 0f) return;

        _effectManager.PlayHitEffect(Attachments.GetSpriteRenderer().material, _stat.CoreStat.ID);

        if (this is PlayerUnit)
        {
            _animHandler.ChangeAnimation(ANIMATION.HIT);
        }

        if (HasSupporter)
        {
            _supporterUnit.OnDamaged();
        }

        _stat.GetDamaged(damageInfo.DamageValue);
        _textManager.OnDamage(Attachments.GetHitBox().bounds, damageInfo);

        if (_stat.HP <= 0f)
        {
            OnDie().Forget();
            return;
        }

        if (elementGaugeResult != null && elementGaugeResult.ElementType != ELEMENT_TYPE.NONE)
        {
            CCUnit.BringLastAttacker = () => elementGaugeResult.Attacker;
            _stat.IncreaseElementGauge(elementGaugeResult.ElementType, elementGaugeResult.ElementGaugeIncreaseValue);
        }
    }

    public void GetHeal(float value)
    {
        if (_stat.HP <= 0f) return;

        float realValue = _stat.GetHealed(value);
        _textManager.OnHeal(Attachments.GetHitBox().bounds, realValue);
    }

    public void AddSP(int cost)
    {
        if (cost != 0)
        {
            _stat.AddSP(cost);
        }
    }

    public void OnElementGaugeFull(ELEMENT_TYPE elementType)
    {
        if (elementType != ELEMENT_TYPE.NONE && elementType != ELEMENT_TYPE.ETC)
        {
            _crowdControlManager.AddCrowdControl(elementType, this, CCUnit.BringLastAttacker.Invoke());
        }
    }

    public async virtual UniTask OnRevive()
    {
        // TODO : Revive Logic
    }

    public async virtual UniTask OnDie()
    {
        if (HasSupporter)
            _supporterUnit.OnDie(CombatInfo).Forget();

        PlayDeathSound();

        using (var eventDisposer = new EventDisposer(new CombatEvent("DeathEvent")))
        {
            ChangeSortingLayer("Actor");
            await _animHandler.ChangeAnimationAsync(ANIMATION.DEATH);
            await OnFinshedDeathAnim();
            ChangeSortingLayer("Character");
        }

        TimelinePublisher.DiscribeObserver(this);
    }

    public void RoundUpdate()
    {
        foreach (var updatable in _updatableList)
        {
            updatable.OnRoundUpdate();
        }
    }

    public void TurnUpdate()
    {
        foreach (var updatable in _updatableList)
        {
            updatable.OnTurnUpdate();
        }
    }

    public void ChangeSortingLayer(string layer)
    {
        Attachments.GetSpriteRenderer().sortingLayerName = layer;

        if(HasSupporter && _supporterUnit != null)
            _supporterUnit.Attachments.GetSpriteRenderer().sortingLayerName = layer;
    }

    public UnitStat GetStat() => _stat;
    public UNIT_TYPE GetUnitType() => _unitType;

    protected abstract UniTask OnFinshedDeathAnim();
    protected abstract void PlayDeathSound();
}
using DataEntity;
using DataEnum;
using DataHashAnim;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System;
using Unity.VisualScripting;
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

    [HideInInspector] 
    public UnitAttachments attachments;
    public UnitCombatInfo combatInfo;
    public CrowdControlUnit crowdControlUnit;
    public Action<BaseUnit> m_FinishedDying;
    
    public virtual void Initialize(EntityData data, int index)
    {
        _stat       = new UnitStat(data, index);
        combatInfo  = new UnitCombatInfo();
        attachments = GetComponent<UnitAttachments>();
        _animHandler.Init();

        ServiceLocator.For(this).Get(out _soundService);

        if (HasSupporter)
            _supporterUnit.Initialize();
    }

    public void OnDamaged(float value)
    {
        attachments.GetSpriteRenderer().color = Color.red;
        attachments.GetSpriteRenderer().DOBlendableColor(Color.white, 0.25f);

        if(this is PlayerUnit)
            _animHandler.ChangeAnimation(AnimCombat.HIT);

        if(HasSupporter)
        {
            _supporterUnit.OnDamaged();
        }
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
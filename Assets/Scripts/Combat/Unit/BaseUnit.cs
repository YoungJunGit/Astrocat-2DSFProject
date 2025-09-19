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

    private List<Buff> buffList = new List<Buff>();
    private UnitStat _stat;
    private CrowdControlManager _crowdControlManager = new();

    [HideInInspector] public UnitAttachments attachments;
    public UnitCombatInfo combatInfo;
    public Action<Buff> m_AddBuff;
    public Action<BaseUnit> m_FinishedDying;
    
    public virtual void Initialize(EntityData data, int index)
    {
        attachments = GetComponent<UnitAttachments>();
        combatInfo = new UnitCombatInfo();
        _animHandler.Init();

        _stat = new UnitStat(data, index);

        _crowdControlManager.Init(this);

        if (HasSupporter)
            _supporterUnit.Initialize();
    }

    // TODO : Buff Test
    public void AddBuff(Buff newBuff)
    {
        Buff buff = buffList.Find(element => element.Buff_Name == newBuff.Buff_Name);

        if (buff == null)
        {
            buffList.Add(newBuff);
            _stat.AddSpeed((float)newBuff.Speed_Value);
        }
        else
        {
            buffList[buffList.IndexOf(buff)] = newBuff;
        }

        m_AddBuff?.Invoke(newBuff);
    }

    public void RemoveBuff(Buff newBuff)
    {
        buffList.Remove(newBuff);

        _stat.AddSpeed(-(float)newBuff.Speed_Value);
    }

    public void OnEndRound()
    {
        for (int i = buffList.Count - 1; i >= 0; i--)
        {
            buffList[i].Buff_Duration -= 1;
            if (buffList[i].Buff_Duration <= 0)
                RemoveBuff(buffList[i]);
        }
    }

    public void OnDamaged(float value, bool isCritical)
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

    }

    public async virtual UniTask OnDie()
    {
        _animHandler.ChangeAnimation(AnimCombat.DEATH);

        if(HasSupporter)
        {
            _supporterUnit.OnDie(combatInfo).Forget();
        }
    }

    public AnimationHandler GetAnimationHandler() => _animHandler;
    public CrowdControlManager GetCrowdControlManager() => _crowdControlManager;
    public UnitStat GetStat() => _stat;
    public UNIT_TYPE GetUnitType() => unit_Type;
}
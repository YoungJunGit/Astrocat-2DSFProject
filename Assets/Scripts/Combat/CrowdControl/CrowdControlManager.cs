using DataEntity;
using DataEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public interface ICrowdControlManager
{
    public void AddCrowdControl(ELEMENT_TYPE element_type, BaseUnit target, BaseUnit caster);
    public void RemoveCrowdControl(ELEMENT_TYPE element_type, BaseUnit target);
}

[CreateAssetMenu(fileName = "CrowdControlManager", menuName = "Manager/CrowdControlManager", order = 1)]
public class CrowdControlManager : ScriptableObject, ICrowdControlManager
{
    public record CCContext(ElementStatusData Data, ICombatEffectManager effectManager, BaseUnit Target, BaseUnit Caster)
    {
        public ElementStatusData Data { get; } = Data;
        public ICombatEffectManager effectManager { get; } = effectManager;
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }

    private DataHandler _dataHandler;
    private ICombatEffectManager _effectManager;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _dataHandler)
            .Get(out _effectManager);
    }

    public void AddCrowdControl(ELEMENT_TYPE element_type, BaseUnit target, BaseUnit caster)
    {
        var previousElement = target.CCUnit.Previous_Element_Type;
        if (previousElement != element_type && previousElement != ELEMENT_TYPE.NONE)
        {
            #region [Chaos상태이상 저장]
            var chaos = target.CCUnit.GetNonStackCC(ELEMENT_TYPE.ETC) as IChaos;
            // If Chaos Element_Status_Effect not exist
            if (chaos == null)
            {
                target.CCUnit.Add(ELEMENT_TYPE.ETC);
                var crowdControl = CrowdControlFactory.CreateCC(ELEMENT_STATUS_CATEGORY.CHAOS);
                target.CCUnit.AddNonStackCC(ELEMENT_TYPE.ETC, crowdControl);
                var context = CreateContext(crowdControl, target, caster);
                if (context != null)
                {
                    crowdControl.ApplyCrowdControl(context);
                }
            }
            // If Chaos Element_Status_Effect already exists -> Save Update Action
            else
            {
                chaos.ReapplyCrowdControl(target.CCUnit.Previous_Element_Type);
            }
            #endregion
        }

        // Check element type dictionary
        if (target.CCUnit.CurrentEffects.TryGetValue(element_type, out var list))
        {
            #region [상태이상 정보만 저장]
            ELEMENT_STATUS_CATEGORY category;
            // If Basic Element_Status_Effect exists
            if (list.Count > 0 && list.ToList().Exists(e => e == ElementStatusRuleTable.GetBasic(element_type)))
            {
                category = ElementStatusRuleTable.GetEnhanced(element_type);
            }
            // If Basic Element_Status_Effect not exist
            else
            {
                category = ElementStatusRuleTable.GetBasic(element_type);
            }
            target.CCUnit.Add(element_type);
            #endregion

            #region [ICrowdControl 저장]
            ICrowdControl crowdControl = CrowdControlFactory.CreateCC(category);
            var context = CreateContext(crowdControl, target, caster);
            if (context != null)
            {
                // 스택기반 상태이상 저장
                if (ElementStatusRuleTable.IsStackableElement(element_type))
                {
                    crowdControl.ApplyCrowdControl(context);
                    target.CCUnit.AddStackCC(element_type, crowdControl);
                }
                // 지속턴기반 상태이상 저장
                else
                {
                    var ac = target.CCUnit.GetNonStackCC(element_type) as AttributeControl;
                    // 만약 AttributeControl 상태이상이 저장되어있지 않은 상태라면 저장하기
                    if (ac == null)
                    {
                        crowdControl.ApplyCrowdControl(context);
                        target.CCUnit.AddNonStackCC(element_type, crowdControl);
                    }
                    // 만약 AttributeControl 상태이상이 저장되어있다면 
                    else
                    {
                        // 새로 추가될 예정이었던 상태이상이 중첩 상태이상이라면 Duration++
                        if(crowdControl is AttributeControl)
                        {
                            ac.AddDuration();
                        }
                    }
                }
            }
            #endregion
        }
        else
        {
            Debug.LogWarning($"There is no Element Type Such as : {element_type}");
        }
    }

    public void RemoveCrowdControl(ELEMENT_TYPE element_type, BaseUnit target)
    {
        target.CCUnit.Remove(element_type);

        if(ElementStatusRuleTable.IsStackableElement(element_type))
        {
            target.CCUnit.RemoveStackCC(element_type);
        }
        else
        {
            target.CCUnit.RemoveNonStackCC(element_type);
        }
    }

    private CCContext CreateContext(ICrowdControl cc, BaseUnit target, BaseUnit caster)
    {
        var elementStatusData = _dataHandler.FindElementStatusData(cc.ID);

        if (elementStatusData == null)
        {
            Debug.LogWarning($"No CC Data : {cc}");
            return null;
        }

        var context = new CCContext(elementStatusData, _effectManager, target, caster);

        return context;
    }
}
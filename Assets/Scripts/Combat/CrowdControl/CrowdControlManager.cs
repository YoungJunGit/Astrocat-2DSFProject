
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControlManager", menuName = "Manager/CrowdControlManager", order = 1)]
public class CrowdControlManager : ScriptableObject
{
    public enum CrowdControlType
    {
        Stun,
        Burn,
        Confusion,
        Exposure,
        Suppression,
        Overload,
    }
    public record CCContext(DamageFactory DamageFactory, BaseUnit Target, BaseUnit Caster)
    {
        public DamageFactory DamageFactory { get; } = DamageFactory;
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }
    
    private DamageFactory _damageFactory;
    
    public void Init()
    {
        ServiceLocator.For(this).Get(out _damageFactory);
    }
    
    public void AddCrowdControl(CrowdControlType crowdControlType, BaseUnit target, BaseUnit caster)
    {
        var context = new CCContext(_damageFactory, target, caster);
        var crowdControl = CrowdControlFactory.CreateCrowdControl(crowdControlType);
        if (crowdControl == null)
            return;

        crowdControl.ApplyCrowdControl(context);

        target.crowdControlUnit.Add(crowdControl); // TODO : 중첩 상태이상 업데이트
    }

    public bool RemoveCrowdControl(CrowdControlType type, BaseUnit target)
    {
        switch(type)
        {
            case CrowdControlType.Stun:
                return target.crowdControlUnit.Remove<StunCC>();
            case CrowdControlType.Burn:
                return target.crowdControlUnit.Remove<BurnCC>();
            case CrowdControlType.Suppression:
                return target.crowdControlUnit.Remove<SuppressionCC>();
            case CrowdControlType.Exposure:
                return target.crowdControlUnit.Remove<ExposeCC>();
            case CrowdControlType.Overload:
                return target.crowdControlUnit.Remove<FloodCC>();
            case CrowdControlType.Confusion:
                return target.crowdControlUnit.Remove<ConfusionCC>();
        }

        return false;
    }
}
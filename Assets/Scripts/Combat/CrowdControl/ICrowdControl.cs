using System.Collections.Generic;
using DataEntity;
using DataEnum;
using UnityEngine;
using UnityEngine.LightTransport;
using static CrowdControlManager;
using static CombatEffectManager;

public interface ICrowdControl
{
    public string ID { get; }
    public void ApplyCrowdControl(CCContext context = null);
    public void Dispose();
}

public interface IBasicCrowdControl { }

public interface IEnhancedCrowdControl { }

public abstract class CrowdControlBase : ICrowdControl
{
    protected List<IEffectable> effectList = new();

    public virtual void ApplyCrowdControl(CCContext context)
    {
        List<EffectInfo> list = new List<EffectInfo>();
        CreateEffectInfoList(context.Data, list);

        foreach (var effectInfo in list)
        {
            var effect = context.effectManager.AddCombatEffect(CombatEffectUnit.COMBAT_EFFECT_TYPE.CC, effectInfo, new EffectContext(context.Target, context.Caster));
            effectList.Add(effect);
        }
    }

    public void Dispose()
    {
        foreach (var effect in effectList)
        {
            effect.Dispose();
        }
    }

    public abstract string ID { get; }
    public abstract void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list);
}

public class Stun : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011001";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001032",
            data.Element_Status_Name, 
            0.0f, 
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Burn : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011002";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001030", 
            data.Element_Status_Name, 
            (float)data.Element_Status_Value[0], 
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Contamination : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011003";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Suppress : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011004";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Strange : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011005";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001033",
            data.Element_Status_Name,
            0.0f,
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Silence : CrowdControlBase, IBasicCrowdControl
{
    public override string ID => "40011006";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001034",
            data.Element_Status_Name,
            0.0f,
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Weakness : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022001";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001032",
            data.Element_Status_Name,
            0.0f,
            data.Element_Status_Duration_Turn
        ));
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Overheat : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022002";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001030",
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Exposure : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022003";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Bind : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022004";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Corrode : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022005";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001033",
            data.Element_Status_Name,
            0.0f,
            data.Element_Status_Duration_Turn
        ));
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Dominate : CrowdControlBase, IEnhancedCrowdControl
{
    public override string ID => "40022006";

    public override void CreateEffectInfoList(ElementStatusData data, List<EffectInfo> list)
    {
        list.Add(new EffectInfo
        (
            "30001034",
            data.Element_Status_Name,
            0.0f,
            data.Element_Status_Duration_Turn
        ));
        list.Add(new EffectInfo
        (
            data.Buff_Table_ID,
            data.Element_Status_Name,
            (float)data.Element_Status_Value[0],
            data.Element_Status_Duration_Turn
        ));
    }
}

public class Chaos : ICrowdControl
{
    public string ID => "40033001";
    private readonly Dictionary<ELEMENT_TYPE, string> chaosIDs = new Dictionary<ELEMENT_TYPE, string>()
    {
        { ELEMENT_TYPE.PHYSICAL, "30001015" },
        { ELEMENT_TYPE.FIRE, "30001016" },
        { ELEMENT_TYPE.RADIATION, "30001017" },
        { ELEMENT_TYPE.GRAVITY, "30001018" },
        { ELEMENT_TYPE.VOID, "30001019" },
        { ELEMENT_TYPE.HOLY, "30001020" }
    };
    private CCContext context;

    public void ApplyCrowdControl(CCContext context = null)
    {
        this.context = context;
        var chaosEffectID = chaosIDs[context.Target.crowdControlUnit.Previous_Element_Type];
        var effectInfo = CreateEffectInfoList(chaosEffectID, (float)context.Data.Element_Status_Value[0], context.Data.Element_Status_Duration_Turn, context.Data.Element_Status_Name);

    }

    public void Dispose()
    {
        
    }

    public EffectInfo CreateEffectInfoList(string ID, float value, int duration, string name)
    {
        return new EffectInfo(ID, name, value, duration);
    }
}
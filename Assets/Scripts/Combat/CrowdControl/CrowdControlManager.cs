using AssetInventory;
using DataEntity;
using DataEnum;
using Language.Lua;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static AYellowpaper.SerializedCollections.SerializedDictionarySample;

public interface ICrowdControlManager
{
    public void AddCrowdControl(ELEMENT_TYPE element_type, BaseUnit target, BaseUnit caster);
}

[CreateAssetMenu(fileName = "CrowdControlManager", menuName = "Manager/CrowdControlManager", order = 1)]
public class CrowdControlManager : ScriptableObject, ICrowdControlManager
{
    private readonly Dictionary<Type, string> _crowdControlIDs = new()
    {
        { typeof(Stun), "40011001"},
        { typeof(Burn), "40011002"},
        { typeof(Contamination), "40011003"},
        { typeof(Suppress), "40011004"},
        { typeof(Strange), "40011005"},
        { typeof(Silence), "40011006"},
        { typeof(Weakness), "40022001"},
        { typeof(Overheat), "40022002"},
        { typeof(Exposure), "40022003"},
        { typeof(Bind), "40022004"},
        { typeof(Corrode), "40022005"},
        { typeof(Dominate), "40022006"},
        { typeof(Chaos), "40033001"},
    };
    private readonly Dictionary<ELEMENT_TYPE, (ELEMENT_STATUS_CATEGORY Basic, ELEMENT_STATUS_CATEGORY Enhanced)> _elementDic = new()
    {
        { ELEMENT_TYPE.PHYSICAL, (ELEMENT_STATUS_CATEGORY.STUN, ELEMENT_STATUS_CATEGORY.WEAKNESS) },
        { ELEMENT_TYPE.FIRE, (ELEMENT_STATUS_CATEGORY.BURN, ELEMENT_STATUS_CATEGORY.OVERHEAT) },
        { ELEMENT_TYPE.RADIATION, (ELEMENT_STATUS_CATEGORY.CONTAMINATION, ELEMENT_STATUS_CATEGORY.EXPOSURE) },
        { ELEMENT_TYPE.GRAVITY, (ELEMENT_STATUS_CATEGORY.SUPPRESS, ELEMENT_STATUS_CATEGORY.BIND) },
        { ELEMENT_TYPE.VOID, (ELEMENT_STATUS_CATEGORY.STRANGE, ELEMENT_STATUS_CATEGORY.CORRODE) },
        { ELEMENT_TYPE.HOLY, (ELEMENT_STATUS_CATEGORY.SILENCE, ELEMENT_STATUS_CATEGORY.DOMINATE) },
    };

    public record CCContext(ElementStatusData Data, BaseUnit Target, BaseUnit Caster)
    {
        public ElementStatusData Data { get; } = Data;
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }

    private DataHandler _dataHandler;

    public void Init()
    {
        ServiceLocator.For(this).Get(out _dataHandler);
    }

    public void AddCrowdControl(ELEMENT_TYPE element_type, BaseUnit target, BaseUnit caster)
    {
        Action updateChaos = null;
        var previousElement = target.crowdControlUnit.Previous_Element_Type;
        if (previousElement != element_type && previousElement != ELEMENT_TYPE.NONE)
        {
            // Check element type dictionary
            if (target.crowdControlUnit.EffectDictionary.TryGetValue(ELEMENT_TYPE.ETC, out var chaosList))
            {
                // If Chaos Element_Status_Effect not exist
                if (chaosList.Count == 0)
                {
                    var crowdControl = CrowdControlFactory.CreateCC(ELEMENT_STATUS_CATEGORY.CHAOS);
                    target.crowdControlUnit.Add(ELEMENT_TYPE.ETC, crowdControl);
                    var context = CreateContext(crowdControl, (target, caster));

                    if (crowdControl != null && context != null)
                    {
                        updateChaos = () => crowdControl.ApplyCrowdControl(context);
                    }
                }
                // If Chaos Element_Status_Effect already exists -> Save Update Action
                else
                {
                    if (chaosList[0] != null)
                    {
                        updateChaos = () => chaosList[0].ApplyCrowdControl();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"There is no Element Type Such as : {element_type}");
            }
        }

        // Check element type dictionary
        if (target.crowdControlUnit.EffectDictionary.TryGetValue(element_type, out var list))
        {
            ICrowdControl crowdControl;
            CCContext context;
            // If Basic Element_Status_Effect exists
            if (list.Count > 0 && list.FirstOrDefault(e => !e.isUpgrade) is ICrowdControl found)
            {
                ELEMENT_STATUS_CATEGORY category = _elementDic[element_type].Enhanced;
                crowdControl = CrowdControlFactory.CreateCC(category);
                target.crowdControlUnit.Replace(element_type, found, crowdControl);
                context = CreateContext(crowdControl, (target, caster));
            }
            // If Basic Element_Status_Effect not exist
            else
            {
                ELEMENT_STATUS_CATEGORY category = _elementDic[element_type].Basic;
                crowdControl = CrowdControlFactory.CreateCC(category);
                target.crowdControlUnit.Add(element_type, crowdControl);
                context = CreateContext(crowdControl, (target, caster));
            }

            if (crowdControl != null && context != null)
                crowdControl.ApplyCrowdControl(context);
        }
        else
        {
            Debug.LogWarning($"There is no Element Type Such as : {element_type}");
        }

        // Update Chaos Effect
        updateChaos?.Invoke();
    }

    public static void RemoveCrowdControl(ELEMENT_TYPE element_type, BaseUnit target)
    {
        if (target.crowdControlUnit.EffectDictionary.TryGetValue(element_type, out var foundList))
        {
            for (var i = foundList.Count - 1; i >= 0; i--)
            {
                target.crowdControlUnit.Remove(element_type, foundList[i]);
            }
        }
    }

    private CCContext CreateContext(ICrowdControl cc, (BaseUnit target, BaseUnit caster) unit)
    {
        if (!_crowdControlIDs.TryGetValue(cc.GetType(), out var ID))
        {
            Debug.LogWarning($"No type found in dictionary : {cc.GetType()}");
            return null;
        }

        var data = _dataHandler.FindElementStatusData(ID);

        if (data == null)
        {
            Debug.LogWarning($"No Data found from this ID : {ID}");
            return null;
        }

        var context = new CCContext(data, unit.target, unit.caster);

        return context;
    }
}
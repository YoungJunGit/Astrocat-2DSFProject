using DataEntity;
using DataEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControlManager", menuName = "Manager/CrowdControlManager", order = 1)]
public class CrowdControlManager : ScriptableObject
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

    public record CCContext(ElementStatusData Data, DamageFactory DamageFactory, BaseUnit Target, BaseUnit Caster)
    {
        public ElementStatusData Data { get; } = Data;
        public DamageFactory DamageFactory { get; } = DamageFactory;
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }

    private DamageFactory _damageFactory;
    private DataHandler dataHandler;

    public void Init()
    {
        ServiceLocator.For(this).Get(out _damageFactory);
        ServiceLocator.For(this).Get(out dataHandler);
    }

    public void AddCrowdControl(ELEMENT_TYPE element_type, BaseUnit target, BaseUnit caster)
    {
        ELEMENT_STATUS_CATEGORY category = _elementDic[element_type].Basic;
        if (target.crowdControlUnit.EffectDictionary.TryGetValue(element_type, out var list))
        {
            if (list != null && list.Count > 0 && list.FirstOrDefault(e => !e.isUpgrade) is ICrowdControl found)
            {
                target.crowdControlUnit.Remove(element_type, found);
                category = _elementDic[element_type].Enhanced;
            }
        }
        else
        {
            Debug.LogWarning($"There is no Element Type Such as : {element_type}");
        }

        ICrowdControl crowdControl = CrowdControlFactory.CreateCC(category);
        target.crowdControlUnit.Add(element_type, crowdControl);
        var tmp = target.crowdControlUnit.EffectDictionary[element_type][0];
        
        if (!_crowdControlIDs.TryGetValue(crowdControl.GetType(), out var ID))
        {
            Debug.LogWarning($"No type found in dictionary : {crowdControl.GetType()}");
            return;
        }

        var data = dataHandler.FindElementStatusData(ID);

        if (data == null)
        {
            Debug.LogWarning($"No Data found from this ID : {ID}");
            return;
        }

        var context = new CCContext(data, _damageFactory, target, caster);

        crowdControl.ApplyCrowdControl(context);
    }

    public void RemoveCrowdControl(ELEMENT_TYPE element_type, BaseUnit target)
    {
        if (target.crowdControlUnit.EffectDictionary.TryGetValue(element_type, out var foundList))
        {
            for(var i = foundList.Count - 1; i >= 0; i--)
            {
                target.crowdControlUnit.Remove(element_type, foundList[i]);
            }
        }
    }
}
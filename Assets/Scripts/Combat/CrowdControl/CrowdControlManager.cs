using DataEntity;
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
    
    public void AddCrowdControl<T>(BaseUnit target, BaseUnit caster) where T : ICrowdControl, new()
    {
        string ID;
        if (!_crowdControlIDs.TryGetValue(typeof(T), out ID))
        {
            Debug.Log($"No type found in dictionary : {typeof(T)}");
            return;
        }

        ElementStatusData data = dataHandler.FindElementStatusData(ID);

        if (data == null)
        {
            Debug.Log($"No Data found from this ID : {ID}");
            return;
        }

        var context = new CCContext(data, _damageFactory, target, caster);
        var crowdControl = new T();

        if (crowdControl != null)
        {
            //target.crowdControlUnit._effectList.TryGetValue(crowdControl, out _); // TODO : 중첩 상태이상 업데이트

            if (target.crowdControlUnit.Add(crowdControl))
            {
                crowdControl.ApplyCrowdControl(context);
            }
        }
    }

    public bool RemoveCrowdControl<T>(BaseUnit target) where T : ICrowdControl
    {
        ICrowdControl toRemove = target.crowdControlUnit._effectList.FirstOrDefault(e => e is T);

        return target.crowdControlUnit.Remove(toRemove);
    }
}
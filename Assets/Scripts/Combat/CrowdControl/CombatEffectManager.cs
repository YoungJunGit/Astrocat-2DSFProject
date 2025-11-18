using System;
using System.Collections.Generic;
using DataEnum;
using Language.Lua;
using UnityEngine;
using static CombatEffectUnit;
using static CombatEffectManager;

public interface ICombatEffectManager
{
    public IEffectable AddCombatEffect(COMBAT_EFFECT_TYPE type, EffectInfo info, EffectContext context);
}

[CreateAssetMenu(fileName = "CombatEffectManager", menuName = "Manager/CombatEffectManager", order = 1)]
public class CombatEffectManager : ScriptableObject , ICombatEffectManager
{
    private readonly Dictionary<string, IEffectable> _effectFactory = new()
    {
        { "30001001", null }, { "30001002", null }, { "30001003", null }, { "30001004", null }, { "30001005", null },
        { "30001006", null }, { "30001007", null }, { "30001008", null }, { "30001009", null }, { "30001010", null },
        { "30001011", null }, { "30001012", null }, { "30001013", null }, { "30001014", null }, { "30001015", null },
        { "30001016", null }, { "30001017", null }, { "30001018", null }, { "30001019", null }, { "30001020", null },
        { "30001021", null }, { "30001022", null }, { "30001023", null }, { "30001024", null }, { "30001025", null },
        { "30001026", null }, { "30001027", null }, { "30001028", null }, { "30001029", null }, { "30001030", new BurnEffect() },
        { "30001031", null }, { "30001032", new StunEffect() }, { "30001033", new StrangeEffect() }, { "30001034", new SilenceEffect() }
    };
    public record EffectInfo(string ID, string Name, float Value, int Duration)
    {
        public string ID { get; } = ID;
        public string Name { get; } = Name;
        public float Value { get; } = Value;
        public int Duration { get; } = Duration;
    }
    public record EffectContext(BaseUnit Target, BaseUnit Caster)
    {
        public BaseUnit Target { get; } = Target;
        public BaseUnit Caster { get; } = Caster;
    }


    private DataHandler _dataHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out _dataHandler);
    }

    public IEffectable AddCombatEffect(COMBAT_EFFECT_TYPE type, EffectInfo info, EffectContext context)
    {
        BaseUnit target = context.Target;

        if(_effectFactory.TryGetValue(info.ID, out var combatEffect))
        {
            target.combatEffectUnit.Add(type, combatEffect);
        }

        combatEffect.Apply(info, context);
        return combatEffect;
    }
}
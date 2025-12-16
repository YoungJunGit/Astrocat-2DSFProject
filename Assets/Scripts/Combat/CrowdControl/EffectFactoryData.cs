using System.Collections.Generic;
using System;
using DataEnum;

public static class EffectFactoryData
{
    private static readonly Dictionary<string, Func<IEffectable>> _effectFactory = new()
    {
        { "Burn", () => new BurnEffect(EffectTrait.Stackable) },
        { "Contamination", () => new SPConsumptionRateEffect(EffectTrait.Stackable) },
        { "Suppress", () => new SpeedEffect(EffectTrait.Stackable) },
        { "Weakness", () => new DamageTakenMultiplierEffect(EffectTrait.Duration) },
        { "Dominate", () => new AttackEffect(EffectTrait.Duration) },
        { "PhysicalGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.PHYSICAL_GAUGE_RESISTANCE) },
        { "FireGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.FIRE_GAUGE_RESISTANCE) },
        { "RadiationGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.RADIATION_GAUGE_RESISTANCE) },
        { "GravityGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.GRAVITY_GAUGE_RESISTANCE) },
        { "VoidGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.VOID_GAUGE_RESISTANCE) },
        { "HolyGaugeResistance", () => new GaugeResistance(EffectTrait.Duration, BUFF_TYPE.HOLY_GAUGE_RESISTANCE) }
    };

    public static Func<IEffectable> GetEffectFactory(string ID) => _effectFactory.TryGetValue(ID, out var factory) ? factory : null;
}
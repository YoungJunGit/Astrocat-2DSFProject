using System.Collections.Generic;
using System;
using DataEnum;

public static class EffectFactoryData
{
    private static readonly Dictionary<string, Func<IEffectable>> _effectFactory = new()
    {
        { "Burn", () => new BurnEffect() },
        { "Weakness", () => new DamageTakenMultiplierEffect() },
        { "PhysicalGaugeResistance", () => new GaugeResistance(BUFF_TYPE.PHYSICAL_GAUGE_RESISTANCE) },
        { "FireGaugeResistance", () => new GaugeResistance(BUFF_TYPE.FIRE_GAUGE_RESISTANCE) },
        { "RadiationGaugeResistance", () => new GaugeResistance(BUFF_TYPE.RADIATION_GAUGE_RESISTANCE) },
        { "GravityGaugeResistance", () => new GaugeResistance(BUFF_TYPE.GRAVITY_GAUGE_RESISTANCE) },
        { "VoidGaugeResistance", () => new GaugeResistance(BUFF_TYPE.VOID_GAUGE_RESISTANCE) },
        { "HolyGaugeResistance", () => new GaugeResistance(BUFF_TYPE.HOLY_GAUGE_RESISTANCE) }
    };

    public static Func<IEffectable> GetEffectFactory(string ID) => _effectFactory.TryGetValue(ID, out var factory) ? factory : null;
}
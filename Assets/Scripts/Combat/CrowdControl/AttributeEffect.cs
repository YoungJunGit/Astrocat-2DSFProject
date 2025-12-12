using System.Collections.Generic;
using DataEnum;
using static TimelinePublisher;

public abstract class AttributeEffect : BaseEffect<float>
{
    protected override BasicStatModifier<float> CreateModifier()
    {
        float value = Info.Value;

        var modifer = new BasicStatModifier<float>(
            buffType,
            updateType,
            (v) => v + value,
            CreateTimer()
        );

        return modifer;
    }
}

public class DamageTakenMultiplierEffect : AttributeEffect, IDurationEffect
{
    public override BUFF_TYPE buffType => BUFF_TYPE.DAMAGE_TAKEN_MULTIPLIER;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}

public class GaugeResistance : AttributeEffect, IDurationEffect
{
    private readonly BUFF_TYPE type;

    public GaugeResistance(BUFF_TYPE type)
    {
        this.type = type;
    }

    public override BUFF_TYPE buffType => type;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}
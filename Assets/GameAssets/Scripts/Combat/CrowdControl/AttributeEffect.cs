using System.Collections.Generic;
using DataEnum;

public abstract class AttributeEffect : BaseEffect<float>
{
    protected AttributeEffect(EffectTrait trait) : base(trait) { }

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

public class AttackEffect : AttributeEffect
{
    public AttackEffect(EffectTrait trait) : base(trait) { }
    public override BUFF_TYPE buffType => BUFF_TYPE.ATTACK;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}

public class SpeedEffect : AttributeEffect
{
    public SpeedEffect(EffectTrait trait) : base(trait) { }
    public override BUFF_TYPE buffType => BUFF_TYPE.SPEED;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}

public class SPConsumptionRateEffect : AttributeEffect
{
    public SPConsumptionRateEffect(EffectTrait trait) : base(trait) { }
    public override BUFF_TYPE buffType => BUFF_TYPE.SP_CONSUMPTION_RATE;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}

public class DamageTakenMultiplierEffect : AttributeEffect
{
    public DamageTakenMultiplierEffect(EffectTrait trait) : base(trait) { }
    public override BUFF_TYPE buffType => BUFF_TYPE.DAMAGE_TAKEN_MULTIPLIER;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}

public class GaugeResistance : AttributeEffect
{
    private readonly BUFF_TYPE type;

    public GaugeResistance(EffectTrait trait, BUFF_TYPE type) : base(trait) => this.type = type;

    public override BUFF_TYPE buffType => type;
    public override UPDATE_TYPE updateType => UPDATE_TYPE.TURN;
}
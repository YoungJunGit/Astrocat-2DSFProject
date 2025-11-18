using System.Collections.Generic;
using DataEnum;
using static TimelinePublisher;

public class StunEffect : BaseEffect<int>
{
    protected override BasicStatModifier<int> CreateModifier()
    {
        return new BasicStatModifier<int>(BUFF_TYPE.STUN, UPDATE_TYPE.TURN, (v) => v++, CreateTimer());
    }

    protected override EffectTimer CreateTimer()
    {
        return new EffectTimer(1);
    }
}

public class StrangeEffect : BaseEffect<int>
{
    protected override BasicStatModifier<int> CreateModifier()
    {
        return new BasicStatModifier<int>(BUFF_TYPE.STRANGE, UPDATE_TYPE.TURN, (v) => v++, CreateTimer());
    }

    protected override EffectTimer CreateTimer()
    {
        return new EffectTimer(1);
    }
}

public class SilenceEffect : BaseEffect<int>
{
    protected override BasicStatModifier<int> CreateModifier()
    {
        return new BasicStatModifier<int>(BUFF_TYPE.SILENCE, UPDATE_TYPE.TURN, (v) => v++, CreateTimer());
    }

    protected override EffectTimer CreateTimer()
    {
        return new EffectTimer(1);
    }
}
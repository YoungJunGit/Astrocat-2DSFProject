using System.Collections.Generic;
using DataEnum;
using static TimelinePublisher;

public abstract class ControlEffect : BaseEffect<int>
{
    protected abstract BUFF_TYPE buffType { get; }

    protected override BasicStatModifier<int> CreateModifier()
    {
        return new BasicStatModifier<int>(buffType, UPDATE_TYPE.TURN, (v) => v++, new EffectTimer(1));
    }
}

public class StunEffect : ControlEffect
{
    protected override BUFF_TYPE buffType => BUFF_TYPE.STUN;
}

public class StrangeEffect : ControlEffect
{
    protected override BUFF_TYPE buffType => BUFF_TYPE.STRANGE;
}

public class SilenceEffect : ControlEffect
{
    protected override BUFF_TYPE buffType => BUFF_TYPE.SILENCE;
}
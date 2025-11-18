using System.Collections.Generic;
using DataEnum;
using static TimelinePublisher;

public class StunEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override BasicStatModifier<ELEMENT_TYPE, int> CreateModifier()
    {
        return new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.PHYSICAL, (v) => v++, new EffectTimer(1));
    }
}

public class StrangeEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override BasicStatModifier<ELEMENT_TYPE, int> CreateModifier()
    {
        return new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.VOID, (v) => v++, new EffectTimer(1));
    }
}

public class SilenceEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override BasicStatModifier<ELEMENT_TYPE, int> CreateModifier()
    {
        return new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.HOLY, (v) => v++, new EffectTimer(1));
    }
}
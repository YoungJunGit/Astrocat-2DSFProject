using System.Collections.Generic;
using DataEnum;
using static TimelinePublisher;

public class StunEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override void CreateModifier()
    {
        modifier = new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.PHYSICAL, (v) => v++, new EffectTimer(PUBLISHER_TYPE.TURN, 1));
    }
}

public class StrangeEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override void CreateModifier()
    {
        modifier = new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.VOID, (v) => v++, new EffectTimer(PUBLISHER_TYPE.TURN, 1));
    }
}

public class SilenceEffect : BaseModiferEffect<ELEMENT_TYPE>
{
    public override void CreateModifier()
    {
        modifier = new BasicStatModifier<ELEMENT_TYPE, int>(ELEMENT_TYPE.HOLY, (v) => v++, new EffectTimer(PUBLISHER_TYPE.TURN, 1));
    }
}
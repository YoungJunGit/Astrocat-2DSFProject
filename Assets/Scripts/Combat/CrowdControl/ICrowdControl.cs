using UnityEngine;
using static CrowdControlManager;

public interface ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context);
    public void Dispose();
}

public interface IEveryTurnBased
{
    public void Apply();
}

public interface ITimeBased
{
    public TimelineTimer Timer { get; set; }
}

public interface IContantBased
{
    public BasicStatModifier<float> modifier { get; set; }
}

public class Stun : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        Context = context;
        Debug.Log($"{Context.Target} Stunned by {Context.Caster}");
    }

    public void Dispose()
    {
        
    }
}

public class Burn : ICrowdControl, IEveryTurnBased
{
    public CCContext Context { get; set; }
    private float damageValue;

    public void ApplyCrowdControl(CCContext context)
    {
        Context = context;
        Debug.Log($"{Context.Target} Burned by {Context.Caster}");

        
    }

    public void Apply()
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Contamination : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Suppress : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Strange : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Silence : ICrowdControl
{
    public CCContext Context { get; set; }

    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Weakness : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Overheat : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Exposure : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Bind : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Corrode : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Dominate : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Chaos : ICrowdControl
{
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}
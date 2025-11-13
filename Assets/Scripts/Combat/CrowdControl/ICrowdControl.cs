using UnityEngine;
using static CrowdControlManager;

public interface ICrowdControl
{
    public bool isUpgrade { get; }
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context = null);
    public void Dispose();
}

public class Stun : ICrowdControl
{
    public bool isUpgrade { get; } = false;
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

public class Burn : ICrowdControl
{
    public bool isUpgrade { get; } = false;
    public CCContext Context { get; set; }
    private float damageValue;

    public void ApplyCrowdControl(CCContext context)
    {
        Context = context;
        Debug.Log($"{Context.Target} Burned by {Context.Caster}");

        
    }

    public void Dispose()
    {
        
    }
}

public class Contamination : ICrowdControl
{
    public bool isUpgrade { get; } = false;
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
    public bool isUpgrade { get; } = false;
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
    public bool isUpgrade { get; } = false;
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
    public bool isUpgrade { get; } = false;
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
    public bool isUpgrade { get; } = true;
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        Context = context;
        Debug.Log($"{Context.Target} Weakness by {Context.Caster}");
    }

    public void Dispose()
    {
        
    }
}

public class Overheat : ICrowdControl
{
    public bool isUpgrade { get; } = true;
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
    public bool isUpgrade { get; } = false;
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
    public bool isUpgrade { get; } = true;
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
    public bool isUpgrade { get; } = true;
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
    public bool isUpgrade { get; } = true;
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
    public bool isUpgrade { get; } = false;
    public CCContext Context { get; set; }
    public void ApplyCrowdControl(CCContext context)
    {
        if(context != null)
            Debug.Log($"Add Chaos!");
        else
            Debug.Log($"Update Chaos!");
    }

    public void Dispose()
    {
        
    }
}
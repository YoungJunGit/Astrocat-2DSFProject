using UnityEngine;
using static CrowdControlManager;

public interface ICrowdControl
{
    public void ApplyCrowdControl(CCContext context = null);
    public void Dispose();
}

public abstract class BasicCrowdControl : ICrowdControl
{
    public abstract void ApplyCrowdControl(CCContext context);
    public abstract void Dispose();
}

public abstract class EnhancedCrowdContrl : ICrowdControl
{
    public abstract void ApplyCrowdControl(CCContext context);
    public abstract void Dispose();
}

public class Stun : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Burn : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Contamination : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Suppress : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Strange : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Silence : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Weakness : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {
        
    }

    public void Dispose()
    {
        
    }
}

public class Overheat : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Exposure : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Bind : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Corrode : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Dominate : ICrowdControl
{
    public void ApplyCrowdControl(CCContext context)
    {

    }

    public void Dispose()
    {
        
    }
}

public class Chaos : ICrowdControl
{
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
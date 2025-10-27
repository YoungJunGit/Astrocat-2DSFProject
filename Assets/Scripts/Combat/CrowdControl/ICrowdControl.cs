using static CrowdControlManager;

public interface ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context);
}

public class StunCC : ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context)
    {

    }
}

public class BurnCC : ICrowdControl
{
    public CCContext Context { get; }
    private double baseDmg;
    public void ApplyCrowdControl(CCContext context)
    {
        
    }
}

public class SuppressionCC : ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }
}

public class ExposeCC : ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }
}

public class FloodCC : ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }
}

public class ConfusionCC : ICrowdControl
{
    public CCContext Context { get; }
    public void ApplyCrowdControl(CCContext context)
    {
        
    }
}

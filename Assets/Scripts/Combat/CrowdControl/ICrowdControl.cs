public interface ICrowdControl
{
    public int Count { get; set; }
    public void ApplyCrowdControl(BaseUnit target);
}

public class BurnCC : ICrowdControl
{
    public int Count { get; set; }

    public void ApplyCrowdControl(BaseUnit target)
    {
        
    }
}

public class OppressionCC : ICrowdControl
{
    public int Count { get; set; }
    
    public void ApplyCrowdControl(BaseUnit target)
    {
        
    }
}

public class ExposeCC : ICrowdControl
{
    public int Count { get; set; }

    public void ApplyCrowdControl(BaseUnit target)
    {
        
    }
}

public class FloodCC : ICrowdControl
{
    public int Count { get; set; }

    public void ApplyCrowdControl(BaseUnit target)
    {
        
    }
}

public class ConfusionCC : ICrowdControl
{
    public int Count { get; set; }

    public void ApplyCrowdControl(BaseUnit target)
    {
        
    }
}

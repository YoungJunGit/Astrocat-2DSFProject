using System.Collections.Generic;
using DataEnum;
using UnityEngine;
using static CrowdControlManager;

public interface ICrowdControl
{
    public void ApplyCrowdControl(CCContext context = null);
    public void Dispose();
}

public abstract class CrowdControlBase : ICrowdControl
{
    protected List<IEffectable> effectList { get; }

    public void ApplyCrowdControl(CCContext context = null)
    {
        foreach (var effect in effectList)
        {
            effect.Apply(new EffectContext(context.Target, context.Caster));
        }
    }

    public void Dispose()
    {
        
    }
}

public interface IBasicCrowdControl
{

}

public interface IEnhancedCrowdContrl
{

}

public class Stun : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Burn : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Contamination : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Suppress : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Strange : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Silence : CrowdControlBase, IBasicCrowdControl
{
    
}

public class Weakness : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Overheat : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Exposure : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Bind : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Corrode : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Dominate : CrowdControlBase, IEnhancedCrowdContrl
{
    
}

public class Chaos : CrowdControlBase, IEnhancedCrowdContrl
{
    
}
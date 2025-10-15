using System.Threading;
using Obvious.Soap;
using UnityEngine;

public class AnimationParryingHandler : MonoBehaviour
{
    private IParryingApplier _parryingApplier;
    [SerializeField]
    private BoolVariable isDebugMode;
    
    private IParryingApplier ParryingApplier
    {
        get
        {
            if (_parryingApplier == null)
                ServiceLocator.For(this).Get(out _parryingApplier);
            
            return _parryingApplier;
        }
    }

    public void SetParryOpen()
    {
        if (isDebugMode?.Value == true)
            Debug.Log($"Parry Open");
        
        ParryingApplier.SetParryOpen();
    }

    public void SetJustParryOpen()
    {
        if (isDebugMode?.Value == true)
            Debug.Log($"Just Parry Open");
        
        ParryingApplier.SetJustParryOpen();
    }

    public void SetParryClose()
    {
        if (isDebugMode?.Value == true)
            Debug.Log($"Just Parry Close");
        
        ParryingApplier.SetParryClose();
    }
}

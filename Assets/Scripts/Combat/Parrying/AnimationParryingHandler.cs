using System.Threading;
using UnityEngine;

public class AnimationParryingHandler : MonoBehaviour
{
    IParryingApplier _parryingApplier;
    
    BaseUnit _attacker;
    BaseUnit _defender;
    CancellationTokenSource _ct;

    public void Init(BaseUnit attacker)
    {
        ServiceLocator.For(this).Get(out _parryingApplier);
        
        _attacker = attacker;
    }
    
    public void SetParryInfo(BaseUnit defander, CancellationTokenSource ct)
    {
        _defender = defander;
        _ct = ct;
    }

    public void SetParryOpen(BaseUnit attacker, BaseUnit defender, CancellationTokenSource ct)
    {
        _parryingApplier.SetParryOpen(attacker, defender, ct);
    }
    
    public void SetJustParryOpen()
    {
        _parryingApplier.SetJustParryOpen();
    }

    public void SetParryingClose()
    {
        _parryingApplier.SetParryClose();
    }
}

using System;
using System.Threading;
using UnityEngine;

public interface IParryingApplier
{
    Action<ParryingApplier.ParryType> OnParry { get; }
    
    public void SetParryOpen(BaseUnit attacker, BaseUnit defender, CancellationTokenSource executedUnitAction);
    public void SetJustParryOpen();
    public void SetParryClose();
}

using System;
using System.Threading;
using UnityEngine;

public interface IParryingApplier
{
    Action<ParryingApplier.ParryType> OnParry { get; }

    void SetCurTernParryInfo(BaseUnit attacker, BaseUnit defender, CancellationTokenSource executedUnitAction);
    
    void SetParryOpen();
    void SetJustParryOpen();
    void SetParryClose();
}

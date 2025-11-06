using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Threading;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    public SIDE Target_Type { get; } 
    public virtual TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;

    public BaseBuffAction(SIDE side) { Target_Type = side; }

    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction,  CancellationTokenSource cancellationToken = default)
    {
        Debug.Log("Buff Action");
    }
}
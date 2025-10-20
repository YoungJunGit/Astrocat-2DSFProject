using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Threading;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    public virtual ACTION_TARGET_TYPE Target_Type { get; } = ACTION_TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction,  CancellationTokenSource cancellationToken = default)
    {
        Debug.Log("Buff Action");
    }
}
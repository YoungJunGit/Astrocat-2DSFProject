using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DataEnum;
using DataHashAnim;
using UnityEngine;

public interface IUnitAction
{
    public TARGET_TYPE Action_Type { get; }
    public SIDE Target_Type { get; }
    public Func<BaseUnit, bool> Target_Filter { get; }
    public UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken);
}

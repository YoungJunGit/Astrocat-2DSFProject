using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DataEnum;
using DataHashAnim;
using UnityEngine;

public interface IUnitAction
{
    public ACTION_TARGET_TYPE Target_Type { get; }
    public Func<BaseUnit, bool> Target_Filter { get; }
    public UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken);
}

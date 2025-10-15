using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DataEnum;
using DataHashAnim;
using UnityEngine;

public interface IUnitAction
{
    public UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken);
}

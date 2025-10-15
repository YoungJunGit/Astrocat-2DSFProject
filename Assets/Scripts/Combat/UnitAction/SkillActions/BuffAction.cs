using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction,  CancellationTokenSource cancellationToken = default)
    {
        Debug.Log("Buff Action");
    }
}
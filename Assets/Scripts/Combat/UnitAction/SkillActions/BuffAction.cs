using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, CancellationTokenSource cancellationToken = default)
    {
        Debug.Log("Buff Action");
    }
}
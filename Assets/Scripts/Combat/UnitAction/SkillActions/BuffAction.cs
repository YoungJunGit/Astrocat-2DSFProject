using Cysharp.Threading.Tasks;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context)
    {
        Debug.Log("Buff Action");
    }
}
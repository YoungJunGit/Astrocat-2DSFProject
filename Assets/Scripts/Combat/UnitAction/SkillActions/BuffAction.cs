using Cysharp.Threading.Tasks;
using UnityEngine;

class BaseBuffAction : IUnitAction
{
    protected BaseUnit _caster;
    protected BaseUnit _target;

    public BaseBuffAction(BaseUnit caster, BaseUnit target)
    {
        _caster = caster;
        _target = target;
    }

    public virtual async UniTask Execute(IUnitActionContext context)
    {
        Debug.Log("Buff Action");
    }
}
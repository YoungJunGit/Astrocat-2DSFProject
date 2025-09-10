using Cysharp.Threading.Tasks;
using System;
using DataEnum;
using DataHashAnim;
using UnityEngine;

interface IUnitAction
{
    public UniTask Execute();
}

#region[Buff Actions]
class BaseBuffAction : IUnitAction
{
    protected BaseUnit _caster;
    protected BaseUnit _target;

    public BaseBuffAction(BaseUnit caster, BaseUnit target)
    {
        _caster = caster;
        _target = target;
    }

    public virtual async UniTask Execute()
    {
        Debug.Log("Buff Action");
    }
}
#endregion

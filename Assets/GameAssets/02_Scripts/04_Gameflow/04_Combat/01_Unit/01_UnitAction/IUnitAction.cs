using Cysharp.Threading.Tasks;
using System;
using DataEnum;

public interface IUnitActionProperty
{
    public TARGET_TYPE Action_Type { get; }
    public SIDE Target_Type { get; }
    public Func<BaseUnit, bool> Target_Filter { get; }
}

public interface IUnitActionInvoker
{
    public UniTask Execute(IUnitActionContext context);
}

public interface IUnitAction : IUnitActionProperty, IUnitActionInvoker { }

public interface IParryAction : IUnitActionInvoker { }
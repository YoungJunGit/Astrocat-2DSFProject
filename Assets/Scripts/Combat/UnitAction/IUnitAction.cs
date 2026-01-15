using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DataEnum;
using UnityEngine;

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

public abstract class BaseUnitAction<TContext> : IUnitAction where TContext : IUnitActionContext
{
    public abstract TARGET_TYPE Action_Type { get; }
    public abstract SIDE Target_Type { get; }
    public abstract Func<BaseUnit, bool> Target_Filter { get; }
    public abstract int SPCost { get; }

    public async UniTask Execute(IUnitActionContext context) => await ExecuteWith((TContext)context);

    public async UniTask ExecuteWith(TContext context)
    {
        context.Caster.AddSP(SPCost);
        context.Caster.Attachments.GetSpriteRenderer().sortingLayerName = "Actor";

        await AsyncOperateAction(context);

        context.Caster.Attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }

    public abstract UniTask AsyncOperateAction(TContext context);
}

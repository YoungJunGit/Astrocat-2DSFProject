using Cysharp.Threading.Tasks;
using System;
using DataEnum;

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
        context.Caster.ChangeSortingLayer("Actor");

        await AsyncOperateAction(context);

        context.Caster.ChangeSortingLayer("Character");
    }

    public abstract UniTask AsyncOperateAction(TContext context);
}
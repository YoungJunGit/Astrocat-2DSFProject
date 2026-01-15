using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Threading;
using UnityEngine;

public class SkillAction : IUnitAction
{
    public virtual SIDE Target_Type { get; }
    public virtual TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;

    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        unitAction.OnStartAction(context);

        //await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}

public class Skill_Taunt : SkillAction
{
    public override SIDE Target_Type { get; } = SIDE.ENEMY;
    public override TARGET_TYPE Action_Type { get; } = TARGET_TYPE.SINGLE;
    public override Func<BaseUnit, bool> Target_Filter { get; } = null;

    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = null)
    {
        await base.Execute(context, unitAction, cancellationToken);

        Debug.Log("Taunt Enemy!");

        unitAction.OnFinishedAction(context);
    }
}
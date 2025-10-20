using Cysharp.Threading.Tasks;
using DataEnum;
using System;
using System.Threading;
using UnityEngine;

public class SkillAttackAction : IUnitAction
{
    public virtual ACTION_TARGET_TYPE Target_Type { get; } = ACTION_TARGET_TYPE.SINGLE;
    public virtual Func<BaseUnit, bool> Target_Filter { get; } = null;
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}

public class Skill_Taunt : SkillAttackAction
{
    public override ACTION_TARGET_TYPE Target_Type { get; } = ACTION_TARGET_TYPE.ALL;
    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = null)
    {
        await base.Execute(context, unitAction, cancellationToken);

        Debug.Log("Taunt Enemy!");

        unitAction.OnFinishedAction(context);
    }
}
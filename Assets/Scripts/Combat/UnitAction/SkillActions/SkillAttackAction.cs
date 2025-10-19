using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}

public class Skill_Taunt : SkillAttackAction
{
    public override async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = null)
    {
        await base.Execute(context, unitAction, cancellationToken);

        Debug.Log("Taunt Enemy!");
    }
}
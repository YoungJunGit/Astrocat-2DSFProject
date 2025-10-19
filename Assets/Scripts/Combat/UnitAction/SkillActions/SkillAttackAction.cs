using System.Threading;
using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction, CancellationTokenSource cancellationToken = default)
    {


        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}
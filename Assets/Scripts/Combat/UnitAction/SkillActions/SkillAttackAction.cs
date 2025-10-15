using System.Threading;
using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, CancellationTokenSource cancellationToken = default)
    {


        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}
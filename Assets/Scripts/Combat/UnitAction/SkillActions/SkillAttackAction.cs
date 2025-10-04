using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context, IUnitActionEvent unitAction)
    {
        unitAction.OnStartAction(context);

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}
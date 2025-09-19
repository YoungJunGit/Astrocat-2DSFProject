using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    public virtual async UniTask Execute(IUnitActionContext context)
    {
        context.Caster.combatInfo.isFinishedAction = false;
        context.Caster.attachments.GetSpriteRenderer().sortingLayerName = "Actor";

        await UniTask.WaitUntil(() => context.Caster.combatInfo.isFinishedAction);
    }
}
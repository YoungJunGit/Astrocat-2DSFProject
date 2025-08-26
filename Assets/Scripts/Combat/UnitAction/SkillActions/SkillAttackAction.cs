using Cysharp.Threading.Tasks;

public class SkillAttackAction : IUnitAction
{
    protected BaseUnit _caster;
    protected BaseUnit _target;

    public SkillAttackAction(BaseUnit caster, BaseUnit target)
    {
        _caster = caster;
        _target = target;
    }

    public virtual async UniTask Execute()
    {
        _caster.combatInfo.isFinishedAction = false;
        _caster.attachments.GetSpriteRenderer().sortingLayerName = "Actor";

        await UniTask.WaitUntil(() => _caster.combatInfo.isFinishedAction);
    }
}
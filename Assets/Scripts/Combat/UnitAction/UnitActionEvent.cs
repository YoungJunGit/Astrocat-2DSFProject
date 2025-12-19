using Cysharp.Threading.Tasks;
using Michsky.UI.Shift;
using System;
using UnityEngine;
using Utils;

public interface IUnitActionEvent
{
    UniTask ShowAttackWarningMessage(IUnitActionContext context);
    UniTask ShowSelfAttackMessage(IUnitActionContext context);
    bool TryGetSingle(IUnitActionContext context, out BaseUnit value);
    void OnStartAction(IUnitActionContext context);
    void OnFinishedAction(IUnitActionContext context);
    void DamageEvent(BaseUnit caster, BaseUnit target);
}

public class UnitActionEvent : IUnitActionEvent
{
    public async UniTask ShowAttackWarningMessage(IUnitActionContext context)
    {
        await context.TextManager.ShowAttackWarningText(context.Caster);
    }

    public async UniTask ShowSelfAttackMessage(IUnitActionContext context)
    {
        await context.TextManager.ShowSelfAttackText(context.Caster);
    }

    public bool TryGetSingle(IUnitActionContext context, out BaseUnit value)
    {
        value = TargetExtensions.SingleOrDefaultFast(context.TargetBag);

        if (value == null)
            return false;
        else
            return true;
    }

    public void OnStartAction(IUnitActionContext context)
    {
        context.Caster.CombatInfo.isFinishedAction = false;
        context.Caster.Attachments.GetSpriteRenderer().sortingLayerName = "Actor";
    }

    public void OnFinishedAction(IUnitActionContext context)
    {
        context.Caster.CombatInfo.isFinishedAction = true;
        context.Caster.Attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }

    public void DamageEvent(BaseUnit caster, BaseUnit target)
    {
        IDamage damage = DamageFactory.CreateDamage<NormalDamageCalculator>(caster, target);
        target.GetStat().GetDamaged(damage);
    }
}

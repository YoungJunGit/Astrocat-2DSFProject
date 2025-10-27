using Cysharp.Threading.Tasks;
using Michsky.UI.Shift;
using System;
using UnityEngine;
using Utils;

public interface IUnitActionEvent
{
    UniTask ShowAttackMessage(IUnitActionContext context);
    public bool TryGetSingle(IUnitActionContext context, out BaseUnit value);
    void OnStartAction(IUnitActionContext context);
    void OnFinishedAction(IUnitActionContext context);
    void DamageEvent(IUnitActionContext context, BaseUnit target);
}

public class UnitActionEvent : IUnitActionEvent
{
    public async UniTask ShowAttackMessage(IUnitActionContext context)
    {
        await context.DialogueManager.ShowAttackWarningDialogue(context.Caster);
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
        context.Caster.combatInfo.isFinishedAction = false;
        context.Caster.attachments.GetSpriteRenderer().sortingLayerName = "Actor";
    }

    public void OnFinishedAction(IUnitActionContext context)
    {
        context.Caster.combatInfo.isFinishedAction = true;
        context.Caster.attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }

    public void DamageEvent(IUnitActionContext context, BaseUnit target)
    {
        float damage = context.DamageFactory.CreateNormalDamage((float)context.Caster.GetStat().GetData().Default_Attack, target.attachments.GetHitBox().bounds);
        target.GetStat().GetDamaged(damage);
    }
}

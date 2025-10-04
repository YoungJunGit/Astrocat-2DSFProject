using Michsky.UI.Shift;
using System;
using UnityEngine;

public interface IUnitActionEvent
{
    void OnStartAction(IUnitActionContext context);
    void OnFinishedAction(IUnitActionContext context);
    void DamageEvent(IUnitActionContext context);
}

public class UnitActionEvent : IUnitActionEvent
{
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

    public void DamageEvent(IUnitActionContext context)
    {
        float damage = context.damageFactory.CreateNormalDamage((float)context.Caster.GetStat().GetData().Default_Attack, context.unitManager.SelectedUnit.attachments.GetHitBox().bounds);
        context.unitManager.SelectedUnit.GetStat().GetDamaged(damage);
    }
}

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    UnitManager unitManager { get; }
    DamageFactory damageFactory { get; }

    void OnStartAction();
    void OnFinishedAction();
    void DamageEvent();
}

public record UnitActionContext(BaseUnit Caster, UnitManager unitManager, DamageFactory damageFactory) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public UnitManager unitManager { get; } = unitManager;
    public DamageFactory damageFactory { get; } = damageFactory;

    public void OnStartAction()
    {
        Caster.combatInfo.isFinishedAction = false;
        Caster.attachments.GetSpriteRenderer().sortingLayerName = "Actor";
    }

    public void OnFinishedAction()
    {
        Caster.combatInfo.isFinishedAction = true;
        Caster.attachments.GetSpriteRenderer().sortingLayerName = "Character";
    }

    public void DamageEvent()
    {
        float damage = damageFactory.CreateNormalDamage((float)Caster.GetStat().GetData().Default_Attack, unitManager.SelectedUnit.attachments.GetHitBox().bounds);

        unitManager.SelectedUnit.GetStat().GetDamaged(damage);
    }
}
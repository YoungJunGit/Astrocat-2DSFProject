using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public interface IUnitActionContext
{
    BaseUnit Caster { get; }
    UnitManager unitManager { get; }

    void OnStartAction();
    void OnFinishedAction();
    void DamageEvent();
}

public record UnitActionContext(BaseUnit Caster, UnitManager unitManager) : IUnitActionContext
{
    public BaseUnit Caster { get; } = Caster;
    public UnitManager unitManager { get; } = unitManager;

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
        DamageContainer damage = AssetLoader.GetDamageFactory().CreateNormalDamage((float)Caster.GetStat().GetData().Default_Attack, unitManager.SelectedUnit.attachments.GetHitBox().bounds);
        unitManager.SelectedUnit.GetStat().GetDamaged(damage.Value, damage.Critical);
    }
}
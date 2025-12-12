using DataEnum;
using static CombatEffectManager;
using static TimelinePublisher;

public abstract class DamageEffect : BaseEffect<float>, IStartTurnEffectProvider
{
    public abstract string StartTurnKey { get; }

    protected override BasicStatModifier<float> CreateModifier()
    {
        float value = Info.Value;

        var modifer = new BasicStatModifier<float>(
            buffType,
            updateType,
            (v) => v + value,
            CreateTimer()
        );

        Context.Target.CEUnit.AddOnStartTurnEffect(StartTurnKey, OnEachTurn);
        return modifer;
    }

    public void OnEachTurn()
    {
        IDamage damage = DamageFactory.CreateNormalDamage<BurnDamageCalculator>(Context.Caster, Context.Target);
        Context.Target.GetStat().GetDamaged(damage);
    }
}

public class BurnEffect : DamageEffect, IStackableEffect
{
    public override string StartTurnKey => "FIRE_DOT";
    public override BUFF_TYPE buffType => BUFF_TYPE.DAMAGE_EACH_TURN;
}
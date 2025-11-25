using DataEnum;
using static CombatEffectManager;
using static TimelinePublisher;

public abstract class DamageEffect : BaseEffect<float>, IStartTurnEffectProvider
{
    public abstract string StartTurnKey { get; }
    public abstract BUFF_TYPE damageType { get; }
    public abstract UPDATE_TYPE updateType { get; }

    protected override BasicStatModifier<float> CreateModifier()
    {
        float value = Info.Value;

        var modifer = new BasicStatModifier<float>(
            damageType,
            updateType,
            (v) => v + value
        );

        Context.Target.combatEffectUnit.AddOnStartTurnEffect(StartTurnKey, OnEachTurn);
        return modifer;
    }

    public void OnEachTurn()
    {
        IDamage damage = DamageFactory.CreateNormalDamage<BurnDamageCalculator>(Context.Caster, Context.Target);
        Context.Target.GetStat().GetDamaged(damage);
    }
}

public class BurnEffect : DamageEffect
{
    public override string StartTurnKey => "FIRE_DOT";

    public override BUFF_TYPE damageType => BUFF_TYPE.DAMAGE_EACH_TURN;

    public override UPDATE_TYPE updateType => UPDATE_TYPE.ROUND;
}
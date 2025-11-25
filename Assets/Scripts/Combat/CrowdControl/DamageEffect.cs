using DataEnum;
using static CombatEffectManager;
using static TimelinePublisher;

public class BurnEffect : BaseEffect<float>, IStartTurnEffectProvider
{
    public string StartTurnKey => "FIRE_DOT";

    protected override BasicStatModifier<float> CreateModifier()
    {
        float value = Info.Value;

        var modifer = new BasicStatModifier<float>(
            BUFF_TYPE.DAMAGE_EACH_TURN, 
            UPDATE_TYPE.ROUND, 
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
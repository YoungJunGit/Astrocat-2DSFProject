using DataEnum;
using static CombatEffectManager;
using static TimelinePublisher;

public class BurnEffect : BaseEffect<float>
{
    protected override BasicStatModifier<float> CreateModifier()
    {
        var modifer = new BasicStatModifier<float>(BUFF_TYPE.DAMAGE_EACH_TURN, UPDATE_TYPE.ROUND, (v) => v + Info.Value);
        modifer.OnTimelineUpdate += () => { OnEachTurn(); };
        return modifer;
    }

    public void OnEachTurn()
    {
        IDamage damage = DamageFactory.CreateNormalDamage<BurnDamageCalculator>(Context.Caster, Context.Target);
        Context.Target.GetStat().GetDamaged(damage);
    }
}
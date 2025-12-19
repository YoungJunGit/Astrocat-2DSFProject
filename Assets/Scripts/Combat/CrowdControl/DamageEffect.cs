using DataEnum;
using static CombatEffectManager;

public abstract class DamageEffect : BaseEffect<float>, IStartTurnEffectProvider
{
    protected DamageEffect(EffectTrait trait) : base(trait) { }

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
        IDamage damage = DamageFactory.CreateDamage<BurnDamageCalculator>(Context.Caster, Context.Target);
        Context.Target.GetStat().GetDamaged(damage);
    }
}

public class BurnEffect : DamageEffect
{
    public BurnEffect(EffectTrait trait) : base(trait) { }
    public override string StartTurnKey => "FIRE_DOT";
    public override BUFF_TYPE buffType => BUFF_TYPE.DAMAGE_EACH_TURN;
}
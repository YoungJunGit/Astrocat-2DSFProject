using UnityEngine;
using Utils;

public interface ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target);
}

public class NormalDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = caster.GetStat().modifierStat.Attack;
        bool isCritical = FunctionUtils.MakeChance(caster.GetStat().modifierStat.CriticalChance);

        // TODO : calculate value using specific formula

        return new Damage(damage, isCritical);
    }
}

public class BurnDamageCalculator : ICalculator
{
    public IDamage Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return null;
    }
}
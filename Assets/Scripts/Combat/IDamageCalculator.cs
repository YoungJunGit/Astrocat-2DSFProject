using UnityEngine;

public interface IDamageCalculator
{
    public float Calculate(BaseUnit caster, BaseUnit target);
}

public class NormalDamageCalculator : IDamageCalculator
{
    public float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}

public class CrowdControlCalculator : IDamageCalculator
{
    protected int stack;
    public CrowdControlCalculator(int stack) { this.stack = stack; }

    public virtual float Calculate(BaseUnit caster, BaseUnit target) => 0f;
}

public class BurnCCDamageCalculator : CrowdControlCalculator
{
    public BurnCCDamageCalculator(int stack) : base(stack) { }

    public override float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}

public class OppressionCCDamageCalculator : CrowdControlCalculator
{
    public OppressionCCDamageCalculator(int stack) : base(stack) { }

    public override float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}

public class ExposeCCDamageCalculator : CrowdControlCalculator
{
    public ExposeCCDamageCalculator(int stack) : base(stack) { }

    public override float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}

public class FloodCCDamageCalculator : CrowdControlCalculator
{
    public FloodCCDamageCalculator(int stack) : base(stack) { }

    public override float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}

public class ConfusionCCDamageCalculator : CrowdControlCalculator
{
    public ConfusionCCDamageCalculator(int stack) : base(stack) { }

    public override float Calculate(BaseUnit caster, BaseUnit target)
    {
        float damage = 0f;

        // TODO : calculate value using specific formula

        return damage;
    }
}
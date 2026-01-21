using DataEnum;

public abstract record CalculationContext(BaseUnit Caster, BaseUnit Target);
public sealed record DamageContext(BaseUnit Caster, BaseUnit Target, float SkillRate = 1.0f) : CalculationContext(Caster, Target);
public sealed record ElementGaugeContext(BaseUnit Caster, BaseUnit Target, ELEMENT_TYPE ElementType, SKILL_ELEMENT_RATE RateType) : CalculationContext(Caster, Target);
public sealed record HealContext(BaseUnit Caster, BaseUnit Target, float SkillRate = 1.0f, float subValue = 0.0f) : CalculationContext(Caster, Target);
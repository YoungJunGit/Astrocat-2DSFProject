using NaughtyAttributes;
using UnityEngine;
using Utils;

[CreateAssetMenu(fileName = "DamageFactory", menuName = "GameScene/DamageFactory")]
public class DamageFactory : ScriptableObject
{
    private int criticalChance = 50;     // tmp

    [SerializeField] private DamageContainer normalDamageContainer;
    [SerializeField] private DamageContainer criticalDamageContainer;

    public float CreateNormalDamage(float damageValue, Bounds bounds, int chance = 0)
    {
        // TEMP
        float resultValue = damageValue;
        chance = criticalChance;

        /// Create Damage Information ///
        IDamageCalculator calculator = new NormalDamageCalculator();

        // TODO : Calculate Method Invoke
        // float resultValue = calculator.Calculate(caster, target);

        bool IsCritical = FunctionUtils.MakeChance(new Vector2(0, 100), chance);

        /// Create Damage Displayer ///
        IDamageValueDisplayer displayer = new DamageValueDisplayer_Bounce();


        /// Invoke Damage Display ///
        IDamageValueDisplayInvoker displayInvoker = new DamageValueDisplayInvoker();
        if (!IsCritical)
            displayInvoker.Invoke(displayer, resultValue, bounds, normalDamageContainer);
        else
            displayInvoker.Invoke(displayer, resultValue, bounds, criticalDamageContainer);

        return resultValue;
    }

    //public DamageContainertmp CreateCrowdControlDamage(ICrowdControl crowdControl, Bounds bounds)
    //{
    //    float damageValue = 0f;
    //    CrowdControlCalculator calculator;
    //    switch(crowdControl)
    //    {
    //        case BurnCC:
    //            calculator = new BurnCCDamageCalculator(crowdControl.Count);
    //            break;
    //        case OppressionCC:
    //            calculator = new OppressionCCDamageCalculator(crowdControl.Count);
    //            break;
    //        case ExposeCC:
    //            calculator = new ExposeCCDamageCalculator(crowdControl.Count);
    //            break;
    //        case FloodCC:
    //            calculator = new FloodCCDamageCalculator(crowdControl.Count);
    //            break;
    //        case ConfusionCC:
    //            calculator = new ConfusionCCDamageCalculator(crowdControl.Count);
    //            break;
    //    }

    //    DamageContainertmp damage = new DamageContainertmp(DecideCritical(criticalChance), damageValue);

    //    return damage;
    //}
}

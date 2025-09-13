using NaughtyAttributes;
using UnityEngine;

public class DamageContainer
{
    public DamageContainer(bool critical, float value)
    {
        this.critical = critical;
        this.value = value;
    }

    private bool critical;
    private float value;

    public bool Critical => critical;
    public float Value => value;
}

[CreateAssetMenu(fileName = "DamageFactory", menuName = "GameScene/DamageFactory")]
public class DamageFactory : ScriptableObject
{
    [SerializeField, MinValue(0), MaxValue(100)]
    private int criticalChance;     // tmp

    [SerializeField] private DamageValue damageValuePrefab;

    public DamageContainer CreateNormalDamage(float damageValue, Bounds bounds)
    {
        DamageContainer damage = new DamageContainer(DecideCritical(criticalChance), damageValue);

        CreateDamageUI(damage, bounds);

        return damage;
    }

    public DamageContainer CreateCrowdControlDamage(ICrowdControl crowdControl, Bounds bounds)
    {
        float damageValue = 0f;
        CrowdControlCalculator calculator;
        switch(crowdControl)
        {
            case BurnCC:
                calculator = new BurnCCDamageCalculator(crowdControl.Count);
                break;
            case OppressionCC:
                calculator = new OppressionCCDamageCalculator(crowdControl.Count);
                break;
            case ExposeCC:
                calculator = new ExposeCCDamageCalculator(crowdControl.Count);
                break;
            case FloodCC:
                calculator = new FloodCCDamageCalculator(crowdControl.Count);
                break;
            case ConfusionCC:
                calculator = new ConfusionCCDamageCalculator(crowdControl.Count);
                break;
        }

        DamageContainer damage = new DamageContainer(DecideCritical(criticalChance), damageValue);

        return damage;
    }

    private void CreateDamageUI(DamageContainer damage, Bounds bounds)
    {
        Vector2 spawnBounds = new Vector2(
            Random.Range(bounds.min.x, bounds.max.x), 
            Random.Range((bounds.center.y + bounds.max.y) / 2, bounds.max.y)
            );
        DamageValue damageValue = Instantiate(damageValuePrefab, spawnBounds, Quaternion.identity);
        damageValue.SetValue(damage.Value, damage.Critical);
    }

    private bool DecideCritical(float chance)
    {
        int rand = Random.Range(0, 100);
        if (rand < chance)
            return true;
        else
            return false;
    }
}

using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageHealValueDisplayInvoker
{
    void Invoke(IDamageValueDisplayer displayer, float value, Bounds bounds, DamageContainer container);
}

public class DamageHealValueDisplayInvoker : IDamageHealValueDisplayInvoker
{
    public void Invoke(IDamageValueDisplayer displayer, float value, Bounds bounds, DamageContainer container)
    {
        displayer.Display(value, bounds, container);
    }
}

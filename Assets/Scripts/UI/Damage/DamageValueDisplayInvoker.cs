using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public interface IDamageValueDisplayInvoker
{
    void Invoke(IDamageValueDisplayer displayer, float value, Bounds bounds, DamageContainer container);
}

public class DamageValueDisplayInvoker : IDamageValueDisplayInvoker
{
    public void Invoke(IDamageValueDisplayer displayer, float value, Bounds bounds, DamageContainer container)
    {
        displayer.Display(value, bounds, container);
    }
}

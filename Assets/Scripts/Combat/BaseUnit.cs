using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using DataEntity;
using DataEnum;

public class BaseUnit : MonoBehaviour
{
    private UnitStat _stat;

    public virtual void Initialize(EntityData data)
    {
        _stat = new UnitStat(data);
    }

    public UnitStat GetStat() { return _stat; }
}
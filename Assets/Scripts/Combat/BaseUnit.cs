using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using DataEntity;
using DataEnum;

public class BaseUnit : MonoBehaviour
{
    private UnitStat stat = new UnitStat();
    protected HUDManager hudManager;

    public virtual void Initialize(EntityData data, HUDManager hudManager)
    {
        stat.IntializeStat(data);
        this.hudManager = hudManager;
        CreateHUD();
        stat.OnIntializeHUD();
    }

    protected virtual void CreateHUD() {}

    protected virtual void Attack()
    {
        stat.OnNormalAttack();

        /* 공격 메서드 작성*/
    }

    protected virtual void Move()
    {
        /* 이동 메서드 작성 */

        Attack();
    }

    public UnitStat GetStat() { return stat; }
}

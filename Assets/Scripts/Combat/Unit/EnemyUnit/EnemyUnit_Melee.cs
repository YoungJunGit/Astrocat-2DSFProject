using UnityEngine;

public class EnemyUnit_Melee : EnemyUnit, IMelee
{
    public void Move()
    {
        Debug.Log("Move");
    }
}

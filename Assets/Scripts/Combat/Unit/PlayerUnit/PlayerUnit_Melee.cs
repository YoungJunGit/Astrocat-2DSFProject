using UnityEngine;

public class PlayerUnit_Melee : PlayerUnit, IMelee
{
    public void Move()
    {
        Debug.Log("Move");
    }
}

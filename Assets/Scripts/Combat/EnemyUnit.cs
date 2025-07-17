using UnityEngine;
using DataEntity;
using DataEnum;

public class EnemyUnit : BaseUnit
{
    protected override void CreateHUD()
    {
        hudManager.CreateEnemyHUD(this);
    }
}

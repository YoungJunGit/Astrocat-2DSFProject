using UnityEngine;
using DataEntity;
using DataEnum;

public class PlayerUnit : BaseUnit
{
    protected override void CreateHUD()
    {
        hudManager.CreatePlayerHUD(this);
    }
}

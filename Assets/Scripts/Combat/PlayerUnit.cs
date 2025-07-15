using UnityEngine;

public class PlayerUnit : BaseUnit
{
    public override void Init(EntityData data)
    {
        spawnTransform = GameObject.Find("PlayerStatus/StatusPanel").transform;
        base.Init(data);
    }
}

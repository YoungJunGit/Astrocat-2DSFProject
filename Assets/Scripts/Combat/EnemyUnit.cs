using UnityEngine;

public class EnemyUnit : BaseUnit
{
    [Space(10f)]
    [SerializeField]
    private Vector3 spawnOffset;

    public override void Init(EntityData data)
    {
        spawnTransform = GameObject.Find("EnemyStatus").transform;
        base.Init(data);
    }
}

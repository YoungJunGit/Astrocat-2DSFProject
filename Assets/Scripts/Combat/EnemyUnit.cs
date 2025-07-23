using UnityEngine;
using DataEntity;
using DataEnum;

public class EnemyUnit : BaseUnit
{
    [SerializeField] private Transform StatusPos;

    public Transform GetStatusPosition() { return StatusPos; }
}

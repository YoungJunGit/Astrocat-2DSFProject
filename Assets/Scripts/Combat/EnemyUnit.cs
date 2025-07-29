using UnityEngine;
using DataEntity;
using DataEnum;

public class EnemyUnit : BaseUnit
{
    [SerializeField] private Transform StatusPos;
    [SerializeField] private Transform UnitSelectArrowPos;

    public Transform GetStatusPosition() { return StatusPos; }
    public Transform GetUnitSelectArrowPos() { return UnitSelectArrowPos; }
}

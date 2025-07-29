using UnityEngine;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField] private Transform StatusPos;
    [SerializeField] private Transform UnitSelectArrowPos;

    public Transform GetStatusPosition() { return StatusPos; }
    public Transform GetUnitSelectArrowPos() { return UnitSelectArrowPos; }
}

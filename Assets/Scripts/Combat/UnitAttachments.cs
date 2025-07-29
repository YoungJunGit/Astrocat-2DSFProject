using UnityEngine;

public class UnitAttachments : MonoBehaviour
{
    [SerializeField] private Transform StatusPos;
    [SerializeField] private Transform UnitSelectArrowPos;
    [SerializeField] private Transform ActionSelectorPos;

    public Transform GetStatusPosition() => StatusPos;
    public Transform GetUnitSelectArrowPos() => UnitSelectArrowPos;
    public Transform GetActionSelectorPos() => ActionSelectorPos;
}

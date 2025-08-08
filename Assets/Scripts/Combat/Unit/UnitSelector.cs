
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DataEnum;

[CreateAssetMenu(fileName = "UnitSelector", menuName = "GameScene/UnitSelector", order = 1)]
public class UnitSelector : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList;
    [SerializeField] private UnitSelectorController controller;
    [SerializeField] public GameObject unitSelectArrowPrefab;
    private GameObject unitSelectArrow;
    private bool isConfirmed;

    public void Init()
    {
        isConfirmed = false;
        controller.Initialize(() => isConfirmed = true);
    }

    public async UniTask<BaseUnit> SelectUnit(SIDE side)
    {
        isConfirmed = false;
        unitSelectArrow = Instantiate(unitSelectArrowPrefab, unitList.GetUnits(side)[controller.GetSelectionIndex(side)].attachments.GetUnitSelectArrowPos(), false);

        controller.OnStartSelect((index) => MoveArrow(index, side), side, unitList.GetUnits(side).Count);
        await UniTask.WaitUntil(() => isConfirmed == true);
        controller.OnEndSelect();

        Destroy(unitSelectArrow);

        return unitList.GetUnits(side)[controller.GetSelectionIndex(side)];
    }

    public void MoveArrow(int index, SIDE side)
    {
        unitSelectArrow.transform.SetParent(unitList.GetUnits(side)[index].attachments.GetUnitSelectArrowPos(), false);
    }
}
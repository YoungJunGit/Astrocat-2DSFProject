
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DataEnum;

[CreateAssetMenu(fileName = "UnitSelector", menuName = "GameScene/UnitSelector", order = 1)]
class UnitSelector : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList;
    [SerializeField] private UnitSelectorController controller;
    [SerializeField] public GameObject unitSelectArrowPrefab;
    private GameObject unitSelectArrow;
    private SIDE side;
    private bool isConfirmed;

    public void Init()
    {
        side = SIDE.NONE;
        isConfirmed = false;
        controller.Initialize(() => isConfirmed = true, (index) => MoveArrow(index));
    }

    public async UniTask<BaseUnit> SelectUnit(SIDE side)
    {
        this.side = side;
        isConfirmed = false;
        unitSelectArrow = Instantiate(unitSelectArrowPrefab, unitList.GetUnits(side)[controller.GetSelectionIndex(side)].attachments.GetUnitSelectArrowPos(), false);

        using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
        {
            controller.OnStartSelect(side, unitList.GetUnits(side).Count);
            await UniTask.WaitUntil(() => isConfirmed == true);
            controller.OnEndSelect();
        }

        Destroy(unitSelectArrow);

        return unitList.GetUnits(side)[controller.GetSelectionIndex(side)];
    }

    public BaseUnit SelectRandomUnit(SIDE side)
    {
        int randomIndex = Random.Range(0, unitList.GetUnits(side).Count);
        return unitList.GetUnits(side)[randomIndex];
    }

    public void MoveArrow(int index)
    {
        unitSelectArrow.transform.SetParent(unitList.GetUnits(side)[index].attachments.GetUnitSelectArrowPos(), false);
    }
}
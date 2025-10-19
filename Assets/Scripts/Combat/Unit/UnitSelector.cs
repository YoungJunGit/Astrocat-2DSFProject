using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using DataEnum;

[CreateAssetMenu(fileName = "UnitSelector", menuName = "GameScene/UnitSelector", order = 1)]
class UnitSelector : ScriptableObject
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private UnitSelectorController controller;
    [SerializeField] private UnitSelectorObject unitSelectArrowPrefab;
    private UnitSelectorObject unitSelectArrow;
    private ScriptableListBaseUnit _units;
    private SIDE side;
    private bool isConfirmed;

    public void Init(ScriptableListBaseUnit units)
    {
        _units = units;
        side = SIDE.NONE;
        isConfirmed = false;
        controller.Initialize(() => isConfirmed = true, 
            (index) => unitSelectArrow.transform.SetParent(_units.GetUnits(side)[index].attachments.GetUnitSelectArrowPos(), false));
    }

    public async UniTask<BaseUnit> SelectUnit(SIDE side)
    {
        controller.Prepare(side, _units.GetUnits(side).Count);
        this.side = side;
        isConfirmed = false;

        int selectIndex = controller.GetSelectionIndex(side);
        unitSelectArrow = Instantiate(unitSelectArrowPrefab, _units.GetUnits(side)[selectIndex].attachments.GetUnitSelectArrowPos(), false);
        unitSelectArrow.Set(side);

        using (var inputDisposer = new InputDisposer(controller.InputHandler, InputHandler.InputState.SelectUnit))
        {
            controller.OnStartSelect(side);
            await UniTask.WaitUntil(() => isConfirmed == true);
            controller.OnEndSelect(side);
        }

        Destroy(unitSelectArrow.gameObject);

        return _units.GetUnits(side)[controller.GetSelectionIndex(side)];
    }

    public BaseUnit SelectRandomUnit(SIDE side)
    {
        int randomIndex = Random.Range(0, _units.GetUnits(side).Count);
        return _units.GetUnits(side)[randomIndex];
    }
}
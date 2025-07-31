using DataEnum;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UnitSelectorController", menuName = "GameScene/UnitSelectorController", order = 1)]
public class UnitSelectorController : ScriptableObject
{
    [SerializeField] private InputActionAsset asset;
    [SerializeField] private InputActionReference selectionConfirmAction;
    [SerializeField] private InputActionReference selectEnemyUnitAction;
    [SerializeField] private InputActionReference selectPlayerUnitAction;

    private UnityAction confirm;
    private UnityAction<int> select;

    private SIDE side;

    private int _selectedUnitIndex;
    private int _previousEnemySelectionIndex;
    private int _previousPlayerSelectionIndex;

    private int _maxUnitCount;

    public void Initialize(UnityAction confirm)
    {
        side                            = SIDE.NONE;
        _selectedUnitIndex              = 0;
        _previousEnemySelectionIndex    = 0;
        _previousPlayerSelectionIndex   = 0;
        this.confirm = confirm;
    }

    public void OnStartSelect(UnityAction<int> select, DataEnum.SIDE side, int maxUnitcount)
    {
        this.select = select;
        this.side = side;
        _maxUnitCount = maxUnitcount;

        asset.Enable();
        selectionConfirmAction.action.performed += OnSelectionConfirmPerformed;

        if (side == DataEnum.SIDE.ENEMY)
        {
            _selectedUnitIndex = _previousEnemySelectionIndex;
            selectEnemyUnitAction.action.performed += OnSelectUnitPerformed;
        }
        else
        {
            _selectedUnitIndex = _previousPlayerSelectionIndex;
            selectPlayerUnitAction.action.performed += OnSelectUnitPerformed;
        }
    }

    public void OnEndSelect()
    {
        asset.Disable();
        selectionConfirmAction.action.performed -= OnSelectionConfirmPerformed;

        if (side == DataEnum.SIDE.ENEMY)
        {
            _previousEnemySelectionIndex = _selectedUnitIndex;
            selectEnemyUnitAction.action.performed -= OnSelectUnitPerformed;
        }
        else
        {
            _previousPlayerSelectionIndex = _selectedUnitIndex;
            selectPlayerUnitAction.action.performed -= OnSelectUnitPerformed;
        }
    }

    public int GetSelectionIndex(SIDE side)
    {
        if (side == DataEnum.SIDE.PLAYER)
            return _previousPlayerSelectionIndex;
        else
            return _previousEnemySelectionIndex;
    }

    private void OnSelectUnitPerformed(InputAction.CallbackContext context)
    {
        _selectedUnitIndex = Mathf.Clamp(_selectedUnitIndex + (int)context.ReadValue<float>(), 0, _maxUnitCount - 1);
        select(_selectedUnitIndex);
    }

    private void OnSelectionConfirmPerformed(InputAction.CallbackContext context)
    {
        confirm();
    }
}

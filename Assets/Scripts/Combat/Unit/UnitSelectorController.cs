using DataEnum;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "UnitSelectorController", menuName = "GameScene/UnitSelectorController", order = 1)]
public class UnitSelectorController : ScriptableObject
{
    [SerializeField] private InputHandler inputHandler;
    public InputHandler InputHandler => inputHandler;

    private UnityAction confirm;
    private UnityAction<int> select;

    private SIDE side;

    private int _selectedUnitIndex;
    private int _previousEnemySelectionIndex;
    private int _previousPlayerSelectionIndex;

    private int _maxUnitCount;

    public void Initialize(UnityAction confirm, UnityAction<int> select)
    {
        side                            = SIDE.NONE;
        _selectedUnitIndex              = 0;
        _previousEnemySelectionIndex    = 0;
        _previousPlayerSelectionIndex   = 0;
        this.confirm    = confirm;
        this.select     = select;
    }

    public void Prepare(SIDE side, int maxUnitCount)
    {
        _maxUnitCount = maxUnitCount;

        if (side == SIDE.ENEMY)
        {
            _previousEnemySelectionIndex = _previousEnemySelectionIndex > _maxUnitCount - 1 ? _maxUnitCount - 1 : _previousEnemySelectionIndex;
        }
        else if(side == SIDE.PLAYER)
        {
            _previousPlayerSelectionIndex = _previousPlayerSelectionIndex > _maxUnitCount - 1 ? _maxUnitCount - 1 : _previousPlayerSelectionIndex;
        }
    }

    public void OnStartSelect(SIDE side)
    {
        this.side = side;

        inputHandler.OnSelectUnitSelectionConfirm += () => confirm();

        if (side == SIDE.ENEMY)
        {
            _selectedUnitIndex = _previousEnemySelectionIndex;
            inputHandler.OnSelectUnitEnemySelectionMove += OnUnitSelect;
        }
        else if (side == SIDE.PLAYER)
        {
            _selectedUnitIndex = _previousPlayerSelectionIndex;
            inputHandler.OnSelectUnitPlayerSelectionMove += OnUnitSelect;
        }
    }

    public void OnEndSelect()
    {
        inputHandler.OnSelectUnitSelectionConfirm = null;

        if (side == SIDE.ENEMY)
        {
            _previousEnemySelectionIndex = _selectedUnitIndex;
            inputHandler.OnSelectUnitEnemySelectionMove = null;
        }
        else if (side == SIDE.PLAYER)
        {
            _previousPlayerSelectionIndex = _selectedUnitIndex;
            inputHandler.OnSelectUnitPlayerSelectionMove = null;
        }
    }

    public int GetSelectionIndex(SIDE side)
    {
        if (side == SIDE.PLAYER)
            return _previousPlayerSelectionIndex;
        else if (side == SIDE.ENEMY)
            return _previousEnemySelectionIndex;
        else
            return 0;
    }

    private void OnUnitSelect(int value)
    {
        _selectedUnitIndex = Mathf.Clamp(_selectedUnitIndex + value, 0, _maxUnitCount - 1);
        select(_selectedUnitIndex);
    }
}

using DataEnum;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UnitSelectorController", menuName = "GameScene/UnitSelectorController", order = 1)]
public class UnitSelectorController : ScriptableObject
{
    [SerializeField] private InputHandler inputHandler;
    public InputHandler InputHandler => inputHandler;

    private UnityAction confirm;
    private UnityAction<int> select;

    private int _selectedUnitIndex;
    private int _previousEnemySelectionIndex;
    private int _previousPlayerSelectionIndex;

    private int _maxUnitCount;

    public void Initialize(UnityAction confirm, UnityAction<int> select)
    {
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

    public void OnEndSelect(SIDE side)
    {
        if (side == SIDE.ENEMY)
            _previousEnemySelectionIndex = _selectedUnitIndex;
        else if (side == SIDE.PLAYER)
            _previousPlayerSelectionIndex = _selectedUnitIndex;
    }

    private void OnUnitSelect(int value)
    {
        _selectedUnitIndex = Mathf.Clamp(_selectedUnitIndex + value, 0, _maxUnitCount - 1);
        select(_selectedUnitIndex);
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
}

using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
class ActionSelector : ScriptableObject
{
    [SerializeField] private ActionFactory _actionFactory;
    [SerializeField] private ActionSelectionButtons selectorPrefab;
    [SerializeField] private InputHandler inputHandler;
    private ActionSelectionButtons selector;
    
    private int _selectedActionType;
    
    public void Init()
    {
        _actionFactory.Init();

        selector = Instantiate(selectorPrefab);
        selector.gameObject.SetActive(false);

        selector.SetBasicAttackButtonListener(() => _selectedActionType = 1);
    }
    
    public async UniTask<IUnitAction> SelectAction(PlayerUnit playerUnit)
    {
        Debug.Log($"{playerUnit.GetStat().Name} : Select Action");
        
        selector.transform.position = playerUnit.attachments.GetActionSelectorPos().position;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;

        using (var inputDisposer = new InputDisposer(inputHandler, InputHandler.InputState.SelectAction))
        {
            await UniTask.WaitUntil(() => _selectedActionType != 0);
        }

        selector.gameObject.SetActive(false);

        IUnitAction unitAction = null;
        switch (_selectedActionType)
        {
            case 1:
                unitAction = await _actionFactory.CreatePlayerBaseAttackAction(playerUnit);
                break;
        }

        return unitAction;
    }

    public async UniTask<IUnitAction> SelectAction(EnemyUnit enemyUnit)
    {
        // TODO : Add Other Actions
        //_selectedActionType = Random.Range(0, 3);

        IUnitAction unitAction = null;
        unitAction = await _actionFactory.CreateEnemyBaseAttackAction(enemyUnit);

        return unitAction;
    }    
}
using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
class ActionSelector : ScriptableObject
{
    [SerializeField] private ActionFactory _actionFactory;
    [SerializeField] private ActionSelectionButtons selectorPrefab;
    private ActionSelectionButtons selector;
    
    private int _selectedActionType;
    
    public void Init()
    {
        selector = Instantiate(selectorPrefab);
        selector.gameObject.SetActive(false);

        selector.SetBasicAttackButtonListener(() => _selectedActionType = 1);
    }
    
    public async UniTask<IUnitAction> SelectAction(PlayerUnit playerUnit)
    {
        Debug.Log($"{playerUnit.GetStat().Name} : Select Action");
        
        // TODO : Set correct position
        selector.transform.position = playerUnit.attachments.GetActionSelectorPos().position;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;
        
        await UniTask.WaitUntil(() => _selectedActionType != 0);

        selector.gameObject.SetActive(false);

        IUnitAction unitAction = null;
        switch (_selectedActionType)
        {
            case 1:
                unitAction = await _actionFactory.CreateBaseAttackAction(playerUnit);
                break;
        }

        return unitAction;
    }
}
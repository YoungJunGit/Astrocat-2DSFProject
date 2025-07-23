using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ActionSelector", menuName = "GameScene/ActionSelector", order = 1)]
class ActionSelector : ScriptableObject
{
    [SerializeField] private InputHandler inputHandler;
    
    [SerializeField] private ActionSelectionButtons selectorPrefab;
    private Transform selectorRoot;
    private ActionSelectionButtons selector;
    
    private int _selectedActionType = 0;
    
    public void Init(Transform selectorRoot)
    {
        this.selectorRoot = selectorRoot;
        
        selector = Instantiate(selectorPrefab, selectorRoot);
        selector.gameObject.SetActive(false);

        selector.SetBasicAttackButtonListener(() => _selectedActionType = 1);
    }
    
    public async UniTask<UnitAction> SelectAction(PlayerUnit playerUnit)
    {
        Debug.Log($"{playerUnit.GetStat().Name} : Select Action");
        
        // TODO : Set correct position
        selector.transform.position = playerUnit.transform.position;
        selector.gameObject.SetActive(true);

        _selectedActionType = 0;
        
        await UniTask.WaitUntil(() => _selectedActionType != 0);

        UnitAction unitAction = null;
        switch (_selectedActionType)
        {
            case 1:
                unitAction = new PlayerBaseAttackAction().SetCaster(playerUnit);
                break;
                
        }
        
        return unitAction;
    }
}


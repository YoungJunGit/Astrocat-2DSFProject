using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class ActionFactory : ScriptableObject
{
    [SerializeField] private UnitManager _unitManager;
    
    public async UniTask<PlayerBaseAttackAction> CreateBaseAttackAction(PlayerUnit playerUnit)
    {
        var enemy = await _unitManager.GetEnemyUnitBySelector();
        
        return new PlayerBaseAttackAction(playerUnit, enemy);
    }
}
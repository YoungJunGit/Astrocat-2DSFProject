using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionFactory", menuName = "GameScene/ActionFactory", order = 3)]
class ActionFactory : ScriptableObject
{
    [SerializeField] private UnitSelector _unitSelector;
    
    public void Init()
    {
        _unitSelector.Init();
    }

    public async UniTask<PlayerBaseAttackAction> CreateBaseAttackAction(PlayerUnit playerUnit)
    {
        EnemyUnit enemy = await _unitSelector.SelectUnit(DataEnum.SIDE.ENEMY) as EnemyUnit;
        
        return new PlayerBaseAttackAction(playerUnit, enemy);
    }
}
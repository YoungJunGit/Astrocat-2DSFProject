using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

class UnitManager : ScriptableObject
{
    static UnitSelector _unitSelector = new();

    public void Init(UnitSelector unitSelector)
    {
        
    }
    
    public static PlayerUnit GetCurrentPlayerUnit()
    {
        return null;
    }

    public static async UniTask<EnemyUnit> GetEnemyUnitBySelector()
    {
        return null;
    }
    
    public static async UniTask<PlayerUnit> GetPlayerUnitBySelector()
    {
        return null;
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;

class UnitSelector
{
    public async UniTask<EnemyUnit> SelectEnemyUnit()
    {
        Debug.Log("Select Enemy Unit");
        await UniTask.WaitUntil(() => Input.anyKeyDown);
        
        return null;
    }
    
    public async UniTask<PlayerUnit> SelectPlayerUnit()
    {
        Debug.Log("Select Player Unit");
        await UniTask.WaitUntil(() => Input.anyKeyDown);
 
        return null;
    }
}
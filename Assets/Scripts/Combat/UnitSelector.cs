
using Cysharp.Threading.Tasks;
using UnityEngine;

class UnitSelector
{
    public async UniTask<EnemyUnit> SelectEnemyUnit()
    {
        //Debug.Log(typeof(PlayerUnit) + " : Select Enemy Unit");
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        
        return null;
    }
    
    public async UniTask<PlayerUnit> SelectPlayerUnit()
    {
        //Debug.Log(typeof(EnemyUnit) + " : Select Player Unit");
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
 
        return null;
    }
}
using Cysharp.Threading.Tasks;
using UnityEngine;

class ActionSelector
{
    public async UniTask<int> SelectAction()
    {
        Debug.Log("Select Action");
        await UniTask.WaitUntil(() => Input.anyKeyDown);
        
        return 0;
    }
}
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public interface IUnitActionExecuter
{
    public UniTask ExecuteRequest(IUnitAction action);
}

public class UnitActionExecuter : IUnitActionExecuter
{
    public async UniTask ExecuteRequest(IUnitAction action)
    {
        var context = new UnitActionContext();
        
        action.Execute(context);
    }
}

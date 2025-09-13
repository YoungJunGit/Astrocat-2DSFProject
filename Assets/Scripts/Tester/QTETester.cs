using UnityEngine;

public class QTETester : MonoBehaviour
{
    private QTEManager qteManager;
    private InputHandler inputHandler;

    public void Init()
    {
        ServiceLocator.For(this)
            .Get(out qteManager)
            .Get(out inputHandler);
        
        inputHandler.OnSelectActionSkillSelect += Test;
    }

    private void Test()
    {
        qteManager.StartSingleQTE(1f);
    }
}
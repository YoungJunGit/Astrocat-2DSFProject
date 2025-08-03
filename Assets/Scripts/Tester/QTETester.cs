using UnityEngine;

public class QTETester : MonoBehaviour
{
    [SerializeField] private QTEManager qteManager;
    [SerializeField] private InputHandler inputHandler;

    public void Init()
    {
        inputHandler.OnSelectActionSkillSelect += Test;
    }

    private void Test()
    {
        qteManager.StartSingleQTE(1f);
    }
}
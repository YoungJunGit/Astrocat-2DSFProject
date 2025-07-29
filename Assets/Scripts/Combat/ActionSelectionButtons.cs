using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionButtons : MonoBehaviour
{
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button useItemButton;
    
    public void SetBasicAttackButtonListener(UnityEngine.Events.UnityAction action)
    {
        basicAttackButton.onClick.RemoveAllListeners();
        basicAttackButton.onClick.AddListener(action);
    }
}

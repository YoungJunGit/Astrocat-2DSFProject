using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionButtons : MonoBehaviour
{
    [SerializeField] private Button basicAttackButton;
    [SerializeField] private Button skillButton;
    [SerializeField] private Button useItemButton;
    [SerializeField] private GameObject skillSelectionButtonsGO;
    [SerializeField] private UI_SelectionButton skillButtonGO;
    
    public Action<int> OnBasicSelection = null;
    public Action<int> OnSkillSelection = null;

    public void Init()
    {
        basicAttackButton.onClick.AddListener(() => OnBasicSelection?.Invoke(1));
        skillButton.onClick.AddListener(() => OnBasicSelection?.Invoke(2));
        useItemButton.onClick.AddListener(() => OnBasicSelection?.Invoke(3));
    }
    
    public void EnableSkillSelectionButtons(string[] skillTexts)
    {
        for (int i = 1; i <= skillTexts.Length; i++)
        {
            int index = i;
            var go = Instantiate(skillButtonGO, skillSelectionButtonsGO.transform);
            go.Button.onClick.AddListener(() => OnSkillSelection?.Invoke(index));
            go.Text.text = skillTexts[i - 1];
        }
    }
    
    public void DisableSkillSelectionButtons()
    {
        foreach (Transform child in skillSelectionButtonsGO.transform)
        {
            Destroy(child.gameObject);
        }
    }
}

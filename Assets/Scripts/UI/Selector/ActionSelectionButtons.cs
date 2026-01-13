using System;
using DataEntity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using DG.Tweening;
using static ActionSelectionSetting;
using System.Linq;

public class ActionSelectionButtons : MonoBehaviour
{
    public enum ActionSelectType
    {
        BasicAttack = 0,
        Skill,
        UseItem
    }

    [Header("Buttons")]
    [SerializeField] private SerializedDictionary<ActionSelectType, GameObject> buttons;

    [Header("Skill Contents")]
    [SerializeField] private GameObject skillSelectPanel;
    [SerializeField] private GameObject skillSelectionBtnPanel;
    [SerializeField] private UI_SelectionButton skillBtnGameObj;

    [Header("Disable Values")]
    [SerializeField] float hologramBlend = 0.5f;
    [SerializeField] float chromAberrAmount = 0.25f;
    [SerializeField] float glitchAmount = 3.5f;
    [SerializeField] float duration = 1.0f;

    private Dictionary<ActionSelectType, Button> btns = new();
    private Dictionary<ActionSelectType, Image> btnImages = new();
    private Dictionary<ActionSelectType, ActionSelectionSetting> btnSettings = new();

    public Action<int> OnBasicSelection = null;
    public Action<int> OnSkillSelection = null;
    public Action OnSelectActionStart = null;
    public Action OnSelectActionEnd = null;

    private bool silenceOn = false;

    private Button previousBtn;

    public void Init()
    {
        skillSelectPanel.SetActive(false);
        var values = (ActionSelectType[])Enum.GetValues(typeof(ActionSelectType));
        foreach (var type in values)
        {
            btns.Add(type, buttons[type].GetComponent<Button>());
            btnImages.Add(type, buttons[type].GetComponent<Image>());
            btnSettings.Add(type, buttons[type].GetComponent<ActionSelectionSetting>());

            int result = (int)type;
            btns[type].onClick.AddListener(() => OnBasicSelection?.Invoke(result + 1));
            btnImages[type].alphaHitTestMinimumThreshold = 0.1f;
            btnSettings[type].Init();
            btnSettings[type].OnDeselectAction += (deselected) => previousBtn = deselected;
        }

        btnImages[ActionSelectType.Skill].material.SetFloat("_HologramBlend", 0.0f);
        btnImages[ActionSelectType.Skill].material.SetFloat("_ChromAberrAmount", 0.0f);
        btnImages[ActionSelectType.Skill].material.SetFloat("_GlitchAmount", 0.0f);
        btnSettings[ActionSelectType.UseItem].OnSelectAction += () => 
        {
            if(previousBtn != btns[ActionSelectType.UseItem])
                btnSettings[ActionSelectType.UseItem].ChangeExplicit(Direction.UP, previousBtn);
        };
    }

    public void OnSelectStart()
    {
        previousBtn = btns[ActionSelectType.BasicAttack];
        foreach (var btn in btns)
        {
            btn.Value.interactable = true;
        }

        if(silenceOn)
            btns[ActionSelectType.Skill].interactable = false;

        OnSelectActionStart?.Invoke();
    }

    public void OnSelectEnd()
    {
        previousBtn = null;
        foreach (var btn in btns)
        {
            btn.Value.interactable = false;
        }

        OnSelectActionEnd?.Invoke();
    }
    
    public void SetSkillBtnEffect(bool toggle)
    {
        silenceOn = !toggle;
        ActionSelectType type = ActionSelectType.Skill;
        if (toggle)
        {
            btnImages[type].material.DOFloat(0.0f, "_HologramBlend", duration);
            btnImages[type].material.DOFloat(0.0f, "_ChromAberrAmount", duration);
            btnImages[type].material.DOFloat(0.0f, "_GlitchAmount", duration);

            btnSettings[ActionSelectType.BasicAttack].ChangeExplicit(Direction.RIGHT, btns[ActionSelectType.Skill]);
            btnSettings[ActionSelectType.UseItem].ChangeExplicit(Direction.RIGHT, btns[ActionSelectType.Skill]);
        }
        else
        {
            btnImages[type].material.DOFloat(hologramBlend, "_HologramBlend", duration);
            btnImages[type].material.DOFloat(chromAberrAmount, "_ChromAberrAmount", duration);
            btnImages[type].material.DOFloat(glitchAmount, "_GlitchAmount", duration);

            btnSettings[ActionSelectType.BasicAttack].ChangeExplicit(Direction.RIGHT, btns[ActionSelectType.UseItem]);
            btnSettings[ActionSelectType.UseItem].ChangeExplicit(Direction.RIGHT, null);
        }
    }
    
    public void EnableSkillSelectionButtons(int SP, SkillData[] skills)
    {
        skillSelectPanel.SetActive(true);
        for (int i = 1; i <= skills.Length; i++)
        {
            int index = i;
            var obj = Instantiate(skillBtnGameObj, skillSelectionBtnPanel.transform);
            obj.SetImage(i - 1);
            obj.SetInteractable(SP >= skills[i - 1].Skill_Cost_SP);
            obj.Button.onClick.AddListener(() => OnSkillSelection?.Invoke(index));
            obj.SkillName.text = skills[i - 1].Skill_Name;
            obj.SkillCost.text = $"SP {skills[i - 1].Skill_Cost_SP}";
        }
    }
    
    public void DisableSkillSelectionButtons()
    {
        foreach (Transform child in skillSelectionBtnPanel.transform)
        {
            Destroy(child.gameObject);
        }
        skillSelectPanel.SetActive(false);
    }
}

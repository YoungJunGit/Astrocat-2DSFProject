using DataEntity;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using DG.Tweening;

public class PlayerHUD : BaseHUD
{
    [Header("AP")]
    [SerializeField] private TMP_Text ap_Text;
    [SerializeField] private GameObject ap_Panel;
    [SerializeField] private GameObject ap_Box;
    [SerializeField] private Color ActivateColor;
    [SerializeField] private Color DeactivateColor;

    [Space(10f)]
    [SerializeField] private Image statusBox;
    [SerializeField] private TMP_Text unitName;
    [SerializeField] private Color DieColor;

    private List<Image> ap_BoxList = new List<Image>();

    protected override void UpdateIconBoxSize<T>(List<T> iconList)
    {

    }

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);

        unitName.text = unit.GetStat().CoreStat.Name;

        for(int i = 0; i < unit.GetStat().ModifierStat.MaxSP; i++)
        {
            var img = Instantiate(ap_Box, ap_Panel.transform).GetComponent<Image>();
            ap_BoxList.Add(img);
        }

        unit.GetStat().OnHPChanged += this.OnHPChanged;
        unit.GetStat().OnSPChanged += this.OnSPChanged;
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        float targetValue= curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";

        hp_Slider.DOKill();

        hp_Slider.direction = Slider.Direction.RightToLeft;
        hp_Slider.DOValue(targetValue, hpTweenDuration);

        if (curHp <= 0)
        {
            statusBox.color = DieColor;
        }
        else
        {
            statusBox.color = Color.white;
        }
    }

    public void OnSPChanged(int curAp, int maxAp)
    {
        foreach (var box in ap_BoxList.Select((value, index) => (value, index)))
        {
            box.value.color = box.index < curAp ? ActivateColor : DeactivateColor;
        }

        ap_Text.text = $"{curAp}/{maxAp}";
    }
}

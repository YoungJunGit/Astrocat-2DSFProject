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
    [Header("Image")]
    [SerializeField] private Image characterImg;
    [SerializeField] private CharacterPortraitInfo portraitInfo;

    [Header("AP")]
    [SerializeField] private TMP_Text ap_Text;
    [SerializeField] private GameObject ap_Panel;
    [SerializeField] private GameObject ap_Box;
    [SerializeField] private Color ActivateColor;
    [SerializeField] private Color DeactivateColor;

    [Space(10.0f)]
    [SerializeField] private CanvasGroup ui;
    [SerializeField] private RectTransform _effectBoxRectTransform;

    protected override RectTransform effectBoxRectTransform => _effectBoxRectTransform;
    private List<Image> ap_BoxList = new List<Image>();

    public override void Initialize(BaseUnit unit)
    {
        base.Initialize(unit);

        if (!portraitInfo.CharacterProtraitDic.TryGetValue(unit.GetStat().CoreStat.AssetFileName, out var sprite))
        {
            throw new System.Exception("Character Portrait has not been assigned properly!!!");
        }

        characterImg.sprite = sprite;

        for(int i = 0; i < unit.GetStat().ModifierStat.MaxSP; i++)
        {
            var img = Instantiate(ap_Box, ap_Panel.transform).GetComponent<Image>();
            ap_BoxList.Add(img);
        }

        unit.GetStat().OnSPChanged += this.OnSPChanged;
    }

    public void OnSPChanged(int curAp, int maxAp)
    {
        foreach (var box in ap_BoxList.Select((value, index) => (value, index)))
        {
            box.value.color = box.index < curAp ? ActivateColor : DeactivateColor;
        }

        ap_Text.text = $"{curAp}/{maxAp}";
    }

    public override void OnFinishedDying(BaseUnit me)
    {
        ui.alpha = 0.5f;
    }
}

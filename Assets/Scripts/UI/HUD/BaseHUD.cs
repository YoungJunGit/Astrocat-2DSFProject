using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using ObservableCollections;
using R3;
using System.Collections.Generic;
using DataEnum;
using DG.Tweening;
using System;

public abstract class BaseHUD : MonoBehaviour
{
    [Header("etc")]
    [SerializeField] private Button showEffectInfoBtn;

    [Header("Icons")]
    [SerializeField] private GameObject CCIconPrefab;
    [SerializeField] private GameObject BuffIconPrefab;

    [Header("HP"), Space(10f)]
    [SerializeField] private TMP_Text hp_Text;
    [SerializeField] private Image hp_Image;
    [SerializeField] private float hp_Tween_Duration = 0.5f;

    [Header("Element Gauge"), Space(10f)]
    [SerializeField] private ElementGaugeContainer eg_Container;
    [SerializeField] private Image eg_Panel_Image;
    [SerializeField] private Image eg_Content_Image;
    [SerializeField] private float eg_Tween_Duration = 0.5f;
    [SerializeField] private float eg_Outline_Width = 0.022f;

    protected abstract RectTransform effectBoxRectTransform { get; }
    private List<ElementIcon> _ccIconList = new();
    private List<BuffIcon> _buffIconList = new();

    public event Action<ELEMENT_TYPE> OnElementGaugeFull = delegate { };

    private int fillStack = 0;
    const string EG_SCALE_TWEEN_ID = "EG_PANEL_SCALE";

    public virtual void Initialize(BaseUnit unit)
    {
        // Attach Events
        unit.GetStat().OnHPChanged += OnHPChanged;
        unit.GetStat().OnEGChanged += OnEGChanged;
        unit.m_FinishedDying += OnFinishedDying;

        // Create new EG Panel Image Material for using AllIn1Shader in each prefabs
        eg_Panel_Image.material = new Material(eg_Panel_Image.material);

        // Icon Data Binding
        var stream = unit.CCUnit.EffectsCountDic.Select(kv => kv.Value.Select(_ => new { Element = kv.Key, Count = kv.Value })).Merge();
        stream.Subscribe(value =>
            {
                var icon = _ccIconList.Find(e => e.IconType == value.Element);

                // 삭제
                if (value.Count.CurrentValue <= 0)
                {
                    if (icon != null)
                    {
                        _ccIconList.Remove(icon);
                        Destroy(icon.gameObject);
                    }
                    return;
                }

                // 생성
                if (icon == null)
                {
                    icon = CreateIcon(_ccIconList, effectBoxRectTransform);
                    icon.Init(value.Element, value.Count.CurrentValue);
                    icon.enabled = true;
                    return;
                }

                // 갱신 (이게 핵심)
                icon.UpdateIcon(value.Count.CurrentValue);
            }
        );
    }

    public void OnHPChanged(float curHp, float maxHp)
    {
        float targetValue = curHp / maxHp;
        hp_Text.text = $"{(int)curHp}/{(int)maxHp}";

        hp_Image.DOKill();
        hp_Image.DOFillAmount(targetValue, hp_Tween_Duration);
    }

    public void OnEGChanged(ELEMENT_TYPE elementType, bool reset, int fillCount, float curEG, float MaxEG)
    {
        eg_Content_Image.sprite = eg_Container.GaugeSpriteList[elementType];

        eg_Content_Image.DOKill();

        if (reset)
        {
            eg_Content_Image.fillAmount = 0.0f;
        }

        fillStack += fillCount;
        float value = curEG / MaxEG;
        float totalFillAmount = fillStack + value;
        float timePerUnit = eg_Tween_Duration / totalFillAmount;

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < fillStack; i++)
        {
            seq.Append(eg_Content_Image.DOFillAmount(1f, timePerUnit).SetEase(Ease.Linear));
            seq.AppendCallback(() =>
            {
                OnEGFullCharge(elementType);
            });
        }

        seq.Append(eg_Content_Image.DOFillAmount(value, value * timePerUnit).SetEase(Ease.Linear));

        seq.SetEase(Ease.OutQuad);

        seq.Play();
    }

    private void OnEGFullCharge(ELEMENT_TYPE elementType)
    {
        fillStack--;
        eg_Content_Image.fillAmount = 0.0f;
        OnElementGaugeFull.Invoke(elementType);

        DOTween.Kill(EG_SCALE_TWEEN_ID);
        eg_Panel_Image.transform.localScale = Vector3.one * 1.2f;
        eg_Panel_Image.transform.DOScale(Vector3.one, 0.5f).SetEase(eg_Container.customEase).SetId(EG_SCALE_TWEEN_ID);

        //eg_Panel_Image.material.DOFloat(0.022f, "_OutlineWidth", 1.0f);
        eg_Panel_Image.material.SetFloat("_OutlineWidth", 0.022f);

        //eg_Panel_Image.material.DOColor(eg_Container.GlowColorList[elementType], "_OutlineColor", 1.0f);
        eg_Panel_Image.material.SetColor("_OutlineColor", eg_Container.GlowColorList[elementType]);
    }

    private T CreateIcon<T>(List<T> iconList, Transform parent)
    {
        var objectToInstantiate = typeof(T) == typeof(ElementIcon) ? CCIconPrefab : BuffIconPrefab;
        var icon = Instantiate(objectToInstantiate, parent, false).GetComponent<T>();
        iconList.Add(icon);

        return icon;
    }

    public abstract void OnFinishedDying(BaseUnit me);
}

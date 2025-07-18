using Unity.VisualScripting;
using UnityEngine;

public class EnemyHUD : BaseHUD
{
    [Space(10f)]
    [SerializeField] private Vector3 spawnOffset;

    public override void Initialize(BaseUnit unit)
    {
        Vector3 spawnPos = unit.transform.Find("StatusBoxPos").transform.position;
        transform.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(spawnPos + spawnOffset);

        unit.GetStat().OnHPChanged += OnHPChanged;
        unit.GetStat().OnDie += OnDied;
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        hp_Slider.value = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";
    }

    public override void OnDied()
    {
        Destroy(gameObject);
    }
}

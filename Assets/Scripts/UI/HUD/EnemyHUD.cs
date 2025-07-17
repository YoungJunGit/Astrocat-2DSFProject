using Unity.VisualScripting;
using UnityEngine;

public class EnemyHUD : BaseHUD
{
    [Space(10f)]
    [SerializeField] private Vector3 spawnOffset;

    public override void Initialize(BaseUnit unit)
    {
        ControlledUnit = unit;
        Vector3 spawnPos = unit.transform.Find("StatusBoxPos").transform.position;
        transform.GetComponent<RectTransform>().position = Camera.main.WorldToScreenPoint(spawnPos + spawnOffset);

        ControlledUnit.GetStat().m_HPChanged += OnHPChanged;
        ControlledUnit.GetStat().m_Die += OnDied;
    }

    public override void OnHPChanged()
    {
        hp_Slider.value = ControlledUnit.GetStat().Cur_HP / ControlledUnit.GetStat().Max_HP;
        hp_Text.text = ControlledUnit.GetStat().Cur_HP + " / " + ControlledUnit.GetStat().Max_HP;
    }

    public override void OnDied()
    {
        Destroy(gameObject);
    }
}

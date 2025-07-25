using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EnemyHUD : BaseHUD
{
    [Space(10f)]
    [SerializeField] private Vector3 posOffset;
    private RectTransform rectTransform;
    private Transform statusPos;

    private CancellationTokenSource update = new();

    public override void Initialize(BaseUnit unit)
    {
        rectTransform = GetComponent<RectTransform>();
        unit.GetStat().OnHPChanged += OnHPChanged;
        unit.GetStat().OnDie += OnDied;
    }

    public void AttachHUD(Transform statusPos)
    {
        this.statusPos = statusPos;
        UpdatePosition().Forget();
    }

    private async UniTask UpdatePosition()
    {
        while (true)
        {
            await UniTask.Yield(cancellationToken: update.Token);
            rectTransform.position = Camera.main.WorldToScreenPoint(statusPos.position + posOffset);
        }
    }

    public override void OnHPChanged(float curHp, float maxHp)
    {
        hp_Slider.value = curHp / maxHp;
        hp_Text.text = $"{curHp}/{maxHp}";
    }

    public override void OnDied(UnitStat stat)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        update.Cancel();
        update.Dispose();
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "GameScene/HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;

    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private EnemyHUD enemyHUDPrefab;
    [SerializeField] private EntityBanner bannerPrefab;

    [SerializeField] private StatusCanvas statuesCanvasPref;
    [SerializeField] private TimelineCanvas timelineCanvasPrefab;
    private StatusCanvas statuesCanvas;
    private TimelineCanvas timelineCanvas;
    
    private TimelineSystem timeline;
 
    public void Init(TimelineSystem timeLine)
    {
        this.timeline = timeLine;

        statuesCanvas = Instantiate(statuesCanvasPref);
        timelineCanvas = Instantiate(timelineCanvasPrefab);

        this.timeline.m_TimelineChanged += OnTimelineChanged;
        this.timeline.Init(timelineCanvas.GetComponentInChildren<TimelineUI>());
    }

    public void Prepare()
    {
        foreach(BaseUnit unit in unit_HUD_Dic.Keys)
        {
            // For initializing unit's HUD
            unit.GetStat().OnPrepareCombat();
        }
    }

    public PlayerHUD CreatePlayerHUD(PlayerUnit unit)
    {
        PlayerHUD hud = Instantiate(playerHUDPrefab).GetComponent<PlayerHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);

        statuesCanvas.SetPlayerHUD(hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);

        statuesCanvas.SetEnemyHUD(hud, unit.GetStatusPosition());

        return hud;
    }

    public void CreateBanners()
    {
        timeline.AddTimeline(unit_HUD_Dic.GetUnits());
        timelineCanvas.SetBanners(timeline.GetBannerList());
    }

    /// <summary>
    /// This Called when received message -> TimelineSystem : m_EndTurn
    /// </summary>
    public void OnTimelineChanged()
    {
        timeline.AddTimeline(unit_HUD_Dic.GetUnits());
        timelineCanvas.SetParent(timeline.GetBannerList());
    }
}

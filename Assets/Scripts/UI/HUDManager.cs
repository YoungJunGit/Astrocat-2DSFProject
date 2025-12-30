using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "GameScene/HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private EnemyHUD enemyHUDPrefab;

    [SerializeField] private HUDCanvas statusCanvasPref;
    private HUDCanvas statusCanvas;

    private Dictionary<BaseUnit, BaseHUD> unit_HUD_Dic = new Dictionary<BaseUnit, BaseHUD>();

    IUnitManager _unitManager;

    public void Init()
    {
        statusCanvas = Instantiate(statusCanvasPref);
        statusCanvas.Init();
        
        ServiceLocator.For(this)
            .Get(out _unitManager);
    }

    public void Prepare()
    {
        foreach(BaseUnit unit in _unitManager.GetAllUnits())
        {
            // For initializing unit's HUD
            unit.GetStat().OnPrepareCombat();
            unit_HUD_Dic[unit].OnElementGaugeFull += unit.OnElementGaugeFull;
        }
    }

    public PlayerHUD CreatePlayerHUD(PlayerUnit unit)
    {
        PlayerHUD hud = Instantiate(playerHUDPrefab).GetComponent<PlayerHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);
        statusCanvas.SetPlayerHUD(hud, unit.GetStat().Priority);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);
        statusCanvas.SetEnemyHUD(hud, unit.Attachments.GetStatusPosition(), unit.Attachments.GetBuffBoxPosition(), unit.GetStat().Priority);

        return hud;
    }
}

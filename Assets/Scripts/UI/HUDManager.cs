using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "GameScene/HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private ScriptableListBaseUnit unitList = null;

    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private EnemyHUD enemyHUDPrefab;

    [SerializeField] private StatusCanvas statusCanvasPref;
    private StatusCanvas statusCanvas;

    Dictionary<PlayerUnit, PlayerHUD> playerHudDic = new Dictionary<PlayerUnit, PlayerHUD>();
    Dictionary<EnemyUnit, EnemyHUD> enemyHudDic = new Dictionary<EnemyUnit, EnemyHUD>();

    public void Init()
    {
        statusCanvas = Instantiate(statusCanvasPref);
    }

    public void Prepare()
    {
        foreach(BaseUnit unit in unitList)
        {
            // For initializing unit's HUD
            unit.GetStat().OnPrepareCombat();
        }
    }

    public PlayerHUD CreatePlayerHUD(PlayerUnit unit)
    {
        PlayerHUD hud = Instantiate(playerHUDPrefab).GetComponent<PlayerHUD>();
        hud.Initialize(unit);
        playerHudDic.Add(unit, hud);
        statusCanvas.SetPlayerHUD(hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        enemyHudDic.Add(unit, hud);
        statusCanvas.SetEnemyHUD(hud, unit.attachments.GetStatusPosition());

        return hud;
    }
}

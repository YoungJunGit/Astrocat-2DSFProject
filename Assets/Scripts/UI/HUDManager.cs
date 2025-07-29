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
        unitList.Add(unit);

        statusCanvas.SetPlayerHUD(hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unitList.Add(unit);

        statusCanvas.SetEnemyHUD(hud, unit.attachments.GetStatusPosition());

        return hud;
    }

    public void DeletePlayerHUD(PlayerUnit playerUnit) {
        if (unit_HUD_Dic.TryGetValue(playerUnit, out var hud))
        {
            if (hud is PlayerHUD playerHud)
            {
                playerHud.OnDied();
            }
            unit_HUD_Dic.Remove(playerUnit);
        }
    }
    public void DeleteEnemyHUD(EnemyUnit enemyUnit)
    {
        if (unit_HUD_Dic.TryGetValue(enemyUnit, out var hud))
        {
            if (hud is EnemyHUD enemyHud)
            {
                enemyHud.OnDied();
            }
            unit_HUD_Dic.Remove(enemyUnit);
        }
    }

    public void repositionEnemyHUD() {
        foreach (var leftHud in unit_HUD_Dic)
        {
            if (leftHud.Value is EnemyHUD enemyHud)
            {
                enemyHud.Initialize(leftHud.Key);
            }
        }
    }

}

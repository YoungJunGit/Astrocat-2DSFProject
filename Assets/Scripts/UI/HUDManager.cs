using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "GameScene/HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;

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

        statusCanvas.SetPlayerHUD(hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);

        statusCanvas.SetEnemyHUD(hud, unit.GetStatusPosition());

        return hud;
    }
}

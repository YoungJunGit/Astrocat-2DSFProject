using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "GameScene/HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private EnemyHUD enemyHUDPrefab;

    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;
    
    [SerializeField] private StatusCanvas statuesCanvasPref;
    private StatusCanvas statuesCanvas;
    
    public void Init()
    {
        statuesCanvas = Instantiate(statuesCanvasPref);
    }

    public PlayerHUD CreatePlayerHUD(PlayerUnit unit)
    {
        PlayerHUD hud = Instantiate(playerHUDPrefab).GetComponent<PlayerHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);
        
        statuesCanvas.SetPlayerHUD(hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit, int index)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);
        
        statuesCanvas.SetEnemyHUD(hud, index);

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

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

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        EnemyHUD hud = Instantiate(enemyHUDPrefab).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);
        
        statuesCanvas.SetEnemyHUD(hud);

        return hud;
    }
}

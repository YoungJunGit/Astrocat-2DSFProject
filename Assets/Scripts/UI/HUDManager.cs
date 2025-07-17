using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "HUDManager", menuName = "HUDManager", order = 2)]
public class HUDManager : ScriptableObject
{
    [SerializeField] private PlayerHUD playerHUDPrefab;
    [SerializeField] private EnemyHUD enemyHUDPrefab;

    [SerializeField] private ScriptableDictionaryUnit_HUD unit_HUD_Dic = null;

    public void Initialize()
    {
        
    }

    public PlayerHUD CreatePlayerHUD(PlayerUnit unit)
    {
        Transform spawnTransform = GameObject.Find("PlayerStatus/StatusPanel").transform;
        PlayerHUD hud = Instantiate(playerHUDPrefab, spawnTransform, false).GetComponent<PlayerHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);

        return hud;
    }

    public EnemyHUD CreateEnemyHUD(EnemyUnit unit)
    {
        Transform spawnTransform = GameObject.Find("EnemyStatus").transform;
        EnemyHUD hud = Instantiate(enemyHUDPrefab, spawnTransform, false).GetComponent<EnemyHUD>();
        hud.Initialize(unit);
        unit_HUD_Dic.Add(unit, hud);

        return hud;
    }
}

using System;
using UnityEngine;

public class StatusCanvas : MonoBehaviour
{
    [SerializeField] Transform _playerStatusPanel;
    [SerializeField] Transform _enemyStatusPanel;

    [SerializeField] private UnitPositioner unitPositioner;

    public void SetPlayerHUD(PlayerHUD playerHud)
    {
        playerHud.transform.SetParent(_playerStatusPanel, false);
    }

    public void SetEnemyHUD(EnemyHUD enemyHud, Transform transform)
    {
        enemyHud.transform.SetParent(_enemyStatusPanel, false);

        if (transform != null)
        {
            enemyHud.AttachHUD(transform);
        }
        else
        {
            Debug.LogWarning($"StatusPosition is not set Properly!!!");
        }
    }
}

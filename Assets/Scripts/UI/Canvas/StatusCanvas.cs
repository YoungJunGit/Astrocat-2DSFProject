using System;
using UnityEngine;

public class StatusCanvas : MonoBehaviour
{
    [SerializeField] Transform _playerStatusPanel;
    [SerializeField] Transform _enemyStatusPanel;

    public void SetPlayerHUD(PlayerHUD playerHud)
    {
        playerHud.transform.SetParent(_playerStatusPanel, false);
    }

    public void SetEnemyHUD(EnemyHUD enemyHud, int index)
    {
        enemyHud.transform.SetParent(_enemyStatusPanel, false);
    }
}

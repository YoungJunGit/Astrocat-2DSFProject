using System;
using UnityEngine;

public class HUDCanvas : BaseCanvas
{
    [SerializeField] Transform _playerStatusPanel;
    [SerializeField] Transform _enemyStatusPanel;

    public void SetPlayerHUD(PlayerHUD playerHud, int index)
    {
        playerHud.transform.SetParent(_playerStatusPanel, false);
    }

    public void SetEnemyHUD(EnemyHUD enemyHud, Transform statusTransform, Transform buffBoxTransform, int index)
    {
        enemyHud.transform.SetParent(_enemyStatusPanel, false);
        enemyHud.AttachHUD(statusTransform, buffBoxTransform);
    }
}

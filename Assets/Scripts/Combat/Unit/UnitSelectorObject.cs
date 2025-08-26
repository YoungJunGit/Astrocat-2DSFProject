using NaughtyAttributes;
using UnityEngine;

public class UnitSelectorObject : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController enemy_select_controller;
    [SerializeField] private RuntimeAnimatorController player_select_controller;

    public void Set(DataEnum.SIDE side)
    {
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = side == DataEnum.SIDE.PLAYER ? player_select_controller : enemy_select_controller;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = side == DataEnum.SIDE.PLAYER ? -1 : 1;
    }
}

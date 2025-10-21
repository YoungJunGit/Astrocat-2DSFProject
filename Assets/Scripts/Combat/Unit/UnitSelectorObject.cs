using DataEnum;
using NaughtyAttributes;
using UnityEngine;

public class UnitSelectorObject : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController enemy_select_controller;
    [SerializeField] private RuntimeAnimatorController player_select_controller;
    private SpriteRenderer _spriteRenderer;

    public void Init(SIDE side, bool isSelectable)
    {
        Animator animator = GetComponent<Animator>();
        _spriteRenderer   = GetComponent<SpriteRenderer>();
        animator.runtimeAnimatorController = side == SIDE.PLAYER ? player_select_controller : enemy_select_controller;
        _spriteRenderer.sortingOrder       = side == SIDE.PLAYER ? -1 : 1;

        if (isSelectable)
            _spriteRenderer.color = Color.white;
        else
            _spriteRenderer.color = Color.red;
    }
}

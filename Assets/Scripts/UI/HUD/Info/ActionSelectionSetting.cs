using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using UnityEngine.EventSystems;
using System;

public class ActionSelectionSetting : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public enum Direction
    {
        UP = 0,
        DOWN,
        LEFT,
        RIGHT
    }

    [SerializeField] 
    private SerializedDictionary<Direction, Button> buttons;

    private Button target;

    public event Action OnSelectAction = delegate{ };
    public event Action<Button> OnDeselectAction = delegate{ };

    public void Init()
    {
        target = GetComponent<Button>();
        SetExplicit(buttons[Direction.UP], buttons[Direction.DOWN], buttons[Direction.LEFT], buttons[Direction.RIGHT]);
    }

    public void SetExplicit(Selectable up = null, Selectable down = null, Selectable left = null, Selectable right = null)
    {
        var nav = target.navigation;
        nav.mode = Navigation.Mode.Explicit;

        nav.selectOnUp = up;
        nav.selectOnDown = down;
        nav.selectOnLeft = left;
        nav.selectOnRight = right;

        target.navigation = nav;
    }

    public void ChangeExplicit(Direction direction, Selectable button)
    {
        var nav = target.navigation;
        nav.mode = Navigation.Mode.Explicit;
        switch(direction)
        {
            case Direction.UP:
                nav.selectOnUp = button;
                break;
            case Direction.DOWN:
                nav.selectOnDown = button;
                break;
            case Direction.LEFT:
                nav.selectOnLeft = button;
                break;
            case Direction.RIGHT:
                nav.selectOnRight = button;
                break;
        }

        target.navigation = nav;
    }

    // Interface
    public void OnSelect(BaseEventData eventData)
    {
        OnSelectAction.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselectAction.Invoke(target);
    }
}
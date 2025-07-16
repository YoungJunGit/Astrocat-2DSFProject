using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public TurnTimelineSystem GetTimeLineSystem() => transform.GetComponentInChildren<TurnTimelineSystem>();
}
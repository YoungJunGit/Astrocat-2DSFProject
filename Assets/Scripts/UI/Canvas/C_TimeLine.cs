using UnityEngine;

public class C_TimeLine : MonoBehaviour
{
    public TurnTimelineSystem GetTimeLineSystem() => transform.GetComponentInChildren<TurnTimelineSystem>();
}
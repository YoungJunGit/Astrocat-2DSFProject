using System;
using UnityEngine;

public class RecoveryEffectController : MonoBehaviour
{
    public event Action onStart = delegate { };
    public event Action onEnd = delegate { };

    /// <summary>
    /// Handled by Animation Event
    /// </summary>
    public void StartFactorEffect()
    {
        onStart.Invoke();
    }

    /// <summary>
    /// Handled by Animation Event
    /// </summary>
    public void EndFactorEffect()
    {
        onEnd.Invoke();
    }
}
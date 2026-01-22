using System;
using UnityEngine;

public class QteuiDisposer : IDisposable
{
    QTEUI _qteUI;
    
    public QteuiDisposer(QTEUI qteui, float time, Vector2 position)
    {
        _qteUI = qteui;
        _qteUI.StartQte(time, position);
    }
    
    public void Dispose()
    {
        _qteUI.EndQte();
    }
}
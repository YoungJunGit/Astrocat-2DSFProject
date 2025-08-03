using System;

public class UpdateDispoer : IDisposable
{
    IUpdateObserver _updateObserver;
    
    public UpdateDispoer(IUpdateObserver updateObserver)
    {
        _updateObserver = updateObserver;
        UpdatePublisher.SubscribeObserver(_updateObserver);
    }
    
    public void Dispose()
    {
        UpdatePublisher.DiscribeObserver(_updateObserver);
    }
}
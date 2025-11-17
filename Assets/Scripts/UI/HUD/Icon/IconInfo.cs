using R3;

public class IconInfo
{
    public string IconDirectory;
    private readonly ReactiveProperty<int> _duration;
    public ReadOnlyReactiveProperty<int> Duration => _duration.ToReadOnlyReactiveProperty();
    public IconInfo(int duration)
    {
        _duration = new ReactiveProperty<int>(duration);
    }

    public void Increase() => _duration.Value++;
    public void Decrease() => _duration.Value--;
    public void Set(int duration) => _duration.Value = duration;
    
}
using R3;

public class IconViewModel<T>
{
    private readonly IconModel<T> _iconModel;
    public T IconType => _iconModel.IconType;
    public ReadOnlyReactiveProperty<int> Count => _iconModel.Count.ToReadOnlyReactiveProperty();
    public IconViewModel(T type, int count) 
    { 
        _iconModel = new IconModel<T>(type, count);

        // TODO : Data Binding
    }
    public void Increase() => _iconModel.Count.Value++;
    public void Decrease() => _iconModel.Count.Value--;
    public void Set(int count) => _iconModel.Count.Value = count;

}
using R3;
public class IconModel<T>
{
    public T IconType;
    public readonly ReactiveProperty<int> Count;
    public IconModel(T type, int count)
    {
        IconType = type;
        Count = new ReactiveProperty<int>(count);
    }
}
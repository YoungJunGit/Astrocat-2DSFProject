using R3;
using UnityEngine;

public class BannerViewModel
{
    private readonly BannerModel _bannerModel;

    public ReadOnlyReactiveProperty<int> ReactiveIndex => _bannerModel.reactiveIndex.ToReadOnlyReactiveProperty();
    public ReadOnlyReactiveProperty<int> ReactiveRound => _bannerModel.reactiveRound.ToReadOnlyReactiveProperty();

    public BannerViewModel(UnitStat stat, int index, int round)
    {
        _bannerModel = new BannerModel(stat, index, round);
    }

    public void SetIndex(int index)
    {
        _bannerModel.reactiveIndex.Value = index;
    }

    public void SetRound(int round)
    {
        _bannerModel.reactiveRound.Value = round;
    }

    public int CompareTo(BannerViewModel other)
    {
        if (this._bannerModel.reactiveRound.Value < other._bannerModel.reactiveRound.Value) { return -1; }
        else if (this._bannerModel.reactiveRound.Value > other._bannerModel.reactiveRound.Value) { return 1; }
        else
        {
            return _bannerModel.Stat.CompareTo(other._bannerModel.Stat);
        }
    }
}

using R3;
using UnityEngine;

public class BannerModel
{
    private UnitStat               _stat;

    public UnitStat              Stat => _stat;
    public ReactiveProperty<int> reactiveIndex;
    public ReactiveProperty<int> reactiveRound;

    public BannerModel(UnitStat stat, int index, int round)
    {
        _stat = stat;
        reactiveIndex = new ReactiveProperty<int>(index);
        reactiveRound = new ReactiveProperty<int>(round);
    }
}

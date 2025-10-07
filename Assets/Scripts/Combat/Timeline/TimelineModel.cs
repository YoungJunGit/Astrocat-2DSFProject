using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using Unity.VisualScripting;
using R3;

public class TimelineModel
{
    public ObservableList<EntityBanner> BannerList = null;
    public EntityBanner CurrentTurnBanner = null;
    public ReactiveProperty<int> curRound;
    public int roundDepth;

    public TimelineModel(int roundDepth, int curRound)
    {
        this.BannerList = new ObservableList<EntityBanner>();
        this.curRound   = new ReactiveProperty<int>(curRound);
        this.roundDepth = roundDepth;
    }
}

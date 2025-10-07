using System.Collections.Generic;
using UnityEngine;
using ObservableCollections;
using Unity.VisualScripting;

public class TimelineModel
{
    public ObservableList<EntityBanner> BannerList = null;
    public EntityBanner CurrentTurnBanner = null;
    public int roundDepth;
    public int curRound;

    public TimelineModel(int roundDepth, int curRound)
    {
        BannerList = new ObservableList<EntityBanner>();
        this.roundDepth = roundDepth;
        this.curRound = curRound;
    }
}

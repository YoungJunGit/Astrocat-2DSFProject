using System;
using System.Reflection;
using UnityEngine;
using static EntityBanner;

public class BannerActions
{
    private readonly TimelineSystem owner;
    public BannerActions(TimelineSystem owner) => this.owner = owner;

    public void FaintingButton()
    {
        int nextBanner = owner.timelineUI.GetCurrentTurnBanner().Index - 1;
        EntityBanner faintingTarget = owner.timelineUI.BannerList[nextBanner];
        faintingTarget.SetState(BannerState.FAINT);
        faintingTarget.FaintingEffect();
    }

    public void ExtraButton()
    {
        owner.roundDepth++;

        int index = owner.timelineUI.BannerList.Count;
        var unitStat = owner.timelineUI.GetCurrentTurnBanner().GetStat();

        EntityBanner banner = owner.timelineUI.effect.CreateExtraBanner(unitStat, index, owner.roundDepth);
        owner.timelineUI.BannerList.Add(banner);
        owner.timelineUI.effect.ReorderExtraTurn(owner.timelineUI.BannerList ,index);
        owner.timelineCanvas.SetParent(owner.timelineUI.BannerList);
    }

    public void UpdateAllUnitStacks()
    {
        var units = owner.unitList.GetUnits();

        foreach (BaseUnit unit in units)
        {
            UnitStat stat = unit.GetStat();
            var fields = typeof(UnitStat).GetFields(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(int) &&
                    field.Name.IndexOf("stack", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    int value = (int)field.GetValue(stat);

                    if (field.Name.Equals("forbiddenStack", StringComparison.OrdinalIgnoreCase))
                        field.SetValue(stat, value - 1);
                    else
                        field.SetValue(stat, value + 1);
                }
            }
        }
    }
}
using static CrowdControlManager;

public class CrowdControlFactory
{
    public static ICrowdControl CreateCrowdControl(CrowdControlType crowdControlType)
    {
        switch (crowdControlType)
        {
            case CrowdControlType.Burn:
                return new BurnCC();
            case CrowdControlType.Oppression:
                return new OppressionCC(); 
            case CrowdControlType.Expose:
                return new ExposeCC();
            case CrowdControlType.Flood:
                return new FloodCC();
            case CrowdControlType.Confusion:
                return new ConfusionCC();
        }

        return null;
    }
}
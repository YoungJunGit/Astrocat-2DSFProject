using static CrowdControlManager;

public class CrowdControlFactory
{
    public static ICrowdControl CreateCrowdControl(CrowdControlType crowdControlType)
    {
        switch (crowdControlType)
        {
            case CrowdControlType.Stun:
                return new StunCC();
            case CrowdControlType.Burn:
                return new BurnCC();
            case CrowdControlType.Suppression:
                return new SuppressionCC();
            case CrowdControlType.Exposure:
                return new ExposeCC();
            case CrowdControlType.Overload:
                return new FloodCC();
            case CrowdControlType.Confusion:
                return new ConfusionCC();
        }

        return null;
    }
}
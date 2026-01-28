using DataEnum;

public class TargetFactory
{
    public static ITarget<BaseUnit> CreateTarget(TARGET_TYPE type)
    {
        switch (type)
        {
            case TARGET_TYPE.SINGLE:
            case TARGET_TYPE.ALL:
            case TARGET_TYPE.RANDOM:
            case TARGET_TYPE.SPLASH:
                return new ListTarget<BaseUnit>();
        }

        return null;
    }
}

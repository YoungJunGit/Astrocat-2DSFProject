using DataEnum;
using System;

public class TargetStrategyFactory
{
    public static ITargetStrategy CreateTargetStrategy(TARGET_TYPE type, params Func<BaseUnit, bool>[] filters)
    {
        switch (type)
        {
            case TARGET_TYPE.SINGLE:
                return new SingleTargetStrategy(filters);
            case TARGET_TYPE.ALL:
                return new AllTargetStrategy(filters);
            case TARGET_TYPE.RANDOM:
                return new RandomTargetStratgy(filters);
            case TARGET_TYPE.SPLASH:
                return new SplashTargetStrategy(1, 1, filters);
        }

        return null;
    }
}
using UnityEngine;
using static SingleShotStrategy;

public class FiringStrategyFactory
{
    public static IFiringStrategy GetStrategy(FiringStrategyType type)
    {
        switch (type)
        {
            case FiringStrategyType.SingleShot:
                return new SingleShotStrategy();
            case FiringStrategyType.SpreadShot:
                return new SpreadShotStrategy();
            default:
                Debug.LogWarning("Unrecognized FiringStrategyType. Defaulting to SingleShot.");
                return new SingleShotStrategy();
        }
    }
}

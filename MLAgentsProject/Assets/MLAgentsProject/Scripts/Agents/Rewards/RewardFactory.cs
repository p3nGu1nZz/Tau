using System;

public static class RewardFactory
{
    public static BaseReward<double[]> CreateReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Binary:
                return new BinaryReward();
            case RewardType.Difference:
                return new DifferenceReward();
            // Add other cases for Threshold, Proportional, Custom
            default:
                throw new ArgumentException("Invalid reward type");
        }
    }
}

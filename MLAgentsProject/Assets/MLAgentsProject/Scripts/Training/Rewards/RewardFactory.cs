using System;

public static class RewardFactory
{
    public static BaseReward<double[]> CreateReward(RewardType rewardType, int initialColumnsToUse = 1)
    {
        switch (rewardType)
        {
            case RewardType.Binary:
                return new BinaryReward();
            case RewardType.Difference:
                return new DifferenceReward();
            case RewardType.Incremental:
                return new IncrementalReward(initialColumnsToUse);
            // Add other cases for Threshold, Proportional, Custom
            default:
                throw new ArgumentException("Invalid reward type");
        }
    }
}

public class TauAgent : BaseAgent
{
    protected override void CalculateReward()
    {
        if (RewardCalculator != null && Data.CachedInputVector != null)
        {
            var expectedResponse = Data.TrainingData[Data.CachedInputVector];
            float reward = RewardCalculator.CalculateReward(Data.CachedOutputVector, expectedResponse);
            UpdateWithReward(reward);

            Log.Message($"Training step completed with reward: {reward}");

            AgentTrainer.Instance.isProcessing = false;
        }
    }
}

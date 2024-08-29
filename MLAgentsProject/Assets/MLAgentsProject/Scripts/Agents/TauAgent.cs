public class TauAgent : BaseAgent
{
    protected override void HandleReward()
    {
        Log.Message("Calculating reward...");
        if (RewardCalculator != null && Data.ModelOutput != null && Data.ExpectedOutput != null)
        {
            float reward = RewardCalculator.CalculateReward(Data.ModelOutput, Data.ExpectedOutput);
            SetReward(reward);

            Log.Message($"Training step {StepCount} completed with reward: {reward}");
            AgentTrainer.Instance.isProcessing = false;
        }
    }
}

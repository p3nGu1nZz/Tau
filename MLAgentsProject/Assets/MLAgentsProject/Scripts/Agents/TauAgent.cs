public class TauAgent : BaseAgent
{
    protected override void HandleReward()
    {
        Log.Message("Calculating reward...");
        if (AgentTrainer.Instance.Reward != null && Data.ModelOutput != null && Data.ExpectedOutput != null)
        {
            float reward = AgentTrainer.Instance.Reward.Calculate(Data.ModelOutput, Data.ExpectedOutput);
            SetReward(reward);

            Log.Message($"step {StepCount} reward: {reward} ({GetCumulativeReward()})");
        }
    }
}

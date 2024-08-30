public class TauAgent : BaseAgent
{
    protected override void HandleReward()
    {
        Log.Message("Handle the agent reward.");
        if (AgentTrainer.Instance.Reward != null && Data.ModelOutput != null && Data.ExpectedOutput != null)
        {
            float reward = AgentTrainer.Instance.Reward.Calculate(Data.ModelOutput, Data.ExpectedOutput);
            SetReward(reward);

            Log.Message($"episode={EpisodeCount}, step={StepCount}, reward={reward} ({GetCumulativeReward()})");
        }
    }
}

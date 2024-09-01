using Trainer = AgentDelegator<AgentTrainer, TauAgent>;

public class TauAgent : BaseAgent<AgentTrainer, TauAgent>
{
    protected override void HandleReward()
    {
        // Log.Message("Handle the agent reward.");
        if (Trainer.Instance.Reward != null && Data.ModelOutput != null && Data.ExpectedOutput != null)
        {
            float reward = Trainer.Instance.Reward.Calculate(Data.ModelOutput, Data.ExpectedOutput);
            SetReward(reward);

            // Log.Message($"Step complete! episode={EpisodeCount}, step={StepCount}, reward={reward} ({GetCumulativeReward()})");
            Trainer.Instance.ReportReward(reward);
        }
    }
}

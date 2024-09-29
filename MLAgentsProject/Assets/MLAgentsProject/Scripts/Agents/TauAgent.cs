using Trainer = AgentDelegator<AgentTrainer, TauAgent>;

public class TauAgent : BaseAgent<AgentTrainer, TauAgent>
{
    protected override void HandleReward()
    {
        if (Trainer.Instance.Reward != null && Data.ModelOutput != null && Data.ExpectedOutput != null)
        {
            float reward = Trainer.Instance.Reward.Calculate(Data.ModelOutput, Data.ExpectedOutput);
            SetReward(reward);

            Trainer.Instance.ReportReward(reward);
        }
    }
}

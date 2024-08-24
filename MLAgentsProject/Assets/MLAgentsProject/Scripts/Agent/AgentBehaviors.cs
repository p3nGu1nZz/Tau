using UnityEngine;

public class AgentBehaviors : MonoBehaviour
{
    private WanderingAgent agent;

    public void Initialize(WanderingAgent agent)
    {
        this.agent = agent;
    }

    public void DrainEnergy(float amount)
    {
        agent.stats.currentEnergy = Mathf.Max(0, agent.stats.currentEnergy - amount);
    }

    public void ReplenishEnergy(float amount)
    {
        agent.stats.currentEnergy = Mathf.Min(agent.settings.maxEnergy, agent.stats.currentEnergy + amount);
    }

    public void DrainLife(float amount)
    {
        agent.stats.currentLife = Mathf.Max(0, agent.stats.currentLife - amount);
    }

    public void ReplenishLife(float amount)
    {
        agent.stats.currentLife = Mathf.Min(agent.settings.maxLife, agent.stats.currentLife + amount);
    }

    public void DrainFood(float amount)
    {
        agent.stats.currentFood = Mathf.Max(0, agent.stats.currentFood - amount);
    }

    public void ReplenishFood(float amount)
    {
        agent.stats.currentFood = Mathf.Min(agent.settings.maxFood, agent.stats.currentFood + amount);
    }

    public void KillAgent()
    {
        agent.SetReward(-1f);
        agent.EndEpisode();
        agent.ResetAgent();
    }

    public void FixedUpdate()
    {
        float foodDrainRate = agent.settings.foodDrainRate * Time.fixedDeltaTime;
        if (agent.state.GetCurrentState() == AgentState.Wander.ToString())
        {
            foodDrainRate *= agent.settings.foodDrainMultiplierWhenMoving;
        }
        DrainFood(foodDrainRate);

        if (agent.stats.currentEnergy <= 0)
        {
            DrainLife(agent.settings.lifeLossRateWhenNoEnergy * Time.fixedDeltaTime);
        }

        if (agent.stats.currentFood <= 0)
        {
            DrainLife(agent.settings.lifeLossRateWhenNoFood * Time.fixedDeltaTime);
        }

        if (agent.stats.currentLife <= 0)
        {
            KillAgent();
        }

        if (agent.GetCumulativeReward() <= agent.settings.cumulativeRewardThreshold)
        {
            agent.EndEpisode();
            agent.ResetAgent();
        }
    }
}

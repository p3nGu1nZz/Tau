using UnityEngine;

public class AgentRewards : MonoBehaviour
{
    private WanderingAgent agent;
    private float targetVelocity;
    [HideInInspector] public float facingReward;
    [HideInInspector] public float velocityReward;
    [HideInInspector] public float energyReward;
    [HideInInspector] public float lifeReward;
    [HideInInspector] public float idlePenalty;
    [HideInInspector] public float targetTouchReward;

    public void Initialize(WanderingAgent agent, float targetVelocity)
    {
        this.agent = agent;
        this.targetVelocity = targetVelocity;
    }

    public float CalculateReward()
    {
        float reward = 0f;
        reward += CalculateFacingReward();
        reward += CalculateVelocityReward();
        reward += CalculateEnergyReward();
        reward += CalculateLifeReward();
        reward += CalculateLifeLossEnergyReward();
        reward += CalculateLifeLossFoodReward();
        reward += CalculateIdlePenalty();
        reward += CalculateTargetTouchReward();
        return reward;
    }

    private float CalculateFacingReward()
    {
        Vector3 headingDirection = agent.heading.GetHeadingDirection();
        float movementDirection = Vector3.Dot(agent.rb.linearVelocity.normalized, headingDirection);
        facingReward = Vector3.Dot(agent.transform.forward, headingDirection);

        if (movementDirection < 0)
        {
            facingReward = -Mathf.Abs(facingReward);
        }
        else
        {
            facingReward = Mathf.Abs(facingReward);
        }

        float velocityScale = agent.rb.linearVelocity.magnitude / targetVelocity;
        facingReward *= Mathf.Clamp(velocityScale, 0f, 1f);

        facingReward = Mathf.Clamp(facingReward, -1f, 1f) * 0.375f;
        return facingReward;
    }

    private float CalculateVelocityReward()
    {
        float velocityDifference = Mathf.Abs(agent.rb.linearVelocity.magnitude - targetVelocity) / targetVelocity;
        velocityReward = agent.rb.linearVelocity.magnitude > 0 ? 1f - velocityDifference : 0f;
        velocityReward = Mathf.Clamp(velocityReward, -1f, 1f) * 0.375f;
        return velocityReward;
    }

    private float CalculateEnergyReward()
    {
        float energyRatio = agent.stats.currentEnergy / agent.settings.maxEnergy;
        energyReward = energyRatio >= 0.5f ? (energyRatio - 0.5f) * 0.125f :
                       energyRatio < 0.1f ? (energyRatio - 0.1f) * 0.125f : 0f;
        energyReward = Mathf.Clamp(energyReward, -0.125f, 0.125f);
        return energyReward;
    }

    private float CalculateLifeReward()
    {
        float lifeRatio = agent.stats.currentLife / agent.settings.maxLife;
        lifeReward = lifeRatio >= 0.5f ? (lifeRatio - 0.5f) * 0.125f :
                     lifeRatio < 0.1f ? (lifeRatio - 0.1f) * 0.125f : 0f;
        lifeReward = Mathf.Clamp(lifeReward, -0.125f, 0.125f);
        return lifeReward;
    }

    private float CalculateLifeLossEnergyReward()
    {
        float reward = 0f;
        if (agent.stats.currentEnergy <= 0)
        {
            agent.behaviors.DrainLife(agent.settings.lifeLossRateWhenNoEnergy * Time.fixedDeltaTime);
            reward += agent.settings.lifeLossRewardWhenNoEnergy;
        }
        return reward;
    }

    private float CalculateLifeLossFoodReward()
    {
        float reward = 0f;
        if (agent.stats.currentFood <= 0)
        {
            agent.behaviors.DrainLife(agent.settings.lifeLossRateWhenNoFood * Time.fixedDeltaTime);
            reward += agent.settings.lifeLossRewardWhenNoFood;
        }
        return reward;
    }

    private float CalculateIdlePenalty()
    {
        if (agent.stats.idleTime >= agent.settings.idleDurationThreshold)
        {
            idlePenalty = agent.settings.idlePenalty;
            return idlePenalty;
        }
        idlePenalty = 0f;
        return idlePenalty;
    }

    private float CalculateTargetTouchReward()
    {
        if (agent.stats.isTouchingTarget)
        {
            if (agent.stats.targetTouchCooldownTimer <= 0)
            {
                agent.AddReward(1f); // Immediate reward
                agent.stats.targetTouchCooldownTimer = agent.settings.targetTouchCooldown; // Start cooldown timer
            }
        }

        if (agent.stats.targetTouchCooldownTimer > 0)
        {
            float lerpFactor = Mathf.Pow(agent.stats.targetTouchCooldownTimer / agent.settings.targetTouchCooldown, 2); // Steeper dropoff
            targetTouchReward = Mathf.Lerp(0, 1f, lerpFactor);
            agent.stats.targetTouchCooldownTimer -= Time.deltaTime * 2; // Faster cooldown
        }
        else
        {
            targetTouchReward = 0f;
        }

        return targetTouchReward;
    }

    public void AddFrameReward()
    {
        agent.AddReward(agent.settings.negativeRewardPerFrame);
    }
}

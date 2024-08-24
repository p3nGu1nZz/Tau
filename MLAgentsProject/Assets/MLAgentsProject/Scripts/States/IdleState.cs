using UnityEngine;

public class IdleState : AgentStateBase
{
    private bool stateJustChanged = false;
    private float stateChangeTimer = 0f;

    public IdleState(WanderingAgent agent) : base(agent) { }

    public override void Enter()
    {
        stateChangeTimer = 0f;
        stateJustChanged = true;
        agent.stats.idleTime = 0f;
        agent.rewards.idlePenalty = 0f;
    }

    public override void LateUpdate()
    {
        if (stateJustChanged)
        {
            stateChangeTimer += Time.deltaTime;
            if (stateChangeTimer > agent.settings.stateChangeMaxTime)
            {
                stateJustChanged = false;
                stateChangeTimer = 0f;
            }
        }
        else if (agent.physics.HasMovementInput(agent.moveInputForward, agent.moveInputStrafe, agent.turnInput))
        {
            agent.ChangeState(AgentState.Wander);
        }
        else
        {
            if (agent.stats.idleTime < agent.settings.idleDurationThreshold)
            {
                agent.stats.idleTime += Time.deltaTime;
            }
            agent.behaviors.ReplenishEnergy(agent.settings.energyReplenishRate * Time.deltaTime / agent.settings.energyReplenishScale);
            agent.behaviors.ReplenishLife(agent.settings.lifeReplenishRate * Time.deltaTime / agent.settings.lifeReplenishScale);
        }
    }

    public override void Exit()
    {
        stateJustChanged = true;
        agent.stats.idleTime = 0f;
        agent.rewards.idlePenalty = 0f;
    }
}

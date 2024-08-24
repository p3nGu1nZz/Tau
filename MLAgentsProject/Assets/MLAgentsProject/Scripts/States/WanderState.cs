using UnityEngine;

public class WanderState : AgentStateBase
{
    private float wanderTimer;
    private float wanderDuration;

    public WanderState(WanderingAgent agent) : base(agent) { }

    public override void Enter()
    {
        SetNewWanderDuration();
    }

    public override void FixedUpdate()
    {
        wanderTimer += Time.fixedDeltaTime;

        if (wanderTimer >= wanderDuration)
        {
            SetNewWanderDuration();
            SetNewHeading();
        }

        agent.physics.ClearMinimumVelocity();
        agent.physics.CalculateMovement(agent.moveInputForward, agent.moveInputStrafe);
        agent.physics.CalculateTorque(agent.turnInput);

        float movementAmount = agent.rb.linearVelocity.magnitude;
        float turningAmount = agent.rb.angularVelocity.magnitude;

        agent.behaviors.DrainEnergy((movementAmount * agent.settings.energyDrainRate + turningAmount * agent.settings.energyDrainRate / agent.settings.energyDrainScale) * Time.fixedDeltaTime);

        if ((!agent.physics.HasMovementInput(agent.moveInputForward, agent.moveInputStrafe, agent.turnInput) &&
             agent.physics.IsIdle()) || agent.stats.currentEnergy <= 0)
        {
            agent.ChangeState(AgentState.Idle);
        }
    }

    private void SetNewWanderDuration()
    {
        wanderTimer = 0f;
        wanderDuration = Random.Range(agent.settings.minWanderDuration, agent.settings.maxWanderDuration);
    }

    private void SetNewHeading()
    {
        Vector3 newHeading = agent.heading.GetRandomHeading();
        agent.heading.SetHeadingDirection(newHeading);
    }
}

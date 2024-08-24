using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class WanderingAgent : Agent
{
    [Header("Settings")]
    public WanderingAgentSettings settings;
    public WanderingAgentStats stats;

    [Header("References")]
    public AgentHeadingController heading;
    public AgentPhysics physics;
    public AgentRewards rewards;
    public AgentObservations observations;
    public AgentBehaviors behaviors;
    public AgentInput agentInput;
    public Rigidbody rb;

    [HideInInspector] public float moveInputForward;
    [HideInInspector] public float moveInputStrafe;
    [HideInInspector] public float turnInput;

    [HideInInspector] public FiniteStateMachine state;
    [HideInInspector] public AgentSpawner agentSpawner;

    private int stepCount;

    public override void Initialize()
    {
        physics.Initialize(this, rb, settings.moveForce, settings.turnForce, settings.maxVelocity, settings.maxRotVelocity);
        rewards.Initialize(this, settings.targetVelocity);
        observations.Initialize(this);
        behaviors.Initialize(this);
        agentInput.Initialize(this);
        state = new FiniteStateMachine(this);
        state.Initialize(AgentState.Idle);
        ResetAgent();

        agentSpawner = FindFirstObjectByType<AgentSpawner>();
    }

    public override void OnEpisodeBegin()
    {
        stats.episodeNumber++;
        stepCount = 0;
        stats.idleTime = 0f;
    }

    public void ChangeState(AgentState newState)
    {
        state.ChangeState(newState);
    }

    public void ResetAgent()
    {
        physics.ResetVelocityPosition();
        stats.fullName = NameGenerator.GenerateRandomTwoName();
        heading.SetHeadingDirection(heading.GetRandomHeading());
        stats.currentEnergy = settings.maxEnergy;
        stats.currentLife = settings.maxLife * settings.startingLifePercentage;
        stats.currentFood = settings.maxFood;
        stats.distanceTravelled = 0f;
        stats.ResetTouchingFlags();
    }

    private void Update()
    {
        state.Update();
    }

    private void FixedUpdate()
    {
        state.FixedUpdate();
        rewards.AddFrameReward();
        stepCount++;

        // Apply idle penalty if idle time exceeds threshold
        if (stats.idleTime >= settings.idleDurationThreshold)
        {
            AddReward(settings.idlePenalty);
        }

        if (stepCount >= settings.maxEpisodeLength && settings.maxEpisodeLength != 0)
        {
            EndEpisode();
        }
    }

    private void LateUpdate()
    {
        state.LateUpdate();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        observations.Collect(sensor);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        moveInputForward = actionBuffers.ContinuousActions[0];
        turnInput = actionBuffers.ContinuousActions[1];
        moveInputStrafe = actionBuffers.ContinuousActions[2];

        stats.currentStepReward = rewards.CalculateReward();
        AddReward(stats.currentStepReward);

        // Check if the agent is idle using HasMovementInput
        if (!physics.HasMovementInput(moveInputForward, moveInputStrafe, turnInput))
        {
            stats.idleTime += Time.fixedDeltaTime;
        }
        else
        {
            stats.idleTime = 0f; // Reset idle time if the agent is moving
        }

        if (stepCount >= settings.minEpisodeLength && (IsTouchingPositiveTag() || IsTouchingNegativeTag()))
        {
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        agentInput.HandleHeuristic(actionsOut);
    }

    private bool IsTouchingPositiveTag()
    {
        return stats.isTouchingTarget || stats.isTouchingPickup || stats.isTouchingCheckpoint || stats.isTouchingGoal || stats.isTouchingResource || stats.isTouchingFood;
    }

    private bool IsTouchingNegativeTag()
    {
        return stats.isTouchingObstacle || stats.isTouchingWall || stats.isTouchingEnemy || stats.isTouchingHazard || stats.isTouchingPoison;
    }
}

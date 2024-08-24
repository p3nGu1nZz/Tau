using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "WanderingAgentSettings", menuName = "ScriptableObjects/WanderingAgentSettings", order = 1)]
public class WanderingAgentSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveForce = 10f;
    public float turnForce = 10f;
    public float maxVelocity = 2f;
    public float maxRotVelocity = 2f;
    public float targetVelocity = 2f;

    [Header("Energy Management")]
    public float maxEnergy = 100f;
    public float energyDrainRate = 1f;
    public float energyReplenishRate = 10f;

    [Header("Life Management")]
    public float maxLife = 100f;
    public float lifeLossRateWhenNoEnergy = 1f;
    public float lifeLossRateWhenNoFood = 0.05f;
    public float lifeReplenishRate = 0.5f;
    public float lifeReplenishScale = 10f;
    public float startingLifePercentage = 0.5f;

    [Header("Food Management")]
    public float maxFood = 100f;
    public float foodDrainRate = 0.1f;
    public float foodDrainMultiplierWhenMoving = 12f;

    [Header("Wander Settings")]
    public float minWanderDuration = 20f;
    public float maxWanderDuration = 30f;

    [Header("Reward Settings")]
    public float positiveReward = 1f;
    public float negativeReward = -1f;
    public float negativeRewardPerFrame = -0.001f;
    public float cumulativeRewardThreshold = -1000f;
    public float lifeLossRewardWhenNoEnergy = -0.1f;
    public float lifeLossRewardWhenNoFood = -0.05f;
    public float collisionStayReward = -0.5f;
    public float positiveContactReward = 0.5f;
    public float targetTouchCooldown = 5f;

    [Header("Idle Penalty Settings")]
    public float idlePenalty = -0.25f;
    public float idleDurationThreshold = 10f;

    [Header("Default Tags")]
    public List<string> positiveTags = new List<string> { "Finish", "Pickup", "Checkpoint", "Goal", "Resource", "Food" };
    public List<string> negativeTags = new List<string> { "Obstacle", "Wall", "Enemy", "Hazard", "Poison" };

    [Header("Episode Settings")]
    public int minEpisodeLength = 100;
    public int maxEpisodeLength = 10000;

    [Header("Advanced Settings")]
    [Tooltip("These settings are usually not changed on a per-agent basis.")]
    public float maxRotationAngle = 89f;
    public float stateChangeMaxTime = 0.33f;
    public float energyDrainScale = 0.1f;
    public float energyReplenishScale = 10f;
    public float touchingFlagDelay = 0.5f;
}

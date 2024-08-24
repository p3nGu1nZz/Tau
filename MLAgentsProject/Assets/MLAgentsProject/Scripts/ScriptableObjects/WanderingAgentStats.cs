using UnityEngine;

[CreateAssetMenu(fileName = "WanderingAgentStats", menuName = "ScriptableObjects/WanderingAgentStats", order = 2)]
public class WanderingAgentStats : ScriptableObject
{
    public string fullName;
    public int episodeNumber = 0;
    public float currentStepReward = 0f;
    public float currentEnergy;
    public float currentLife;
    public float currentFood;
    public float distanceTravelled;
    public float idleTime;
    public float targetTouchCooldownTimer;

    // Add boolean properties for each tag
    public bool isTouchingGround;
    public bool isTouchingTarget;
    public bool isTouchingObstacle;
    public bool isTouchingWall;
    public bool isTouchingPickup;
    public bool isTouchingEnemy;
    public bool isTouchingFriend;
    public bool isTouchingCheckpoint;
    public bool isTouchingHazard;
    public bool isTouchingGoal;
    public bool isTouchingResource;
    public bool isTouchingAgent;
    public bool isTouchingFood;
    public bool isTouchingPlant;
    public bool isTouchingPoison;

    public void ResetTouchingFlags()
    {
        isTouchingGround = true;
        isTouchingTarget = false;
        isTouchingObstacle = false;
        isTouchingWall = false;
        isTouchingPickup = false;
        isTouchingEnemy = false;
        isTouchingFriend = false;
        isTouchingCheckpoint = false;
        isTouchingHazard = false;
        isTouchingGoal = false;
        isTouchingResource = false;
        isTouchingAgent = false;
        isTouchingFood = false;
        isTouchingPlant = false;
        isTouchingPoison = false;
    }

    public void UpdateTouchingFlags(string tag, bool isTouching)
    {
        switch (tag)
        {
            case "Target":
                isTouchingTarget = isTouching;
                break;
            case "Obstacle":
                isTouchingObstacle = isTouching;
                break;
            case "Wall":
                isTouchingWall = isTouching;
                break;
            case "Pickup":
                isTouchingPickup = isTouching;
                break;
            case "Enemy":
                isTouchingEnemy = isTouching;
                break;
            case "Friend":
                isTouchingFriend = isTouching;
                break;
            case "Checkpoint":
                isTouchingCheckpoint = isTouching;
                break;
            case "Hazard":
                isTouchingHazard = isTouching;
                break;
            case "Goal":
                isTouchingGoal = isTouching;
                break;
            case "Resource":
                isTouchingResource = isTouching;
                break;
            case "Agent":
                isTouchingAgent = isTouching;
                break;
            case "Food":
                isTouchingFood = isTouching;
                break;
            case "Plant":
                isTouchingPlant = isTouching;
                break;
            case "Poison":
                isTouchingPoison = isTouching;
                break;
        }
    }
}

using UnityEngine;
using TMPro;

public class RewardDisplay : MonoBehaviour
{
    [HideInInspector] public WanderingAgent agent;
    [HideInInspector] public AgentHeadingController heading;
    [HideInInspector] public TextMeshProUGUI rewardText;

    void Start()
    {
        rewardText = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (agent == null) return;

        int lineWidth = 60;

        string displayText =
            $"{FormatLine($"{agent.stats.fullName} : name", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.episodeNumber:D6} : episode", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.StepCount:D6} : step", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.GetCumulativeReward():F4} : reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.currentStepReward:F4} : step reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rb.linearVelocity.magnitude:F4} : velocity", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rb.angularVelocity.y:F4} : rot velocity", ref lineWidth)}\n" +
            $"{FormatLine($"{heading.GetHeadingDirection():F4} : heading", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.moveInputForward:F4} : move forward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.moveInputStrafe:F4} : strafe", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.turnInput:F4} : turn", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.facingReward:F4} : facing reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.velocityReward:F4} : velocity reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.energyReward:F4} : energy reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.lifeReward:F4} : life reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.currentEnergy:F3} / {agent.settings.maxEnergy:F3} : energy", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.currentLife:F3} / {agent.settings.maxLife:F3} : life", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.currentFood:F3} / {agent.settings.maxFood:F3} : food", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.distanceTravelled:F3} : distance travelled", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.idlePenalty:F4} : idle penalty", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.idleTime:F3} : idle timer", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.rewards.targetTouchReward:F4} : target touch reward", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.state.GetCurrentState()} : state", ref lineWidth)}\n" +
            $"{FormatLine("-------------------------------------------", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingTarget} : touching target", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingObstacle} : touching obstacle", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingWall} : touching wall", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingPickup} : touching pickup", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingEnemy} : touching enemy", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingFriend} : touching friend", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingCheckpoint} : touching checkpoint", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingHazard} : touching hazard", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingGoal} : touching goal", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingResource} : touching resource", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingGround} : touching ground", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingAgent} : touching agent", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingFood} : touching food", ref lineWidth)}\n" +
            $"{FormatLine($"{agent.stats.isTouchingPlant} : touching plant", ref lineWidth)}\n";
        rewardText.text = displayText;
    }

    public void SetAgent(WanderingAgent newAgent)
    {
        agent = newAgent;
    }

    public void SetHeadingController(AgentHeadingController newHeading)
    {
        heading = newHeading;
    }

    private string FormatLine(string content, ref int width)
    {
        return $"{content.PadLeft(width)} |";
    }
}

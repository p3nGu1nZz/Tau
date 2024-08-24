using Unity.MLAgents.Sensors;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class AgentObservations : MonoBehaviour
{
    private WanderingAgent agent;

    public void Initialize(WanderingAgent agent)
    {
        this.agent = agent;
    }

    public void Collect(VectorSensor sensor)
    {
        sensor.AddObservation(agent.transform.localPosition);
        sensor.AddObservation(agent.transform.localRotation);
        sensor.AddObservation(agent.rb.linearVelocity);
        sensor.AddObservation(agent.rb.angularVelocity);
        sensor.AddObservation(agent.transform.forward);
        sensor.AddObservation(agent.transform.right);
        sensor.AddObservation(agent.stats.currentEnergy / agent.settings.maxEnergy);
        sensor.AddObservation(agent.stats.currentLife / agent.settings.maxLife);
        sensor.AddObservation(agent.stats.currentFood / agent.settings.maxFood);
        sensor.AddObservation(agent.stats.distanceTravelled);
        sensor.AddObservation(agent.stats.idleTime);
        sensor.AddObservation(agent.state.GetCurrentStateAsFloat());

        sensor.AddObservation(agent.stats.isTouchingTarget);
        sensor.AddObservation(agent.stats.isTouchingObstacle);
        sensor.AddObservation(agent.stats.isTouchingWall);
        sensor.AddObservation(agent.stats.isTouchingPickup);
        sensor.AddObservation(agent.stats.isTouchingEnemy);
        sensor.AddObservation(agent.stats.isTouchingFriend);
        sensor.AddObservation(agent.stats.isTouchingCheckpoint);
        sensor.AddObservation(agent.stats.isTouchingHazard);
        sensor.AddObservation(agent.stats.isTouchingGoal);
        sensor.AddObservation(agent.stats.isTouchingResource);
        sensor.AddObservation(agent.stats.isTouchingGround);
        sensor.AddObservation(agent.stats.isTouchingAgent);
        sensor.AddObservation(agent.stats.isTouchingFood);
        sensor.AddObservation(agent.stats.isTouchingPlant);
        sensor.AddObservation(agent.stats.isTouchingPoison);
    }
}

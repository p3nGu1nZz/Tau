using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public interface IBaseAgent
{
    AgentData Data { get; }
    BaseReward<double[]> RewardCalculator { get; set; }

    void Setup();
    void Initialize();
    void OnEpisodeBegin();
    void CollectObservations(VectorSensor sensor);
    void OnActionReceived(ActionBuffers actions);
    void Heuristic(in ActionBuffers actionsOut);
    void UpdateWithReward(float reward);
    void ResetAgent();

    // Additional methods and properties from Agent
    void EndEpisode();
    GameObject gameObject { get; }
    T GetComponent<T>();
    float GetCumulativeReward();
    void RequestDecision();
    int StepCount { get; }
}

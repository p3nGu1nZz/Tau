using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;

public interface IBaseAgent
{
    AgentData Data { get; }

    void Setup();
    void Initialize();
    void OnEpisodeBegin();
    void CollectObservations(VectorSensor sensor);
    void OnActionReceived(ActionBuffers actions);
    void Heuristic(in ActionBuffers actions);
    void ProcessActions(ActionSegment<float> actions);
    void ResetAgent();
    void EndEpisode();
    GameObject gameObject { get; }
    T GetComponent<T>();
    float GetCumulativeReward();
    void RequestDecision();
    int StepCount { get; }
}

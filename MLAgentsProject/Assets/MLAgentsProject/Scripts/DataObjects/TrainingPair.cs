using UnityEngine;

public struct TrainingPair<TDelegator, TAgent>
    where TDelegator : AgentDelegator<TDelegator, TAgent>
    where TAgent : BaseAgent<TDelegator, TAgent>
{
    public GameObject AgentDelegator { get; set; }
    public GameObject BaseAgent { get; set; }

    public TrainingPair(GameObject agentDelegator, GameObject baseAgent)
    {
        AgentDelegator = agentDelegator;
        BaseAgent = baseAgent;

        // Set the trainer reference on the base agent
        var agentDelegatorComponent = agentDelegator.GetComponent<TDelegator>();
        var baseAgentComponent = baseAgent.GetComponent<TAgent>();

        baseAgentComponent.Delegator = agentDelegatorComponent;
        agentDelegatorComponent.Agent = baseAgentComponent;
    }
}

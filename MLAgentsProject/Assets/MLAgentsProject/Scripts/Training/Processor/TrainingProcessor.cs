using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainingProcessor<TDelegator, TAgent> : BaseProcessor<TDelegator, TAgent>
    where TDelegator : AgentDelegator<TDelegator, TAgent>
    where TAgent : BaseAgent<TDelegator, TAgent>
{
    public TrainingProcessor(int numAgents, GameObject agentTrainerPrefab, GameObject tauAgentPrefab, int columns)
        : base(numAgents, columns)
    {
        // Setup the training pairs in the constructor
        for (int i = 0; i < numAgents; i++)
        {
            GameObject pairParent = new GameObject($"TrainingPair_{i}");
            GameObject agentDelegatorInstance = UnityEngine.Object.Instantiate(agentTrainerPrefab, pairParent.transform);
            GameObject baseAgentInstance = UnityEngine.Object.Instantiate(tauAgentPrefab, pairParent.transform);
            AddTrainingPair(agentDelegatorInstance, baseAgentInstance);
        }
    }

    public void StartTraining(string fileName)
    {
        AgentTrainer.TrainingFileName = fileName;
        foreach (var pair in trainingPairs)
        {
            StartTrainingTask(pair, cancellationTokenSource.Token);
        }
    }
}

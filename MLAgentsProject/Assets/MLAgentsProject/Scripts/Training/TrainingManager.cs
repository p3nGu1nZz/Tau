using System;
using UnityEngine;

public class TrainingManager
{
    private static TrainingManager _instance;
    public static TrainingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new TrainingManager();
            }
            return _instance;
        }
    }

    private TrainingProcessor<AgentTrainer, TauAgent> processor;

    private TrainingManager() { }

    public void ExecuteTraining(string agentType, string fileName, int numAgents, int columns)
    {
        try
        {
            Log.Message("Checking if database is loaded.");
            if (!Database.Instance.IsLoaded())
            {
                throw new InvalidOperationException("No database loaded. Please load a database or training data.");
            }

            Log.Message($"Starting training for {TrainingType.Agent} with agent type {agentType} using file {fileName}, {numAgents} agents, and {columns} columns");

            string agentPrefabName = $"{agentType}{TrainingType.Agent}";
            string trainerPrefabName = $"{TrainingType.Agent}Trainer";

            Log.Message($"Loading prefabs: {agentPrefabName} and {trainerPrefabName}");
            GameObject agentTrainerPrefab = Resources.Load<GameObject>(trainerPrefabName);
            GameObject tauAgentPrefab = Resources.Load<GameObject>(agentPrefabName);

            if (agentTrainerPrefab == null || tauAgentPrefab == null)
            {
                throw new Exception($"Failed to load prefabs: '{trainerPrefabName}' or '{agentPrefabName}'");
            }

            Log.Message("Removing existing instances of prefabs.");
            GameUtilities.RemoveExistingInstances(agentPrefabName);
            GameUtilities.RemoveExistingInstances(trainerPrefabName);

            Log.Message("Instantiating prefabs.");
            processor = new TrainingProcessor<AgentTrainer, TauAgent>(numAgents, agentTrainerPrefab, tauAgentPrefab, columns);

            Log.Message($"Successfully instantiated {numAgents} pairs of {agentPrefabName} and {trainerPrefabName} prefabs.");

            // Start the training
            processor.StartTraining(fileName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }

    public void CancelTraining()
    {
        processor?.CancelTraining();
    }
}

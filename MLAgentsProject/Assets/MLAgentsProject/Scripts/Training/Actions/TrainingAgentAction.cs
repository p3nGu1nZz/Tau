using System;
using UnityEngine;

public static class TrainingAgentAction
{
    public static void Execute(string trainType, string agentType)
    {
        try
        {
            if (!Database.Instance.IsLoaded())
            {
                throw new InvalidOperationException("No database loaded. Please load a database or training data.");
            }

            Log.Message($"Starting training for {trainType} with agent type {agentType}.");

            string agentPrefabName = $"{agentType}{trainType}";
            string trainerPrefabName = $"{trainType}Trainer";

            GameObject agentTrainerPrefab = Resources.Load<GameObject>(trainerPrefabName);
            GameObject tauAgentPrefab = Resources.Load<GameObject>(agentPrefabName);

            if (agentTrainerPrefab == null || tauAgentPrefab == null)
            {
                throw new Exception($"Failed to load prefabs: '{trainerPrefabName}' or '{agentPrefabName}'");
            }

            GameUtilities.RemoveExistingInstances(agentPrefabName);
            GameUtilities.RemoveExistingInstances(trainerPrefabName);

            GameObject agentTrainer = UnityEngine.Object.Instantiate(agentTrainerPrefab);
            GameObject tauAgent = UnityEngine.Object.Instantiate(tauAgentPrefab);
            Log.Message($"Successfully instantiated {agentPrefabName} and {trainerPrefabName} prefabs.");
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }
}

using CommandTerminal;
using System;
using System.Collections.Generic;
using UnityEngine;

using AgentTrainerDelegator = AgentDelegator<AgentTrainer, TauAgent>;

public static class TrainingAgentAction
{
    private static TrainingProcessor<AgentTrainer, TauAgent> processor;

    public static void Execute(CommandArg[] args)
    {
        try
        {
            string agentType = StringUtilities.CapitalizeFirstLetter(args[1].String.ToLower());
            string fileName = args[2].String.ToLower();

            var argValues = CommandUtilities.ParseArgs(args, "num-agents");
            int numAgents = argValues.ContainsKey("num-agents") && int.TryParse(argValues["num-agents"], out int parsedNumAgents) ? parsedNumAgents : 1;

            Log.Message("Checking if database is loaded.");
            if (!Database.Instance.IsLoaded())
            {
                throw new InvalidOperationException("No database loaded. Please load a database or training data.");
            }

            Log.Message($"Starting training for {TrainingType.Agent} with agent type {agentType} using file {fileName} and {numAgents} agents");

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
            processor = new TrainingProcessor<AgentTrainer, TauAgent>(numAgents, agentTrainerPrefab, tauAgentPrefab);

            Log.Message($"Successfully instantiated {numAgents} pairs of {agentPrefabName} and {trainerPrefabName} prefabs.");

            // Start the training
            processor.StartTraining(fileName);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }

    public static void CancelTraining()
    {
        processor?.CancelTraining();
    }
}

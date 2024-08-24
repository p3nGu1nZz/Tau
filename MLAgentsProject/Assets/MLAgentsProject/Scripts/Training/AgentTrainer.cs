using UnityEngine;
using System.Collections.Generic;

public class AgentTrainer : MonoBehaviour
{
    private TauAgent tauAgent;
    private Dictionary<string, Embedding> trainingData;
    private Dictionary<string, Embedding> evaluationData;

    void Start()
    {
        InitializeTraining();
    }

    void Update() { }

    private void InitializeTraining()
    {
        Log.Message("Initializing training loop.");

        // Load training and evaluation data from the database
        trainingData = Database.Instance.GetTable(TableNames.TrainingData);
        evaluationData = Database.Instance.GetTable(TableNames.EvaluationData);

        if (trainingData == null || evaluationData == null)
        {
            Log.Error("Failed to load training or evaluation data from the database.");
            return;
        }

        Log.Message($"Loaded {trainingData.Count} training data entries.");
        Log.Message($"Loaded {evaluationData.Count} evaluation data entries.");

        tauAgent = FindFirstObjectByType<TauAgent>();
        if (tauAgent != null)
        {
            tauAgent.Setup();
            TrainingUtilities.EnableTauAgent(tauAgent.gameObject);
        }
        else
        {
            Log.Error("TauAgent not found in the scene.");
        }
    }
}

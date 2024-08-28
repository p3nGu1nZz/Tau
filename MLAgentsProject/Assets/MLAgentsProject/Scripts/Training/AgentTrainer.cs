using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AgentTrainer : MonoBehaviour
{
    private TauAgent tauAgent;
    private Dictionary<string, Embedding> trainingData;
    private Dictionary<string, Embedding> evaluationData;
    private BaseReward<double[]> rewardCalculator;

    void Start()
    {
        InitializeTraining();
    }

    void Update()
    {
        if (tauAgent != null)
        {
            TrainAgent();
        }
    }

    private void InitializeTraining()
    {
        Log.Message("Initializing training loop.");
        LoadData();
        SetupTauAgent();
        rewardCalculator = RewardFactory.CreateReward(RewardType.Binary);
    }

    private void LoadData()
    {
        trainingData = Database.Instance.GetTable(TableNames.TrainingData);
        evaluationData = Database.Instance.GetTable(TableNames.EvaluationData);

        if (trainingData == null || evaluationData == null)
        {
            Log.Error("Failed to load training or evaluation data from the database.");
            return;
        }

        Log.Message($"Loaded {trainingData.Count} training data entries.");
        Log.Message($"Loaded {evaluationData.Count} evaluation data entries.");
    }

    private void SetupTauAgent()
    {
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

    private void TrainAgent()
    {
        Embedding currentEmbedding = GetRandomTrainingEmbedding();
        if (currentEmbedding == null)
        {
            Log.Error("Failed to get a random training embedding.");
            return;
        }

        double[] embeddingArray = currentEmbedding.Vector.ToArray();
        tauAgent.SetObservations(embeddingArray);
        tauAgent.RequestDecision();

        double[] outputEmbedding = tauAgent.GetOutputEmbedding();
        double reward = rewardCalculator.CalculateReward(outputEmbedding, embeddingArray); // Use the reward calculator
        tauAgent.UpdateWithReward(reward);

        if (ShouldTerminateTraining())
        {
            Log.Message("Training terminated.");
            enabled = false;
        }
    }

    private Embedding GetRandomTrainingEmbedding()
    {
        if (trainingData == null || trainingData.Count == 0)
        {
            Log.Error("Training data is empty or not initialized.");
            return null;
        }

        List<string> keys = trainingData.Keys.ToList();
        string randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
        return trainingData[randomKey];
    }

    private bool ShouldTerminateTraining()
    {
        return false;
    }
}

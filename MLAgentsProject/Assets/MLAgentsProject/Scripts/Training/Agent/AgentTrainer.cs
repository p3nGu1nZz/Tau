using System;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public int maxStepsPerEpisode = 10000;
    public float rewardThreshold = -1000f;

    protected override void Initialize()
    {
        if (agent == null)
        {
            agent = FindFirstObjectByType<TauAgent>();
        }
        SetupAgent();

        if (string.IsNullOrEmpty(data.TrainingFileName))
        {
            throw new ArgumentException("Training file name is not set or is empty.");
        }

        LoadTrainingData(data.TrainingFileName);
    }

    protected override void Process()
    {
        if (!isProcessing)
        {
            StartTrainingEpisode();
        }
    }

    protected override void SetupAgent()
    {
        agent = FindFirstObjectByType<TauAgent>();
        if (agent == null)
        {
            Log.Error("TauAgent not found in the scene.");
            return;
        }

        agent.Setup();
        rewardCalculator = RewardFactory.CreateReward(RewardType.Binary);
        agent.RewardCalculator = rewardCalculator;
        agent.gameObject.SetActive(true);

        var behaviorParameters = agent.GetComponent<BehaviorParameters>();
        if (behaviorParameters != null)
        {
            behaviorParameters.enabled = true;
        }
    }

    void StartTrainingEpisode()
    {
        isProcessing = true;
        SetupTrainingLoop();
    }

    void FixedUpdate()
    {
        if (isProcessing)
        {
            if (agent.StepCount >= maxStepsPerEpisode || agent.GetCumulativeReward() <= rewardThreshold)
            {
                EndTrainingEpisode();
            }
        }
    }

    void EndTrainingEpisode()
    {
        isProcessing = false;
        agent.EndEpisode();
        Process();
    }

    void SetupTrainingLoop()
    {
        if (data.TrainingDataList != null && data.TrainingDataList.Count > 0)
        {
            var randomTrainingData = GetRandomTrainingData();
            agent.Data.CachedInputVector = randomTrainingData.InputEmbedding;
            agent.Data.CachedOutputVector = randomTrainingData.OutputEmbedding;
            UpdateObservations();
            agent.RequestDecision();
        }
    }

    void UpdateObservations()
    {
        agent.Data.InputVector = AgentUtilities.ConvertToFloatArray(agent.Data.CachedInputVector);
    }

    void LoadTrainingData(string fileName)
    {
        try
        {
            data.TrainingDataList = TrainingDataFactory.CreateTrainingDataList(fileName);
            if (data.TrainingDataList == null || data.TrainingDataList.Count == 0)
            {
                throw new Exception("Training data list is empty or null.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load training data from file '{fileName}': {ex.Message}");
        }
    }

    EmbeddingPair GetRandomTrainingData()
    {
        int randomIndex = UnityEngine.Random.Range(0, data.TrainingDataList.Count);
        return data.TrainingDataList[randomIndex];
    }
}

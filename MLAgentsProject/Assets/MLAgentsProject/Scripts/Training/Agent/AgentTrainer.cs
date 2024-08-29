using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public int maxStepsPerEpisode = 10000;
    public float rewardThreshold = -1000f;
    public float waitTimeInSeconds = 1.0f;
    private bool isCoroutineRunning = false;

    protected override void Initialize()
    {
        Log.Message("Initializing AgentTrainer.");
        SetupAgent();

        if (string.IsNullOrEmpty(data.TrainingFileName))
        {
            throw new ArgumentException("Training file name is not set or is empty.");
        }

        Log.Message($"Loading training data from file: {data.TrainingFileName}");
        LoadTrainingData(data.TrainingFileName);
    }

    protected override void SetupAgent()
    {
        Log.Message("Setting up TauAgent.");
        agent = FindFirstObjectByType<TauAgent>();
        if (agent == null)
        {
            throw new ArgumentException("TauAgent not found in the scene.");
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

    void FixedUpdate()
    {
        if (isProcessing)
        {
            if (agent.StepCount >= maxStepsPerEpisode || agent.GetCumulativeReward() <= rewardThreshold)
            {
                Log.Message("Ending training episode.");
                EndTrainingEpisode();
            }
        }
        else if (!isCoroutineRunning)
        {
            StartCoroutine(TrainingUpdateCoroutine());
        }
    }

    void EndTrainingEpisode()
    {
        Log.Message("Ending training episode.");
        isProcessing = false;
        agent.EndEpisode();
    }

    IEnumerator TrainingUpdateCoroutine()
    {
        isCoroutineRunning = true;
        Log.Message("Starting training update coroutine.");
        yield return new WaitForSeconds(waitTimeInSeconds);

        TrainingUpdate();
        isCoroutineRunning = false;
    }

    void TrainingUpdate()
    {
        Log.Message("Updating training loop.");
        var randomTrainingData = GetRandomTrainingData();
        agent.Data.ModelInput = randomTrainingData.InputEmbedding;
        agent.Data.ExpectedOutput = randomTrainingData.OutputEmbedding;
        UpdateObservations();
        agent.RequestDecision();
    }

    void UpdateObservations()
    {
        Log.Message("Updating observations.");
        agent.Data.Observations = AgentUtilities.ConvertToFloatArray(agent.Data.ModelInput);
    }

    void LoadTrainingData(string fileName)
    {
        try
        {
            Log.Message($"Loading training data from file: {fileName}");
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
        Log.Message("Selecting random training data.");
        int randomIndex = UnityEngine.Random.Range(0, data.TrainingDataList.Count);
        return data.TrainingDataList[randomIndex];
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public int MaxStepsPerEpisode = 10000;
    public float RewardThreshold = -1000f;
    public float WaitTimeInSeconds = 1.0f;
    private bool _IsTrainingRunning = false;
    public string TrainingFileName { get; set; }
    public List<EmbeddingPair> TrainingData { get; set; }

    protected override void Initialize()
    {
        Log.Message("Initializing AgentTrainer.");
        Setup();

        if (string.IsNullOrEmpty(TrainingFileName))
        {
            throw new ArgumentException("Training file name is not set or is empty.");
        }

        Log.Message($"Loading training data from file: {TrainingFileName}");
        Load(TrainingFileName);
    }

    protected override void Setup()
    {
        Log.Message("Setting up TauAgent.");
        Agent = FindFirstObjectByType<TauAgent>();
        if (Agent == null)
        {
            throw new ArgumentException("TauAgent not found in the scene.");
        }

        Agent.Setup();
        Reward = RewardFactory.CreateReward(RewardType.Binary);
        Agent.gameObject.SetActive(true);

        var behaviorParameters = Agent.GetComponent<BehaviorParameters>();
        if (behaviorParameters != null)
        {
            behaviorParameters.enabled = true;
        }
    }

    void FixedUpdate()
    {
        if (IsProcessing)
        {
            if (Agent.StepCount >= MaxStepsPerEpisode || Agent.GetCumulativeReward() <= RewardThreshold)
            {
                Log.Message("Ending training episode.");
                EndTrainingEpisode();
            }
        }
        else if (!_IsTrainingRunning)
        {
            StartCoroutine(TrainingUpdate());
        }
    }

    void EndTrainingEpisode()
    {
        Log.Message("Ending training episode.");
        IsProcessing = false;
        Agent.EndEpisode();
    }

    IEnumerator TrainingUpdate()
    {
        _IsTrainingRunning = true;
        Log.Message("Starting training update coroutine.");
        yield return new WaitForSeconds(WaitTimeInSeconds);

        RequestTraining();
        _IsTrainingRunning = false;
    }

    void RequestTraining()
    {
        Log.Message("Request training.");
        EmbeddingPair trainingData = GetRandomTrainingData();
        Agent.Data.ModelInput = trainingData.InputEmbedding;
        Agent.Data.ExpectedOutput = trainingData.OutputEmbedding;
        Agent.Data.Observations = AgentUtilities.ConvertToFloatArray(Agent.Data.ModelInput);
        Agent.RequestDecision();
    }

    void Load(string fileName)
    {
        try
        {
            Log.Message($"Loading training data from file: {fileName}");
            TrainingData = TrainingDataFactory.CreateTrainingData(fileName);
            if (TrainingData == null || TrainingData.Count == 0)
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
        int index = UnityEngine.Random.Range(0, TrainingData.Count);
        return TrainingData[index];
    }
}

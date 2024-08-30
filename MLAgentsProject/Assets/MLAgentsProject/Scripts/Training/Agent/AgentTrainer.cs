using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public int MaxStepsPerEpisode = 10000;
    public float RewardThreshold = -100f;
    public float WaitTimeInSeconds = 0f;
    private bool _IsTraining = false;
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
        Log.Message($"Setting up AgentTrainer. maxSteps={MaxStepsPerEpisode}, rewardThreshold={RewardThreshold}");
        Agent = FindFirstObjectByType<TauAgent>();
        if (Agent == null)
        {
            throw new ArgumentException("TauAgent not found in the scene.");
        }

        Agent.Setup();
        Reward = RewardFactory.CreateReward(RewardType.Binary);
        Agent.gameObject.SetActive(true);

        ComponentUtilities.EnableAllComponents(Agent.gameObject);
    }

    void FixedUpdate()
    {
        if (IsProcessing)
        {
            if (Agent.StepCount >= MaxStepsPerEpisode || Agent.GetCumulativeReward() <= RewardThreshold)
            {
                EndTrainingEpisode();
            }
        }
        else if (!_IsTraining)
        {
            StartCoroutine(TrainingStep());
        }
    }

    void EndTrainingEpisode()
    {
        Log.Message($"Ending training episode {Agent.EpisodeCount}");
        IsProcessing = false;
        Agent.EndEpisode();
    }

    IEnumerator TrainingStep()
    {
        _IsTraining = true;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        yield return new WaitForSeconds(WaitTimeInSeconds);

        RequestTraining();

        stopwatch.Stop();
        Log.Message($"Training step took {stopwatch.ElapsedMilliseconds} ms");
        _IsTraining = false;
    }

    void RequestTraining()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Log.Message("Request training started.");

        EmbeddingPair trainingData = GetRandomTrainingData();
        Log.Message($"training_data: InputEmbedding={StringUtilities.TruncateVectorString(StringUtilities.ConvertVectorToString(trainingData.InputEmbedding))}");
        Log.Message($"training_data: OutputEmbedding={StringUtilities.TruncateVectorString(StringUtilities.ConvertVectorToString(trainingData.OutputEmbedding))}");

        stopwatch.Stop();
        Log.Message($"Data preparation took {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();

        Agent.Data.ModelInput = trainingData.InputEmbedding;
        Agent.Data.ExpectedOutput = trainingData.OutputEmbedding;
        Agent.Data.Observations = AgentUtilities.ConvertToFloatArray(Agent.Data.ModelInput);
        Log.Message(StringUtilities.TruncateVectorString($"model_input: Observations={StringUtilities.ConvertVectorToString(Agent.Data.Observations)}"));

        stopwatch.Stop();
        Log.Message($"Data assignment took {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Restart();

        Log.Message("Requesting decision from agent.");
        Agent.RequestDecision();

        stopwatch.Stop();
        Log.Message($"Agent decision request took {stopwatch.ElapsedMilliseconds} ms");
    }

    void Load(string fileName)
    {
        try
        {
            Log.Message($"Loading training data from file: '{fileName}'.");
            TrainingData = TrainingDataFactory.CreateTrainingData(fileName);
            if (TrainingData == null || TrainingData.Count == 0)
            {
                throw new Exception("Training data list is empty or null.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Failed to load training data from file '{fileName}': '{ex.Message}'");
        }
    }

    EmbeddingPair GetRandomTrainingData()
    {
        int index = UnityEngine.Random.Range(0, TrainingData.Count);
        Log.Message($"Selecting random training data. index={index}");
        return TrainingData[index];
    }
}

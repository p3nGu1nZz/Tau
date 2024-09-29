using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public static int MaxStepsPerEpisode = 10000;
    public static float RewardThreshold = -100f;
    public static float WaitTimeInSeconds = 0f;
    public static string TrainingFileName { get; set; }
    public static List<EmbeddingPair> TrainingData { get; set; }
    public static int Columns { get; set; }

    private List<float> _Rewards = new();
    private int _LogCounter = 0;
    private int _LogInterval = 100;
    private int _TotalSteps = 0;
    private Stopwatch _Stopwatch = new ();
    private EmbeddingPair trainingData;

    public override void Initialize()
    {
        Log.Message("Initializing AgentTrainer.");
        Setup();

        if (string.IsNullOrEmpty(TrainingFileName))
        {
            throw new ArgumentException("Training file name is not set or is empty.");
        }

        Log.Message($"Loading training data from file: {TrainingFileName}");
        LoadTrainingData(TrainingFileName);
        _Stopwatch.Start();
    }

    public void SetColumns(int columns)
    {
        Log.Message("Setting columns for AgentTrainer.");
        Columns = columns;
    }

    protected override void Setup()
    {
        Log.Message($"Setting up AgentTrainer. maxSteps={MaxStepsPerEpisode}, rewardThreshold={RewardThreshold}, columns={Columns}");

        if (Agent == null)
        {
            throw new ArgumentException("TauAgent not found in the scene.");
        }

        Agent.Setup();
        Reward = RewardFactory.CreateReward(RewardType.Incremental, Columns);
        Agent.gameObject.SetActive(true);

        ComponentUtilities.EnableAllComponents(Agent.gameObject);
    }

    void FixedUpdate()
    {
        if (Agent.StepCount >= MaxStepsPerEpisode || Agent.GetCumulativeReward() <= RewardThreshold)
        {
            EndTrainingEpisode();
        }
        else if (Agent.Data.ModelOutput == null)
        {
            StartCoroutine(TrainingStep());
        }

        UpdateReporting();
    }

    void EndTrainingEpisode()
    {
        IsProcessing = false;
        Agent.EndEpisode();
    }

    IEnumerator TrainingStep()
    {
        IsProcessing = true;
        yield return new WaitForSeconds(WaitTimeInSeconds);

        RequestTraining();
        IsProcessing = false;
    }

    void RequestTraining()
    {
        try
        {
            if(trainingData == null)
            trainingData = GetRandomTrainingData();

            Agent.Data.ModelInput = trainingData.InputEmbedding;
            Agent.Data.ExpectedOutput = trainingData.OutputEmbedding;
            Agent.Data.Observations = AgentUtilities.ConvertToFloatArray(Agent.Data.ModelInput);

            Agent.RequestDecision();
        }
        catch (Exception ex)
        {
            Log.Error($"Error during training request: {ex.Message}");
        }
    }

    void LoadTrainingData(string fileName)
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
            throw new Exception($"Failed to load training data from file '{fileName}': '{ex.Message}'");
        }
    }

    EmbeddingPair GetRandomTrainingData()
    {
        int index = UnityEngine.Random.Range(0, TrainingData.Count);
        return TrainingData[index];
    }

    public void ReportReward(float reward)
    {
        _Rewards.Add(reward);
        _LogCounter++;
        _TotalSteps++;
    }

    void LogReward()
    {
        float meanReward = 0f;
        if (_Rewards.Count > 0)
        {
            meanReward = _Rewards.Average();
        }

        double elapsedSeconds = _Stopwatch.Elapsed.TotalSeconds;
        double iterationsPerSecond = _LogInterval / elapsedSeconds;

        Log.Message($"training >> step={_TotalSteps}, episode={Agent.EpisodeCount} reward={meanReward} ({iterationsPerSecond:F2} it/sec)");

        // Log to TensorBoard
        Academy.Instance.StatsRecorder.Add("Averaged Reward", meanReward);

        _Stopwatch.Restart();
    }

    void UpdateReporting()
    {
        if (_LogCounter >= _LogInterval)
        {
            LogReward();
            _LogCounter = 0;
            _Rewards.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.MLAgents;

public class AgentTrainer : AgentDelegator<AgentTrainer, TauAgent>
{
    public static string TrainingFileName { get; set; }
    public static List<EmbeddingPair> TrainingData { get; set; }
    public static int Columns { get; set; }

    private List<float> _Rewards = new();
    private int _LogCounter = 0;
    private int _TotalSteps = 0;
    private Stopwatch _Stopwatch = new();
    private EmbeddingPair trainingData;
    private TrainingState _currentState = TrainingState.Idle;

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
        Log.Message($"Setting up AgentTrainer. maxSteps={Constants.MaxStepsPerEpisode}, rewardThreshold={Constants.RewardThreshold}, columns={Columns}");

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
        switch (_currentState)
        {
            case TrainingState.Idle:
                if (Agent.StepCount >= Constants.MaxStepsPerEpisode || Agent.GetCumulativeReward() <= Constants.RewardThreshold)
                {
                    EndTrainingEpisode();
                }
                else
                {
                    _currentState = TrainingState.Preparing;
                }
                break;

            case TrainingState.Preparing:
                RequestTraining();
                _currentState = TrainingState.Training;
                break;

            case TrainingState.Training:
                if (Agent.Data.ModelOutput != null)
                {
                    _currentState = TrainingState.Completed;
                }
                break;

            case TrainingState.Completed:
                _currentState = TrainingState.Idle;
                break;
        }

        UpdateReporting();
    }

    void EndTrainingEpisode()
    {
        Agent.EndEpisode();
        _currentState = TrainingState.Idle;
    }

    void RequestTraining()
    {
        try
        {
            trainingData = GetRandomTrainingData();

            Agent.Data.ModelInput = trainingData.InputEmbedding;
            Agent.Data.ExpectedOutput = trainingData.OutputEmbedding;
            Agent.Data.Observations = AgentUtilities.ConvertToFloatArray(Agent.Data.ModelInput);

            // Ensure observations are set before requesting a decision
            if (Agent.Data.Observations != null && Agent.Data.Observations.Length > 0)
            {
                Agent.RequestDecision();
            }
            else
            {
                Log.Error("Observations are not set correctly.");
                _currentState = TrainingState.Idle;
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error during training request: {ex.Message}");
            _currentState = TrainingState.Idle;
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
        double iterationsPerSecond = Constants.LogInterval / elapsedSeconds;

        Log.Message($"training >> step={_TotalSteps}, ep={Agent.EpisodeCount} mean={meanReward} count={_Rewards.Count} ({iterationsPerSecond:F2} itr/s)");

        // Log to TensorBoard
        Academy.Instance.StatsRecorder.Add("Averaged Reward", meanReward);

        _Stopwatch.Restart();
    }

    void UpdateReporting()
    {
        if (_LogCounter >= Constants.LogInterval)
        {
            LogReward();
            _LogCounter = 0;
            _Rewards.Clear();
        }
    }
}

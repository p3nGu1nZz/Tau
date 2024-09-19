using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseProcessor<TDelegator, TAgent>
    where TDelegator : AgentDelegator<TDelegator, TAgent>
    where TAgent : BaseAgent<TDelegator, TAgent>
{
    protected SemaphoreSlim semaphore;
    protected CancellationTokenSource cancellationTokenSource;
    protected List<TrainingPair<TDelegator, TAgent>> trainingPairs;
    protected int columns;

    protected BaseProcessor(int numAgents, int columns)
    {
        semaphore = new SemaphoreSlim(numAgents);
        cancellationTokenSource = new CancellationTokenSource();
        trainingPairs = new List<TrainingPair<TDelegator, TAgent>>();
        this.columns = columns;
    }

    protected void AddTrainingPair(GameObject agentDelegatorInstance, GameObject baseAgentInstance)
    {
        TrainingPair<TDelegator, TAgent> pair = new TrainingPair<TDelegator, TAgent>(agentDelegatorInstance, baseAgentInstance);
        trainingPairs.Add(pair);
    }

    protected async void StartTrainingTask(TrainingPair<TDelegator, TAgent> pair, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            var agentTrainer = pair.AgentDelegator.GetComponent<AgentTrainer>();
            agentTrainer.SetColumns(columns);
            agentTrainer.Initialize();
        }
        catch (OperationCanceledException)
        {
            Log.Message("Training task was canceled.");
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void CancelTraining()
    {
        cancellationTokenSource.Cancel();
    }
}

using CommandTerminal;
using System;

public static class TrainingCancelAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            Log.Message("Cancelling ongoing training...");
            TrainingAgentAction.CancelTraining();
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while cancelling the training: {ex.Message}");
        }
    }
}

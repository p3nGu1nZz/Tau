using CommandTerminal;
using System;

public static class TrainingAgentAction
{
    public static void Execute(CommandArg[] args)
    {
        try
        {
            string agentType = StringUtilities.CapitalizeFirstLetter(args[1].String.ToLower());
            string fileName = args[2].String.ToLower();

            var argValues = CommandUtilities.ParseArgs(args, "agents", "columns");
            int numAgents = argValues.ContainsKey("agents") && int.TryParse(argValues["agents"], out int parsedNumAgents) ? parsedNumAgents : 1;

            TrainingManager.Instance.ExecuteTraining(agentType, fileName, numAgents);
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }

    public static void CancelTraining()
    {
        TrainingManager.Instance.CancelTraining();
    }
}

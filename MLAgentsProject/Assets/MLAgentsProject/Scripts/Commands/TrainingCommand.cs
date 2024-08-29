using CommandTerminal;
using System;
using System.Collections.Generic;

public static class TrainingCommand
{
    private static readonly Dictionary<string, Action<CommandArg[]>> CommandActions = new()
    {
        { TrainingType.Agent.ToLower(), ExecuteAgentTraining }
    };

    [RegisterCommand(Help = "Training a specified agent", MinArgCount = 2)]
    public static void CommandTraining(CommandArg[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("Insufficient arguments for train command. Usage: train agent tau");
            }

            string trainType = args[0].String.ToLower();
            if (CommandActions.TryGetValue(trainType, out var commandAction))
            {
                commandAction(args);
            }
            else
            {
                throw new ArgumentException("Invalid train type. Only 'agent' is supported.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }

    private static void ExecuteAgentTraining(CommandArg[] args)
    {
        string agentType = StringUtilities.CapitalizeFirstLetter(args[1].String.ToLower());
        string fileName = args[2].String.ToLower();
        TrainingAgentAction.Execute(TrainingType.Agent, agentType, fileName);
    }
}

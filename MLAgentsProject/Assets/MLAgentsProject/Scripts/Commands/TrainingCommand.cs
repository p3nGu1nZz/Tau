using CommandTerminal;
using System;
using System.Collections.Generic;

public static class TrainingCommand
{
    private static readonly Dictionary<string, Action<CommandArg[]>> CommandActions = new()
    {
        { TrainingType.Agent.ToLower(), TrainingAgentAction.Execute },
        { "cancel", TrainingCancelAction.Execute },
        { "help", TrainingHelpAction.Execute }
    };

    [RegisterCommand(Help = "Training a specified agent", MinArgCount = 1)]
    public static void CommandTraining(CommandArg[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Insufficient arguments for train command. Usage: train agent tau <filename> [--num-agents <number>] or train cancel or train help");
            }

            string commandType = args[0].String.ToLower();
            if (CommandActions.TryGetValue(commandType, out var commandAction))
            {
                commandAction(args);
            }
            else
            {
                throw new ArgumentException("Invalid command type. Only 'agent', 'cancel', and 'help' are supported.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the training command: {ex.Message}");
        }
    }
}

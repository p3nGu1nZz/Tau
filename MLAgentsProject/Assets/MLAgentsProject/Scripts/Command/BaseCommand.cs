using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseCommand<T> where T : BaseCommand<T>, new()
{
    protected abstract Dictionary<string, Action<CommandArg[]>> CommandActions { get; }

    public static void Execute(CommandArg[] args)
    {
        var command = new T();
        command.ExecuteCommand(args);
    }

    protected void ExecuteCommand(CommandArg[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                throw new ArgumentException("Insufficient arguments.");
            }

            string command = args[0].String.ToLower();
            var commandAction = GetCommandAction(command, args);
            if (commandAction != null)
            {
                commandAction(args.Skip(1).ToArray());
            }
            else
            {
                throw new ArgumentException("Invalid command or insufficient arguments.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"An error occurred while executing the command: {ex.Message}");
        }
    }

    protected virtual Action<CommandArg[]> GetCommandAction(string command, CommandArg[] args)
    {
        if (CommandActions.TryGetValue(command, out var commandAction))
        {
            return commandAction;
        }

        return null;
    }
}

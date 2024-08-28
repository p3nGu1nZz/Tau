using CommandTerminal;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseCommand<T> where T : BaseCommand<T>, new()
{
    protected abstract Dictionary<string, Action<CommandArg[]>> Actions { get; }

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
            var action = GetAction(command, args);
            if (action != null)
            {
                action(args);
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

    protected virtual Action<CommandArg[]> GetAction(string command, CommandArg[] args)
    {
        if (Actions.TryGetValue(command, out var commandAction))
        {
            return commandAction;
        }

        return null;
    }
}

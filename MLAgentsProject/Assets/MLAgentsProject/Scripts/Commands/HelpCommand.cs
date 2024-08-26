using CommandTerminal;
using UnityEngine;
using System.Collections.Generic;

public static class HelpCommand
{
    [RegisterCommand(Help = "Lists all Commands or displays help documentation of a Command", MaxArgCount = 1)]
    static void CommandHelp(CommandArg[] args)
    {
        if (args.Length == 0)
        {
            var helpText = new List<string>
            {
                "Usage: <command> [options]",
                "",
                "Commands:"
            };

            foreach (var command in Terminal.Shell.Commands)
            {
                helpText.Add($"  {command.Key.PadRight(16).ToLower()} {command.Value.help}");
            }

            helpText.Add("");

            foreach (var line in helpText)
            {
                Terminal.Log(line);
            }
            return;
        }

        string command_name = args[0].String.ToUpper();

        if (!Terminal.Shell.Commands.ContainsKey(command_name))
        {
            Terminal.Shell.IssueErrorMessage($"Command {command_name} could not be found.");
            return;
        }

        string help = Terminal.Shell.Commands[command_name].help;

        if (help == null)
        {
            Terminal.Log($"{command_name} does not provide any help documentation.");
        }
        else
        {
            Terminal.Log(help);
        }
    }
}

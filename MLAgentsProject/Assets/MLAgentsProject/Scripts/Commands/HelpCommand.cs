using CommandTerminal;
using UnityEngine;
using System.IO;

public static class HelpCommand
{
    [RegisterCommand(Help = "Lists all Commands or displays help documentation of a Command", MaxArgCount = 1)]
    static void CommandHelp(CommandArg[] args)
    {
        if (args.Length == 0)
        {
            foreach (var command in Terminal.Shell.Commands)
            {
                Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
            }
            return;
        }

        string command_name = args[0].String.ToUpper();

        if (!Terminal.Shell.Commands.ContainsKey(command_name))
        {
            Terminal.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
            return;
        }

        string help = Terminal.Shell.Commands[command_name].help;

        if (help == null)
        {
            Terminal.Log("{0} does not provide any help documentation.", command_name);
        }
        else
        {
            Terminal.Log(help);
        }
    }
}

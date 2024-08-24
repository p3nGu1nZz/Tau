using CommandTerminal;
using UnityEngine;
using System.IO;

public static class ClearCommand
{
    [RegisterCommand(Help = "Clears the Command Console", MaxArgCount = 0)]
    static void CommandClear(CommandArg[] args)
    {
        Terminal.Buffer.Clear();
    }
}

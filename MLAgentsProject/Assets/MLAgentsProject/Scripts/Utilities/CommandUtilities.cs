using CommandTerminal;
using System;
using System.Collections.Generic;

public static class CommandUtilities
{
    public static Dictionary<string, string> ParseArgs(CommandArg[] args, params string[] argNames)
    {
        var argValues = new Dictionary<string, string>();

        for (int i = 0; i < args.Length; i++)
        {
            foreach (var argName in argNames)
            {
                if (args[i].String == $"--{argName}" && i + 1 < args.Length)
                {
                    argValues[argName] = args[i + 1].String;
                    i++; // Skip the next argument as it's the value for the current argName
                }
            }
        }

        return argValues;
    }
}

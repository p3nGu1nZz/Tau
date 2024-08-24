using CommandTerminal;
using UnityEngine;
using System.IO;

public static class QuitCommand
{
    [RegisterCommand(Help = "Quits running Application", MaxArgCount = 0)]
    static void CommandQuit(CommandArg[] args)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

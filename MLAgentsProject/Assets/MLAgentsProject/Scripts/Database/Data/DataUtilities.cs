using CommandTerminal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class DataUtilities
{
    public static string GetFilePath(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException(nameof(fileName), "File name cannot be null or empty.");
        }

        return Path.Combine(Application.dataPath, DatabaseConstants.UpDirectoryLevel, DatabaseConstants.DataDirectoryName, fileName);
    }

    public static string CombineArgs(CommandArg[] args)
    {
        return string.Join(" ", args.Select(arg => arg.String));
    }

    public static List<string> ParseFileNames(string combinedArgs)
    {
        if (string.IsNullOrEmpty(combinedArgs))
        {
            throw new ArgumentException("Combined arguments cannot be null or empty.");
        }

        return combinedArgs.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public static List<string> GetDirectoryContents(string directoryPath, string searchPattern)
    {
        if (!Directory.Exists(directoryPath))
        {
            throw new DirectoryNotFoundException($"Directory '{directoryPath}' not found.");
        }

        return Directory.GetFiles(directoryPath, searchPattern).Select(Path.GetFileName).ToList();
    }
}

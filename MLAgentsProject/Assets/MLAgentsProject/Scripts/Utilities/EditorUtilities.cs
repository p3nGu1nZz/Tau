using System.IO;
using UnityEngine;

public static class EditorUtilities
{
    public static void CreateDirectories(string[] directories)
    {
        foreach (string directory in directories)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                Debug.Log($"Created directory: {directory}");
            }
        }
    }

    public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
    {
        foreach (DirectoryInfo dir in source.GetDirectories())
        {
            CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            Debug.Log($"Created subdirectory: {dir.Name}");
        }
        foreach (FileInfo file in source.GetFiles())
        {
            file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            Debug.Log($"Copied file: {file.Name} to {target.FullName}");
        }
    }
}

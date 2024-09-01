using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;

public class PostBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        string buildPath = Path.GetDirectoryName(report.summary.outputPath);
        string projectRoot = GetProjectRootPath();
        string sourceScripts = Path.Combine(projectRoot, "Scripts");
        string sourceData = Path.Combine(projectRoot, "Data");
        string destScripts = Path.Combine(buildPath, "Scripts");
        string destData = Path.Combine(buildPath, "Data");

        Debug.Log($"Build Path: {buildPath}\nProject Root: {projectRoot}\nSource Scripts Path: {sourceScripts}\nSource Data Path: {sourceData}\nDestination Scripts Path: {destScripts}\nDestination Data Path: {destData}");

        EditorUtilities.CreateDirectories(new string[] { destScripts, destData });

        EditorUtilities.CopyFilesRecursively(new DirectoryInfo(sourceScripts), new DirectoryInfo(destScripts));
        EditorUtilities.CopyFilesRecursively(new DirectoryInfo(sourceData), new DirectoryInfo(destData));

        Debug.Log("Post-build process completed: Scripts and Data directories copied.");
    }

    private static string GetProjectRootPath()
    {
        return Path.Combine(Application.dataPath, DatabaseConstants.UpDirectoryLevel);
    }
}

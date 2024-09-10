using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Ophrase : MonoBehaviour
{
    private static Ophrase _instance;
    public static Ophrase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Ophrase>();
                if (_instance == null)
                {
                    Log.Error("Ophrase object not found in the scene.");
                }
            }
            return _instance;
        }
    }

    public string BatchFilePath { get; private set; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        BatchFilePath = Path.Combine(Application.dataPath, "..", "Scripts", "ophrase.bat");
    }

    public async Task<string[]> Paraphrase(string inputString)
    {
        string command = $"/c \"{BatchFilePath} \"{inputString}\"\"";
        Log.Message($"Executing command: {command}");

        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Log.Message($"Process start info configured with arguments: {startInfo.Arguments}");

        try
        {
            using (var process = Process.Start(startInfo))
            {
                string result = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    Log.Error($"Process error: {error}");
                }

                Log.Message($"Process output: {result}");

                try
                {
                    var ophraseResult = JsonUtility.FromJson<OphraseResult>(result);
                    Log.Message($"Parsed {ophraseResult.responses.Length} paraphrased responses.");
                    return ophraseResult.responses;
                }
                catch (Exception jsonEx)
                {
                    Log.Error($"JSON parse error: {jsonEx.Message}");
                    return new string[] { "Error: JSON parse error." };
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Exception occurred during paraphrasing: {ex.Message}");
            return new string[] { "Error: Exception occurred during paraphrasing." };
        }
    }
}

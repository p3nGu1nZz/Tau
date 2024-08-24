using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class Encoder : MonoBehaviour
{
    private static Encoder _instance;
    public static Encoder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<Encoder>();
                if (_instance == null)
                {
                    UnityEngine.Debug.LogError("Encoder object not found in the scene.");
                }
            }
            return _instance;
        }
    }

    private static string batchFilePath;

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

        batchFilePath = Path.Combine(Application.dataPath, "..", "Scripts", "encoder.bat");
    }

    public async Task<string> Encode(string inputString)
    {
        string command = $"/c \"{batchFilePath} \"{inputString}\"\"";
        UnityEngine.Debug.Log($"Command: {command}");

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        UnityEngine.Debug.Log($"Starting process with arguments: {start.Arguments}");

        try
        {
            Process process = Process.Start(start);
            using StreamReader reader = process.StandardOutput;
            using StreamReader errorReader = process.StandardError;

            string result = "";
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                result += line;
            }

            string error = await errorReader.ReadToEndAsync();
            if (!string.IsNullOrEmpty(error))
            {
                UnityEngine.Debug.LogError($"Process error: {error}");
            }

            return result;
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"Exception occurred: {ex.Message}");
            return null;
        }
    }
}

using CommandTerminal;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

public static class Log
{
    private static readonly string logFilePath = Path.Combine(Application.dataPath, "..", "logs", "log.txt");
    private static ConcurrentQueue<string> messageQueue = new ConcurrentQueue<string>();
    private static ConcurrentQueue<string> warningQueue = new ConcurrentQueue<string>();
    private static ConcurrentQueue<string> errorQueue = new ConcurrentQueue<string>();
    public static int maxSpeed = 500;
    public static int minSpeed = 1;

    public static int LogsPerFrame { get; set; } = 1; // Default value

    static Log()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
    }

    public static void Message(string message)
    {
        messageQueue.Enqueue(message);
    }

    public static void Warning(string message)
    {
        warningQueue.Enqueue($"[WARNING] {message}");
    }

    public static void Error(string message)
    {
        errorQueue.Enqueue($"[ERROR] {message}");
    }

    public static void LogToFile(string message)
    {
        int retryCount = 3;
        while (retryCount > 0)
        {
            try
            {
                using (var fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.WriteLine($"{DateTime.Now}: {message}");
                }
                break;
            }
            catch (IOException)
            {
                retryCount--;
                if (retryCount == 0) throw;
                Thread.Sleep(100); // Wait before retrying
            }
        }
    }

    public static int CalculateLogSpeed(int totalQueueSize)
    {
        int speed = Math.Max(minSpeed, Math.Min(maxSpeed, totalQueueSize));
        return speed;
    }

    public static int GetTotalQueueSize()
    {
        return (messageQueue.Count + warningQueue.Count + errorQueue.Count) / 3;
    }

    public static void ProcessQueue()
    {
        LogsPerFrame = CalculateLogSpeed(GetTotalQueueSize());

        int logsProcessed = 0;
        while (logsProcessed < LogsPerFrame && messageQueue.TryDequeue(out string message))
        {
            Debug.Log(message);
            LogToFile(message);
            logsProcessed++;
        }
        while (logsProcessed < LogsPerFrame && warningQueue.TryDequeue(out string warningMessage))
        {
            Debug.LogWarning(warningMessage);
            LogToFile(warningMessage);
            logsProcessed++;
        }
        while (logsProcessed < LogsPerFrame && errorQueue.TryDequeue(out string errorMessage))
        {
            Debug.LogError(errorMessage);
            LogToFile(errorMessage);
            logsProcessed++;
        }
    }
}

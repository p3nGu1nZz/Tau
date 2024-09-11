using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public interface ITask
{
    void InitializeCounters();
    Task Process(MessageList messageList, string jsonDataFilename);
    Task<string[]> Generate(string userContent, TimeSpan timeout);
    Task<string[]> Execute(string userContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000);
    string GetUserContent(Message message);
    List<Message> CreateNewMessages(Message originalMessage, string[] responses);
    Message GetMessage(MessageList messageList, int index);
    void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix);
    void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages);
    Task HandleTasksCompletion(List<Task> tasks);
    void LogProcessingCompletion(Stopwatch stopwatch, MessageList messageList, List<Message> newMessagesList, string jsonDataFilename, List<Message> errorMessageList);
    List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages);
    Task ProcessUserContent(string userContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages);
    void ValidateResponses(string[] responses, string userContent);
    void AddNewMessages(Message message, string[] responses, List<Message> newMessagesList);
    void AddErrorMessage(Message message, List<Message> errorMessageList);
    void UpdateCounters(int generatedPhrases, int processedMessages);
}

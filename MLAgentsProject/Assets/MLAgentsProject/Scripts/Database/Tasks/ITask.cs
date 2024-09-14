using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

public interface ITask<TResult>
{
    void InitializeCounters();
    Task Process(MessageList messageList, string jsonDataFilename);
    Task<List<TResult>> Generate(string userContent, string agentContent, TimeSpan timeout);
    Task<List<TResult>> Execute(string userContent, string agentContent, TimeSpan timeout, int maxRetries = 1, int delay = 1000);
    string GetUserContent(Message message);
    string GetAgentContent(Message message);
    List<Message> CreateNewMessages(Message originalMessage, List<string> responses);
    Message GetMessage(MessageList messageList, int index);
    void SaveMessages(MessageList messageList, string jsonDataFilename, string suffix);
    void SaveErrorMessages(List<Message> errorMessageList, string jsonDataFilename, int totalErrorMessages, string suffix);
    Task HandleTasksCompletion(List<Task> tasks);
    void LogProcessingCompletion(Stopwatch stopwatch, MessageList messageList, List<Message> newMessagesList, string jsonDataFilename, List<Message> errorMessageList, string suffix);
    List<Task> CreateTasks(MessageList messageList, List<Message> newMessagesList, List<Message> errorMessageList, int totalMessages);
    Task ProcessContent(string userContent, string agentContent, Message message, List<Message> newMessagesList, List<Message> errorMessageList, int index, int totalMessages);
    void ValidateResponses(List<string> responses, string userContent);
    void AddNewMessages(Message message, List<string> responses, List<Message> newMessagesList);
    void AddErrorMessage(Message message, List<Message> errorMessageList);
    void UpdateCounters(int generatedPhrases, int processedMessages);
    void RemoveMessages(MessageList messageList, List<Message> responses);
}

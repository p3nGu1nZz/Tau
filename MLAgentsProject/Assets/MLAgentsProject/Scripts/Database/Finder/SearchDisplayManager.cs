using System.Collections.Generic;

public class SearchDisplayManager
{
    private readonly Dictionary<string, List<Embedding>> _tables;
    private readonly TableSearcher _tableSearcher;
    private readonly TableDisplay _tableDisplay;

    public SearchDisplayManager(Dictionary<string, List<Embedding>> tables)
    {
        _tables = tables;
        _tableSearcher = new TableSearcher(_tables);
        _tableDisplay = new TableDisplay(_tables);
    }

    public void ViewTable(string tableName)
    {
        if (string.IsNullOrEmpty(tableName))
        {
            Log.Error("Insufficient arguments for view table command.");
            return;
        }

        _tableDisplay.DisplayTable(tableName);
    }

    public void FindInTable(string tableName, string token)
    {
        var results = _tableSearcher.SearchInTable(tableName, token);
        var resultFormatter = new ResultFormatter();
        Log.Message(resultFormatter.FormatFindResults(results));
    }
}

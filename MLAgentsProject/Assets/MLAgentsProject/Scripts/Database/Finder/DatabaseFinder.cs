using System.Collections.Generic;

public class DatabaseFinder
{
    private readonly TableSearcher _tableSearcher;
    private readonly ResultFormatter _resultFormatter;

    public DatabaseFinder(Dictionary<string, List<Embedding>> tables)
    {
        _tableSearcher = new TableSearcher(tables);
        _resultFormatter = new ResultFormatter();
    }

    public void FindInTables(string token, string tableName, bool searchAllTables)
    {
        var results = _tableSearcher.SearchInTables(token, searchAllTables, tableName);
        Log.Message(_resultFormatter.FormatFindResults(results));
    }
}

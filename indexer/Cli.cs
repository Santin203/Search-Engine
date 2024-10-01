namespace Indexer
{
    public class CliMode : IUserInterface
{
    private readonly SearchEngineCore _searchEngine;

    public CliMode(SearchEngineCore searchEngine)
    {
        _searchEngine = searchEngine;
    }

    public void IndexFolder(string folderPath)
    {
        _searchEngine.IndexFolder(folderPath);
        Console.WriteLine("Folder indexed successfully.");
    }

    public void LoadIndex(string indexPath)
    {
        _searchEngine.LoadIndex(indexPath);
        Console.WriteLine("Index loaded successfully.");
    }

    public void Search(string query, int k)
    {
        var results = _searchEngine.Search(query, k);
        Console.WriteLine($"Found {results.Count} results:");
        foreach (var result in results)
        {
            Console.WriteLine(result);
        }
    }
}

}
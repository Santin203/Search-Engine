namespace Indexer
{
    public class CliMode : IUserInterface
    {
        private readonly SearchEngineCore searchEngine;

        public CliMode(SearchEngineCore searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        public void IndexFolder(string folderPath)
        {
            searchEngine.IndexFolder(folderPath);
            Console.WriteLine("Folder indexed successfully.");
        }

        public void LoadIndex(string indexPath)
        {
            SearchEngineCore.LoadIndex(indexPath);
            Console.WriteLine("Index loaded successfully.");
        }

        public void Search(string query, int k)
        {
            var results = searchEngine.Search(query, k);
            Console.WriteLine($"Found {results.Count} results:");
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }
    }

}
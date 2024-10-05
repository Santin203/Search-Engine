namespace Indexer
{
    public class Cli
    {
        private readonly SearchEngineCore searchEngine;

        public Cli(SearchEngineCore searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        public void Run()
        {
            bool isRunning = true;

            // Command loop
            while (isRunning)
            {
                Console.WriteLine("\nPlease enter a command (index, search, load, or exit):");
                string command = Console.ReadLine()?.Trim();

                switch (command)
                {
                    case "index":
                        SetVectorizer();
                        SetDistance();
                        HandleIndexCommand();
                        break;

                    case "search":
                        HandleSearchCommand();
                        break;

                    case "load":
                        HandleLoadCommand();
                        break;

                    case "exit":
                        isRunning = false;
                        Console.WriteLine("Exiting the search engine. Goodbye!");
                        break;

                    default:
                        Console.WriteLine("Invalid command. Please use 'index', 'search', 'load', or 'exit'.");
                        break;
                }
            }
        }

        private void HandleIndexCommand()
        {
            Console.WriteLine("Enter the folder path to index:");
            string folderPath = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(folderPath))
            {
                searchEngine.IndexFolder(folderPath);
                Console.WriteLine("Indexing completed.");
            }
            else
            {
                Console.WriteLine("Folder path cannot be empty.");
            }
        }

        private void HandleLoadCommand()
        {
            Console.WriteLine("Enter the path to the saved index file:");
            string filePath = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                searchEngine.LoadIndex(filePath);
            }
            else
            {
                Console.WriteLine("Invalid file path or file does not exist.");
            }
        }


        private void HandleSearchCommand()
        {
            Console.WriteLine("Enter the search query:");
            string query = Console.ReadLine()?.Trim();
            
            Console.WriteLine("Enter the number of top results to return (k):");
            if (int.TryParse(Console.ReadLine(), out int k))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    var results = searchEngine.Search(query, k);
                    if (results.Count > 0)
                    {
                        Console.WriteLine("\nTop results:");
                        foreach (var result in results)
                        {
                            Console.WriteLine(result);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No results found.");
                    }
                }
                else
                {
                    Console.WriteLine("Query cannot be empty.");
                }
            }
            else
            {
                Console.WriteLine("Please enter a valid number for k.");
            }
        }
        
        private void SetVectorizer()
        {
            bool askIndexer = true;
            while(askIndexer)
            {
                //Ask for indexing type
                Console.WriteLine("Please select an indexer type:\n1- TF-IDF\n2- BM25");
                string command = Console.ReadLine()?.Trim();

                //Select type and change vectorizer if needed
                switch (command)
                {
                    case "1":
                        Console.WriteLine("Indexing using TF-IDF");
                        askIndexer = false;
                        break;
                    case "2":
                        Console.WriteLine("Indexing using BM25");
                        askIndexer = false;
                        break;
                    default:
                        Console.WriteLine("Outside of range given, please try again.");
                        break;
                }
            }
        }

        private void SetDistance()
        {
            bool askDistance = true;
            while(askDistance)
            {
                //Ask for distance type
                Console.WriteLine("Please select a distance type:\n1- Cosine\n2- Euclidean");
                string command = Console.ReadLine()?.Trim();

                //Select type and change distance if needed
                switch (command)
                {
                    case "1":
                        Console.WriteLine("Using Cosine Similarity");
                        askDistance = false;
                        break;
                    case "2":
                        Console.WriteLine("Using Searchito");
                        askDistance = false;
                        break;
                    default:
                        Console.WriteLine("Outside of range given, please try again.");
                        break;
                }
            }
        }
    }
}

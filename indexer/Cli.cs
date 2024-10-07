namespace Indexer
{
    public class Cli
    {
        private readonly SearchEngineCore searchEngine;

        public Cli(SearchEngineCore searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        // Main loop that listens for user commands
        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\nPlease enter a command (index, search, load, or exit):");
                string? command = Console.ReadLine()?.Trim();

                switch (command)
                {
                    case "index":
                        SetVectorizer();
                        HandleIndexCommand();
                        break;

                    case "search":
                        SetDistance();
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

        // Handles the process of indexing documents
        private void HandleIndexCommand()
        {
            Console.WriteLine("Enter the folder path to index:");
            string? folderPath = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(folderPath))
            {
                searchEngine.IndexFolder(folderPath);  // Index documents from the specified folder
                Console.WriteLine("Indexing completed.");
            }
            else
            {
                Console.WriteLine("Folder path cannot be empty.");
            }
        }

        // Handles loading a previously saved index from a file
        private void HandleLoadCommand()
        {
            Console.WriteLine("Enter the path to the saved index file:");
            string? filePath = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                searchEngine.LoadIndex(filePath);  // Load index data from the provided file path
            }
            else
            {
                Console.WriteLine("Invalid file path or file does not exist.");
            }
        }

        // Handles user search queries
        private void HandleSearchCommand()
        {
            Console.WriteLine("Enter the search query:");
            string? query = Console.ReadLine()?.Trim();
            
            Console.WriteLine("Enter the number of top results to return (k):");
            if (int.TryParse(Console.ReadLine(), out int k))
            {
                if (!string.IsNullOrEmpty(query))
                {
                    var results = searchEngine.Search(query, k);  // Perform search using the provided query and return top k results
                    if (results.Count > 0)
                    {
                        Console.WriteLine("\nTop results:");
                        foreach (var result in results)
                        {
                            Console.WriteLine(result);  // Display the search results
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
        
        // Prompts user to select the indexer type
        private void SetVectorizer()
        {
            bool askIndexer = true;
            while(askIndexer)
            {
                Console.WriteLine("Please select an indexer type:\n1- TF-IDF\n2- BM25");
                string? command = Console.ReadLine()?.Trim();

                switch (command)
                {
                    case "1":
                        Console.WriteLine("Indexing using TF-IDF");
                        searchEngine.SetVectorizer(int.Parse(command));  // Set vectorizer to TF-IDF
                        askIndexer = false;
                        break;
                    case "2":
                        Console.WriteLine("Indexing using BM25");
                        searchEngine.SetVectorizer(int.Parse(command));  // Set vectorizer to BM25
                        askIndexer = false;
                        break;
                    default:
                        Console.WriteLine("Outside of range given, please try again.");
                        break;
                }
            }
        }

        // Prompts user to select the distance
        private void SetDistance()
        {
            bool askDistance = true;
            while(askDistance)
            {
                Console.WriteLine("Please select a distance type:\n1- Cosine\n2- Searchito");
                string? command = Console.ReadLine()?.Trim();

                switch (command)
                {
                    case "1":
                        Console.WriteLine("Using Cosine Similarity");
                        searchEngine.SetDistance(int.Parse(command));  // Set distance to Cosine Similarity
                        askDistance = false;
                        break;
                    case "2":
                        Console.WriteLine("Using Searchito");
                        searchEngine.SetDistance(int.Parse(command));  // Set distance to Searchito
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

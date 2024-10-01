using System;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instantiate the search engine core
            var searchEngine = new SearchEngineCore();
            bool isRunning = true;

            Console.WriteLine("Welcome to the Search Engine");
            
            // Command loop
            while (isRunning)
            {
                Console.WriteLine("\nPlease enter a command (index, search, load, or exit):");
                string command = Console.ReadLine()?.Trim();

                switch (command)
                {
                    case "index":
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
                        break;

                    case "search":
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
                        break;

                    case "load":
                        // Future implementation for loading a pre-indexed file
                        Console.WriteLine("Load functionality not yet implemented.");
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
    }
}

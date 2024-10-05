using System;

namespace Indexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Search Engine");

            var searchEngine = new SearchEngineCore(new TFIDF(), new CosineSimilarity());
            var cli = new Cli(searchEngine);

            cli.Run();
        }
    }
}

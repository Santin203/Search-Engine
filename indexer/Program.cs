using System;

namespace Indexer
{
    class Program
    {
        static void Main()
        {
            var documents = new string[]
            {
                "This is the first document.",
                "This document is the second document.",
                "And this is the third one.",
                "Is this the first document?"
            };

            TfIdfVectorizer vectorizer = new TfIdfVectorizer();
            var tfidfMatrix = vectorizer.FitTransform(documents);

            foreach (var docVector in tfidfMatrix)
            {
                Console.WriteLine("TF-IDF Vector:");
                foreach (var kvp in docVector)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;

namespace Indexer
{
    public abstract class Indexer
    {
        // Vocabulary and documents are common across all indexers
        public Dictionary<string, int> Vocabulary { get; protected set; }
        protected string[] Documents;

        protected Indexer()
        {
            Vocabulary = new Dictionary<string, int>();
            Documents = Array.Empty<string>();
        }

        // Common methods for all vectorizers (tokenization and vocabulary building)
        protected List<string> Tokenize(string doc)
        {
            var tokens = doc.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return tokens.Where(token => !StopWords.stopWordsList.Contains(token)).ToList(); // Stop word filtering
        }

        protected void BuildVocabulary(string[] documents)
        {
            int index = 0;
            foreach (var doc in documents)
            {
                var tokens = Tokenize(doc);
                foreach (var token in tokens)
                {
                    if (!Vocabulary.ContainsKey(token))
                    {
                        Vocabulary[token] = index++;
                    }
                }
            }
        }

        // Abstract methods to be implemented by child classes
        public abstract void Fit(string[] documents);
        public abstract List<Dictionary<string, double>> Transform(string[] documents);
        public abstract List<Dictionary<string, double>> FitTransform(string[] documents);
    }
}

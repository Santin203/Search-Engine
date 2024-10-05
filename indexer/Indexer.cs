using System;
using System.Collections.Generic;

namespace Indexer
{
    public abstract class Indexer
    {
        // Vocabulary and documents are common across all indexers
        public Dictionary<string, int> Vocabulary { get; set; }
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

        public List<Dictionary<string, double>> Transform(string[] documents)
        {
            var tfidfVectors = new List<Dictionary<string, double>>();

            foreach (var doc in documents)
            {
                //Tokenize document (separate words)
                var tokens = Tokenize(doc);

                //Calculate words' term frequency
                var termFrequency = tokens.GroupBy(x => x).ToDictionary(g => g.Key, g => (double)g.Count() / tokens.Count);

                //Compute vector for 
                tfidfVectors.Add(ComputeVector(termFrequency));
            }

            return tfidfVectors;
        }

        protected Dictionary<string, int> ComputeDocFrequency()
        {
            var docFrequency = new Dictionary<string, int>();

            foreach (var doc in Documents)
            {
                var tokens = Tokenize(doc).Distinct();
                foreach (var token in tokens)
                {
                    if (!docFrequency.ContainsKey(token))
                    {
                        docFrequency[token] = 0;
                    }
                    docFrequency[token]++;
                }
            }
            return docFrequency;
        }

        // Abstract methods to be implemented by child classes
        public abstract void Fit(string[] documents);
        protected abstract void ComputeIdf();
        protected abstract Dictionary<string, double> ComputeVector(Dictionary<string, double> termFrequency);
        public abstract List<Dictionary<string, double>> FitTransform(string[] documents);
    }
}

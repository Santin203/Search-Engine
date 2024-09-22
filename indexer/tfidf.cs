using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class TfIdfVectorizer
    {
        public Dictionary<string, int> Vocabulary { get; private set; }
        public Dictionary<string, double> Idf { get; private set; }
        private string[] _documents;
        private bool _useIdf;
        private bool _smoothIdf;

        public TfIdfVectorizer(bool useIdf = true, bool smoothIdf = true)
        {
            Vocabulary = new Dictionary<string, int>();
            Idf = new Dictionary<string, double>();
            _documents = Array.Empty<string>();
            _useIdf = useIdf;
            _smoothIdf = smoothIdf;
        }

        // Tokenize a document into words (filtering stop words)
        private List<string> Tokenize(string doc)
        {
            var tokens = doc.ToLower().Split(' ').Where(word => word.Length > 0).ToList();
            return tokens.Where(token => !StopWords.stopWordsList.Contains(token)).ToList(); // Filter stop words
        }

        // Build vocabulary from documents
        private void BuildVocabulary(string[] documents)
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

        // Fit the vectorizer to the documents
        public void Fit(string[] documents)
        {
            _documents = documents;
            BuildVocabulary(documents);
            if (_useIdf)
            {
                ComputeIdf();
            }
        }

        // Compute IDF values
        private void ComputeIdf()
        {
            int docCount = _documents.Length;
            var docFrequency = new Dictionary<string, int>();

            foreach (var doc in _documents)
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

            foreach (var token in Vocabulary.Keys)
            {
                double idfValue = Math.Log((double)(docCount + (_smoothIdf ? 1 : 0)) / (docFrequency.ContainsKey(token) ? docFrequency[token] + (_smoothIdf ? 1 : 0) : 1)) + 1;
                Idf[token] = idfValue;
            }
        }

        // Transform documents into their TF-IDF representations
        public List<Dictionary<string, double>> Transform(string[] documents)
        {
            var tfidfVectors = new List<Dictionary<string, double>>();

            foreach (var doc in documents)
            {
                var tfidf = new Dictionary<string, double>();
                var tokens = Tokenize(doc);
                var termFrequency = tokens.GroupBy(x => x).ToDictionary(g => g.Key, g => (double)g.Count() / tokens.Count);

                foreach (var term in termFrequency.Keys)
                {
                    if (Vocabulary.ContainsKey(term))
                    {
                        double tfidfValue = termFrequency[term];
                        if (_useIdf && Idf.ContainsKey(term))
                        {
                            tfidfValue *= Idf[term];
                        }
                        tfidf[term] = tfidfValue;
                    }
                }

                tfidfVectors.Add(tfidf);
            }

            return tfidfVectors;
        }

        // Fit the vectorizer and transform the documents in one go
        public List<Dictionary<string, double>> FitTransform(string[] documents)
        {
            Fit(documents);
            return Transform(documents);
        }
    }
}

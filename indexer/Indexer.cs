using System;
using System.Collections.Generic;

namespace Indexer
{
    public abstract class Indexer
    {

        public Dictionary<string, int> Vocabulary { get; set; }
        protected string[] Documents;

        protected Indexer()
        {
            Vocabulary = new Dictionary<string, int>();
            Documents = Array.Empty<string>();
        }

        // Tokenize document and remove stop words
        protected List<string> Tokenize(string doc)
        {
            var tokens = doc.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return tokens.Where(token => !StopWords.stopWordsList.Contains(token)).ToList(); // Stop word filtering
        }

        // Build vocabulary from documents
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

        // Transform documents into vectors
        public List<Dictionary<string, double>> TransformToVector(string[] documents)
        {
            var Vectors = new List<Dictionary<string, double>>();

            foreach (var doc in documents)
            {
                //Tokenize document
                var tokens = Tokenize(doc);

                //Compute vector for doc
                Vectors.Add(ComputeVector(tokens));
            }

            return Vectors;
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

        // Fit and transform documents into vectors
        public  List<Dictionary<string, double>> FitTransform(string[] documents)
        {
            Fit(documents);
            return TransformToVector(documents);
        }

        // Abstract methods to be implemented by child classes
        public abstract void Fit(string[] documents);
        protected abstract void ComputeIdf();
        protected abstract Dictionary<string, double> ComputeVector(List<string> words);
    }
}

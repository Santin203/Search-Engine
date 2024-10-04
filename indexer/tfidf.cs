using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class TFIDF : Indexer
    {
        public Dictionary<string, double> Idf { get; private set; }
        private bool _useIdf;
        private bool _smoothIdf;

        public TFIDF(bool useIdf = true, bool smoothIdf = true)
        {
            Idf = new Dictionary<string, double>();
            _useIdf = useIdf;
            _smoothIdf = smoothIdf;
        }

        public override void Fit(string[] documents)
        {
            Documents = documents;
            BuildVocabulary(documents);
            if (_useIdf)
            {
                ComputeIdf();
            }
        }

        protected override void ComputeIdf()
        {
            int docCount = Documents.Length;
            var docFrequency = ComputeDocFrequency();
            
            //Compute IDF
            foreach (var token in Vocabulary.Keys)
            {
                double idfValue = Math.Log((double)(docCount + (_smoothIdf ? 1 : 0)) / 
                                           (docFrequency.ContainsKey(token) ? docFrequency[token] + (_smoothIdf ? 1 : 0) : 1)) + 1;
                Idf[token] = idfValue;
            }
        }

        public override List<Dictionary<string, double>> Transform(string[] documents)
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

        public override List<Dictionary<string, double>> FitTransform(string[] documents)
        {
            Fit(documents);
            return Transform(documents);
        }
    }
}

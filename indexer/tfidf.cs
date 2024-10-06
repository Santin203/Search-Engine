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

        protected override Dictionary<string, double> ComputeVector(List<string> words)
        {
            //Make new tfidf vector 
            var tfidf = new Dictionary<string, double>();

            //Calculate words' normalized term frequency
            var termFrequency = words.GroupBy(x => x).ToDictionary(g => g.Key, g => (double)g.Count() / words.Count);

            //Iterate through list of terms and calculate their tfidf
            foreach (var term in termFrequency.Keys)
            {
                if (Vocabulary.ContainsKey(term))
                {
                    double tfidfValue = termFrequency[term];
                    if (_useIdf && Idf.ContainsKey(term))
                    {
                        tfidfValue *= Idf[term];
                    }
                    //Store tfidf score into dictionary
                    tfidf[term] = tfidfValue;
                }
            }

            //Return vector component
            return tfidf;
        }
    }
}

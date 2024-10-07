using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class TFIDF : Indexer
    {
        public Dictionary<string, double> Idf { get; private set; }
        private bool useIdf;
        private bool smoothIdf;

        public TFIDF(bool useIdf = true, bool smoothIdf = true)
        {
            Idf = new Dictionary<string, double>();
            this.useIdf = useIdf;
            this.smoothIdf = smoothIdf;
        }

        // Fit the model with the documents
        public override void Fit(string[] documents)
        {
            Documents = documents;
            BuildVocabulary(documents);
            if (useIdf)
            {
                ComputeIdf();
            }
        }

        // Compute the Inverse Document Frequency (IDF) values for the terms
        protected override void ComputeIdf()
        {
            int docCount = Documents.Length;
            var docFrequency = ComputeDocFrequency();
            
            //Compute IDF
            foreach (var token in Vocabulary.Keys)
            {
                double idfValue = Math.Log((double)(docCount + (smoothIdf ? 1 : 0)) / 
                                           (docFrequency.ContainsKey(token) ? docFrequency[token] + (smoothIdf ? 1 : 0) : 1)) + 1;
                Idf[token] = idfValue;
            }
        }

        // Compute the TF-IDF vector for a document
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
                    if (useIdf && Idf.ContainsKey(term))
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

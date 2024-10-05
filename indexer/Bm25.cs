using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class Bm25: Indexer
    {
        double _k = 1.5;
        double _b = 0.75;
        double _averageDocumentLength;
        int _docCount;
        public Dictionary<string, double> Idf { get; private set; }
        public Bm25()
        {
            Idf = new Dictionary<string, double>();
            _averageDocumentLength = 0;
        }
        public override void Fit(string[] documents)
        {
            Documents = documents;
            GetAverageDocumentLength();
            BuildVocabulary(documents);

            ComputeIdf();
        }

        protected void GetAverageDocumentLength()
        {
            List<int> lengths = new List<int>{};
            List<string> words = new List<string>{};

            //Get word count in each document
            foreach(string doc in Documents)
            {
                words = Tokenize(doc);
                lengths.Add(words.Count);
            }

            //Sum all lengths into averageDocumentLength
            foreach(int length in lengths)
            {
                _averageDocumentLength += length;
            }

            //Take average
            _averageDocumentLength = _averageDocumentLength/lengths.Count;
            
        }

        protected override void ComputeIdf()
        {
            _docCount = Documents.Length;
            var docFrequency = ComputeDocFrequency();

            foreach (var term in docFrequency.Keys)
            {
                double idfValue = Math.Log(1 + (_docCount - docFrequency[term] + 0.5) / (docFrequency[term] + 0.5));
                Idf[term] = idfValue;
            }
        }

        protected override Dictionary<string, double> ComputeVector(Dictionary<string, double> termFrequency)
        {
            //Make new bm25 vector
            var bm25 = new Dictionary<string, double>();

            double docLength = 0;

            foreach(var term in termFrequency.Keys)
            {
                docLength += termFrequency[term];
            }

            //Iterate through list of terms and calculate their tfidf
            foreach (var term in termFrequency.Keys)
            {
                if (Vocabulary.ContainsKey(term))
                {
                    double tf = termFrequency[term];
                    if (Idf.ContainsKey(term))
                    {
                        double idf = Idf[term];

                        // BM25 formula
                        double numerator = tf * (_k + 1);
                        double denominator = tf + _k * (1 - _b + _b * (docLength / _averageDocumentLength));
                        double bm25Value = idf * (numerator / denominator);

                        //Store BM25 score into dictionary
                        bm25[term] = bm25Value;
                    }
                }
            }

            //Return vector component
            return bm25;
        }

        public override List<Dictionary<string, double>> FitTransform(string[] documents)
        {
            throw new NotImplementedException();
        }
    }
}
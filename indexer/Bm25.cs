using System;
using System.Collections.Generic;
using System.Linq;

namespace Indexer
{
    public class Bm25: Indexer
    {
        double k = 1.5;
        double b = 0.75;
        double averageDocumentLength;
        int docCount;
        public Dictionary<string, double> Idf { get; private set; }
        public Bm25()
        {
            Idf = new Dictionary<string, double>();
            averageDocumentLength = 0;
        }
        public override void Fit(string[] documents)
        {
            Documents = documents;
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
                averageDocumentLength += length;
            }

            //Take average
            averageDocumentLength = averageDocumentLength/lengths.Count;
            
        }

        protected override void ComputeIdf()
        {
            docCount = Documents.Length;
            var docFrequency = ComputeDocFrequency();

            foreach (var term in docFrequency.Keys)
            {
                double idfValue = Math.Log(1 + (docCount - docFrequency[term] + 0.5) / (docFrequency[term] + 0.5));
                Idf[term] = idfValue;
            }
        }

        public override List<Dictionary<string, double>> Transform(string[] documents)
        {
            throw new NotImplementedException();
        }

        public override List<Dictionary<string, double>> FitTransform(string[] documents)
        {
            throw new NotImplementedException();
        }
    }
}
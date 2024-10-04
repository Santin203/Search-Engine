using System;

namespace Indexer
{
    public abstract class SearchEngine
    {
        private double _similarityMeasure;

        public abstract double ComputeSimilarity(double[] vectorA, double[] vectorB);
    }
}
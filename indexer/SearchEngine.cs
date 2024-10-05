using System;

namespace Indexer
{
    public abstract class SearchEngine
    {
        protected double _similarityMeasure;

        protected void checkVectorLength(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                Console.WriteLine("Error: Vector lengths do not match.");
            }

        }

        public abstract double ComputeSimilarity(double[] vectorA, double[] vectorB);
    }
}
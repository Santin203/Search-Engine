using System;

namespace Indexer
{
    public abstract class SearchEngine
    {
        protected double _similarityMeasure;

        protected bool checkVectorLength(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                Console.WriteLine("Error: Vector lengths do not match.");
                return false;
            }
            return true;
        }

        public abstract double ComputeSimilarity(double[] vectorA, double[] vectorB);
    }
}
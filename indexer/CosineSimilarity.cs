using System;

namespace Indexer
{
    public class CosineSimilarity : SearchEngine
    {
        private double _similarityMeasure;

        public double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("Vectors must have the same length.");
            }

            double dotProduct = 0;
            double normA = 0;
            double normB = 0;

            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                normA += Math.Pow(vectorA[i], 2);
                normB += Math.Pow(vectorB[i], 2);
            }

            return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
        }


    }
}
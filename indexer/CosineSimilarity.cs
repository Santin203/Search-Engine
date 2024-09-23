using System;
using System.ComponentModel.DataAnnotations;

namespace Indexer
{
    public class CosineSimilarity : SearchEngine
    {
        private double _similarityMeasure;

        private void checkVectorLength(double[] vectorA, double[] vectorB)
        {
            if (vectorA.Length != vectorB.Length)
            {
                Console.WriteLine("Error: Vector lengths do not match.");
            }

        }

        private double computeMagnitude(double[] vector)
        {
            double magnitude = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                magnitude += Math.Pow(vector[i], 2);
            }
            return Math.Sqrt(magnitude);
        }

        private double computeDotProduct(double[] vectorA, double[] vectorB)
        {
            double dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }
            return dotProduct;
        }

        public double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            checkVectorLength(vectorA, vectorB);
            double dotProduct = computeDotProduct(vectorA, vectorB);
            double magnitudeA = computeMagnitude(vectorA);
            double magnitudeB = computeMagnitude(vectorB);
            _similarityMeasure = dotProduct / (magnitudeA * magnitudeB);
            return _similarityMeasure;
        }

    }
}
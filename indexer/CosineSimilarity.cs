using System;
using System.ComponentModel.DataAnnotations;

namespace Indexer
{
    public class CosineSimilarity : SearchEngine
    {
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

        public override double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            bool isSameLength = checkVectorLength(vectorA, vectorB);

            if(isSameLength)
            {
                double dotProduct = computeDotProduct(vectorA, vectorB);
                double magnitudeA = computeMagnitude(vectorA);
                double magnitudeB = computeMagnitude(vectorB);
                _similarityMeasure = dotProduct / (magnitudeA * magnitudeB);
            }
            return _similarityMeasure;
        }

    }
}
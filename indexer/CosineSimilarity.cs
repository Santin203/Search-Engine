namespace Indexer
{
    public class CosineSimilarity : SearchEngine
    {
        // Method to compute the magnitude of a vector
        private double ComputeMagnitude(double[] vector)
        {
            double magnitude = 0;
            for (int i = 0; i < vector.Length; i++)
            {
                magnitude += Math.Pow(vector[i], 2);  // Sum of squares of each component
            }
            return Math.Sqrt(magnitude);
        }


        private double ComputeDotProduct(double[] vectorA, double[] vectorB)
        {
            double dotProduct = 0;
            for (int i = 0; i < vectorA.Length; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
            }
            return dotProduct;
        }

        // Override method to compute cosine similarity between two vectors
        public override double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            // Ensure both vectors have the same length
            bool isSameLength = vectorA.Length == vectorB.Length;
            if (!isSameLength)
            {
                throw new ArgumentException("Vectors must have the same length for cosine similarity.");
            }

            // Compute dot product and magnitudes
            double dotProduct = ComputeDotProduct(vectorA, vectorB);
            double magnitudeA = ComputeMagnitude(vectorA);
            double magnitudeB = ComputeMagnitude(vectorB);

            // Guard against division by zero
            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            // Compute and return cosine similarity
            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}

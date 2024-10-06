using System;
using System.ComponentModel.DataAnnotations;

namespace Indexer
{
    public class Searchito : SearchEngine
    {
        //Convert vector component to points in a function, compute correlation of both sets of points
        public override double ComputeSimilarity(double[] vectorA, double[] vectorB)
        {
            bool isSameLength = checkVectorLength(vectorA, vectorB);

            if(isSameLength)
            {
                //Represent vectors as points in a linear function, a = 10, vector[index] = x
                //Function of the form f(x) = ax + b, b = 0
                double[] functionA = GetFunction(vectorA, vectorA.Length);
                double[] functionB = GetFunction(vectorB, vectorB.Length);

                //Calculate correlation between both sets of points
                CalculatePearsonCorrelation(functionA,functionB);
            }
            return _similarityMeasure;
        }

        //Get function points
        protected double[] GetFunction(double [] vector, int vectorSize)
        {
            double[] function = new double[vectorSize];
            for(int i = 0; i < vectorSize; i++)
            {
                function[i] = 10*vector[i];
            }
            return function;
        }

        // Function to calculate Pearson correlation between two sets of points
        protected void CalculatePearsonCorrelation(double[] functionA, double[] functionB)
        {
            int n = functionA.Length;
            double sumX = 0, sumY = 0, sumX2 = 0, sumY2 = 0, sumXY = 0;

            //Compute values of correlation components 
            for (int i = 0; i < n; i++)
            {
                double x = functionA[i];
                double y = functionB[i];

                sumX += x;
                sumY += y;
                sumX2 += x * x;
                sumY2 += y * y;
                sumXY += x * y;
            }

            //Compute value of correlation numerator and denominator
            double numerator = n * sumXY - sumX * sumY;
            double denominator = Math.Sqrt((n * sumX2 - sumX * sumX) * (n * sumY2 - sumY * sumY));

            if (denominator == 0)   
            {
                _similarityMeasure = 0; //Handle division by 0
                return;
            } 

            // Additional check to prevent NaN
            if (double.IsNaN(numerator) || double.IsNaN(denominator))
            {
                _similarityMeasure = 0;
                return;
            }

            //Compute correlation
            _similarityMeasure = numerator / denominator;
            return;
        }
    }
}

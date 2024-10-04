using System;
using System.Collections.Generic;

namespace Indexer
{
    public class Bm25: Indexer
    {
        public Bm25()
        {
            
        }
        public override void Fit(string[] documents)
        {
            throw new NotImplementedException();
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
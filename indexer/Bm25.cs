namespace Indexer
{
    public class Bm25: Indexer
    {
        // BM25 k parameter for term frequency saturation
        double k;

         // BM25 b parameter for document length normalization
        double b; 
        double averageDocumentLength;
        int docCount;
        public Dictionary<string, double> Idf { get; private set; }

        public Bm25()
        {
            k = 1.5; 
            b = 0.75;
            Idf = new Dictionary<string, double>();
            averageDocumentLength = 0;
        }

        // Processing a set of documents
        public override void Fit(string[] documents)
        {
            Documents = documents;

            if (Documents.Length == 0)
            {
                averageDocumentLength = 0;
            }
            else
            {
                // Calculate the average document length for the provided documents
                SetAverageDocumentLength();

                // Build the vocabulary from the given documents (terms extraction)
                BuildVocabulary(documents);

                // Compute the Inverse Document Frequency (IDF) values for the terms
                ComputeIdf();
            }
        }

        // Get average document length
        public double GetAverageDocumentLength()
        {
            return averageDocumentLength;
        }

        // Calculate and set the average length of documents
        protected void SetAverageDocumentLength()
        {
            List<int> lengths = new List<int>{};
            List<string> words = new List<string>{};

            // Get word count in each document
            foreach(string doc in Documents)
            {
                words = Tokenize(doc);
                lengths.Add(words.Count);
            }

            // Sum all lengths into averageDocumentLength
            foreach(int length in lengths)
            {
                averageDocumentLength += length;
            }

            // Take average document length
            averageDocumentLength = averageDocumentLength / lengths.Count;
        }

        // Compute Inverse Document Frequency (IDF) for each term
        protected override void ComputeIdf()
        {
            docCount = Documents.Length;
            var docFrequency = ComputeDocFrequency(); // Document frequency for each term

            // Calculate IDF for each term
            foreach (var term in docFrequency.Keys)
            {
                // BM25 IDF formula
                double idfValue = Math.Log(1 + (docCount - docFrequency[term] + 0.5) / (docFrequency[term] + 0.5));
                Idf[term] = idfValue;
            }
        }

        // Compute BM25 vector for a document
        protected override Dictionary<string, double> ComputeVector(List<string> words)
        {
            var bm25 = new Dictionary<string, double>();

            // Calculate raw term frequency for the document
            var termFrequency = CalculateTermFrequency(words);

            double docLength = 0;

            // Calculate total document length based on term frequency
            foreach(var term in termFrequency.Keys)
            {
                docLength += termFrequency[term];
            }

            // Iterate through list of terms and calculate their BM25 score
            foreach (var term in termFrequency.Keys)
            {
                if (Vocabulary.ContainsKey(term))
                {
                    double tf = termFrequency[term];

                    if (Idf.ContainsKey(term)) // Check if IDF for term is available
                    {
                        double idf = Idf[term];

                        // BM25 formula
                        double numerator = tf * (k + 1);
                        double denominator = tf + k * (1 - b + b * (docLength / averageDocumentLength));
                        double bm25Value = idf * (numerator / denominator);

                        bm25[term] = bm25Value;
                    }
                }
            }

            return bm25;
        }

        // Calculate the term frequency (TF) for each word in a document
        protected Dictionary<string, int> CalculateTermFrequency(List<string> words)
        {
            Dictionary<string, int> termFrequency = new Dictionary<string, int>();

            // Iterate over each word in the document
            foreach (var word in words)
            {
                // Check if the word is already in the dictionary
                if (termFrequency.ContainsKey(word))
                {
                    termFrequency[word]++;
                }
                else
                {
                    termFrequency[word] = 1;
                }
            }
            return termFrequency;
        }
    }
}

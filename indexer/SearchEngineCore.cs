using Newtonsoft.Json;

namespace Indexer
{
    public class SearchEngineCore
    {
        public Dictionary<string, Dictionary<string, double>> index { get; private set; } = new Dictionary<string, Dictionary<string, double>>();
        private Indexer vectorizer;

        private SearchEngine distance;

        public SearchEngineCore(Indexer vectorizer, SearchEngine distance)
        {
            this.vectorizer = vectorizer;
            this.distance = distance;
        }

        public void SetVectorizer(int vectorizerType)
        {
            switch(vectorizerType)
            {
                case 1:
                    vectorizer = new TFIDF();
                    break;
                case 2:
                    vectorizer = new Bm25();
                    break;
                default:
                    vectorizer = new TFIDF();
                    break;
            }
        }

        public void SetDistance(int distanceType)
        {
            switch(distanceType)
            {
                case 1:
                    distance = new CosineSimilarity();
                    break;
                case 2:
                    distance = new Searchito();
                    break;
                default:
                    distance = new CosineSimilarity();
                    break;
            }
        }

        // Index a folder of documents
        public void IndexFolder(string folderPath)
        {
            var fileProcessor = new FileHandler();
            var files = fileProcessor.GetFilesFromFolder(folderPath);
            var documents = new List<string>();

            // Read the content of each file and add it to the documents list
            foreach (var file in files)
            {
                var content = fileProcessor.ReadFileContent(file);
                if (!string.IsNullOrEmpty(content))
                {
                    documents.Add(content);
                }
            }

            // Fit the vectorizer to the documents
            var vectorMatrix = vectorizer.FitTransform(documents.ToArray());

            // Store the index data
            var indexData = files
            .Select((file, i) => new { Key = file, Value = vectorMatrix[i] })
            .ToDictionary(x => x.Key, x => x.Value);

            FileHandler.StoreIndex(indexData, vectorizer);
        }
        
        // Convert a dictionary to an array
        private double[] ToArray(Dictionary<string, double> vector)
        {
            var array = new double[vectorizer.Vocabulary.Count];
            foreach (var term in vector)
            {
                array[vectorizer.Vocabulary[term.Key]] = term.Value;
            }
            return array;
        }

        // Load the index from a file
        public void LoadIndex(string filePath)
        {
            // Read and deserialize the index from the file
            string jsonData = File.ReadAllText(filePath);
            index = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(jsonData) 
                ?? new Dictionary<string, Dictionary<string, double>>();

            vectorizer.Vocabulary = index.Values.SelectMany(v => v.Keys).Distinct().Select((term, i) => (term, i)).ToDictionary(p => p.term, p => p.i);

            Console.WriteLine("Index loaded successfully from " + filePath);
        }

        // Search for documents similar to a given query
        public List<string> Search(string query, int k)
        {
            // Vectorize the search query using the same vectorizer
            var queryVector = vectorizer.TransformToVector(new[] { query }).FirstOrDefault();

            // Check if the query vector is empty or null
            if (queryVector == null || queryVector.Count == 0)
            {
                Console.WriteLine("Query vector is empty. No results can be returned.");
                return new List<string>();
            }

            var rankedResults = new List<(string, double)>();

            // Iterate over each document in the index
            foreach (var docEntry in index)
            {
                var docVector = docEntry.Value;
                var docVectorArray = ToArray(docVector);
                var queryVectorArray = ToArray(queryVector);

                // Calculate the similarity
                double similarity = distance.ComputeSimilarity(queryVectorArray, docVectorArray);

                // Check for valid similarity scores
                if (!double.IsNaN(similarity) && !double.IsInfinity(similarity))
                {
                    rankedResults.Add((docEntry.Key, similarity));
                }
            }

            // Sort results by similarity and return top k results
            return rankedResults
                .OrderByDescending(result => result.Item2)
                .Take(k)
                .Select(result => result.Item1)  // Return only file paths
                .ToList();
        }

    }
}
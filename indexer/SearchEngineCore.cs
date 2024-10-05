using Newtonsoft.Json;

namespace Indexer
{
    public class SearchEngineCore
    {
        private Dictionary<string, Dictionary<string, double>> index = new Dictionary<string, Dictionary<string, double>>();
        private Indexer vectorizer;

        private SearchEngine distance;

        public SearchEngineCore(Indexer vectorizer, SearchEngine distance)
        {
            this.vectorizer = vectorizer;
            this.distance = distance;
        }

        public void IndexFolder(string folderPath)
        {
            var fileProcessor = new FileHandler();
            var files = fileProcessor.GetFilesFromFolder(folderPath);
            var documents = new List<string>();

            foreach (var file in files)
            {
            var content = fileProcessor.ReadFileContent(file);
            if (!string.IsNullOrEmpty(content))
            {
                documents.Add(content);
            }
            }

            var vectorMatrix = vectorizer.FitTransform(documents.ToArray());

            var indexData = files
            .Select((file, i) => new { Key = file, Value = vectorMatrix[i] })
            .ToDictionary(x => x.Key, x => x.Value);

            StoreIndex(indexData);
        }

        private double[] ToArray(Dictionary<string, double> vector)
        {
            var array = new double[vectorizer.Vocabulary.Count];
            foreach (var term in vector)
            {
                array[vectorizer.Vocabulary[term.Key]] = term.Value;
            }
            return array;
        }

        public void LoadIndex(string filePath)
        {
            // Read and deserialize the index from the file
            string jsonData = File.ReadAllText(filePath);
            index = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(jsonData) 
                ?? new Dictionary<string, Dictionary<string, double>>();

            vectorizer.Vocabulary = index.Values.SelectMany(v => v.Keys).Distinct().Select((term, i) => (term, i)).ToDictionary(p => p.term, p => p.i);

            Console.WriteLine("Index loaded successfully from " + filePath);
        }

        public static string StoreIndex(Dictionary<string, Dictionary<string, double>> indexData)
        {
            //Create a random number generator
            Random idGenerator = new Random();
            int id = idGenerator.Next(1000);

            //Save file in current directory
            string filePath  = Directory.GetCurrentDirectory() + "/Indexer" + id.ToString() + ".json";

            //Serialize data into a Json file
            string jsonSerialization = JsonConvert.SerializeObject(indexData, Formatting.Indented);
            
            //Write file
            File.WriteAllText(filePath, jsonSerialization);
            Console.WriteLine($"File written to {filePath}");
            return filePath;
        }

        public List<string> Search(string query, int k)
        {
            // Vectorize the search query using the same vectorizer
            var queryVector = vectorizer.Transform(new[] { query }).FirstOrDefault(); // Transform the query into a vector

            // Check if the query vector is empty or null
            if (queryVector == null || queryVector.Count == 0)
            {
                Console.WriteLine("Query vector is empty. No results can be returned.");
                return new List<string>(); // Return an empty list if the query vector is not valid
            }

            // Compute cosine similarity between the query and each document
            var rankedResults = new List<(string, double)>();

            foreach (var docEntry in index)
            {
                var docVector = docEntry.Value;
                var docVectorArray = ToArray(docVector);
                var queryVectorArray = ToArray(queryVector);

                // Calculate the cosine similarity
                double similarity = distance.ComputeSimilarity(queryVectorArray, docVectorArray);

                // Check for valid similarity scores
                if (!double.IsNaN(similarity) && !double.IsInfinity(similarity))
                {
                    rankedResults.Add((docEntry.Key, similarity));
                }
            }

            // Debugging output: Check the similarities calculated
            Console.WriteLine("Calculated Similarities:");
            foreach (var result in rankedResults)
            {
                Console.WriteLine($"Document: {result.Item1}, Similarity: {result.Item2}");
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
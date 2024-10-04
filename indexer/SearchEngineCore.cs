using Newtonsoft.Json;

namespace Indexer
{
    public class SearchEngineCore
    {
        private Dictionary<string, Dictionary<string, double>> index = new Dictionary<string, Dictionary<string, double>>();
        private TFIDF vectorizer;

        public SearchEngineCore()
        {
            vectorizer = new TFIDF();
        }

        public void IndexFolder(string folderPath)
        {
            // Get all files in the folder and subfolders
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            var documents = new List<string>();
            var filePaths = new List<string>();

            // Read and process all supported files
            foreach (var file in files)
            {
                Files? fileProcessor = null;

                // Check file extension and instantiate the correct file handler
                string extension = Path.GetExtension(file).ToLower();
                switch (extension)
                {
                    case ".txt":
                        fileProcessor = new TxtFiles();
                        break;
                    case ".csv":
                        fileProcessor = new CSVFiles();
                        break;
                    case ".json":
                        fileProcessor = new JsonFiles();
                        break;
                    case ".xml":
                        fileProcessor = new XmlFiles();
                        break;
                    case ".html":
                        fileProcessor = new HTMLFiles();
                        break;
                    case ".pdf":
                        fileProcessor = new PdfFiles();
                        break;
                    default:
                        Console.WriteLine($"Unsupported file type: {file}");
                        continue; // Skip unsupported file types
                }

                // Extract content from the file and build the index
                if (fileProcessor != null && fileProcessor.ExtractContent(file))
                {
                    Console.WriteLine($"Indexing file: {file}");
                    documents.Add(fileProcessor.fileData); // Add the extracted content
                    filePaths.Add(file); // Keep the file path for later reference
                }
            }

            // Compute TF-IDF for all documents
            var tfidfMatrix = vectorizer.FitTransform(documents.ToArray());

            // Store the indexed documents (TF-IDF vectors)
            for (int i = 0; i < filePaths.Count; i++)
            {
                index[filePaths[i]] = tfidfMatrix[i];
            }

            // Store the index in a file
            

            Console.WriteLine("Folder indexing complete.");
        }

        private bool IsSupportedFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".txt" || extension == ".csv" || extension == ".json" || extension == ".xml" || extension == ".html" || extension == ".pdf";
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

        public static List<(string, List<int>)> LoadIndex(string filePath)
        {
            //Create empty list
            List<(string indexedFilePath, List<int>)> readIndexer = new List<(string, List<int>)>{};  

            string jsonData = File.ReadAllText(filePath);
            readIndexer = JsonConvert.DeserializeObject<List<(string, List<int>)>>(jsonData) ?? new List<(string, List<int>)>();

            return readIndexer;
        }
        public static string StoreIndex(List<(string filePath, List<int> vector)> indexData)
        {
            //Create a random number generator
            Random idGenerator = new Random();
            int id = idGenerator.Next(1000);

            //Generate a file path
            string filePath  = "Indexer" + id.ToString() + ".json";

            //Serialize data into a Json file
            string jsonSerialization = JsonConvert.SerializeObject(indexData, Formatting.Indented);
            
            //Write file
            File.WriteAllText(filePath, jsonSerialization);
            Console.WriteLine($"File written to {filePath}");
            return filePath;
        }

        public List<string> Search(string query, int k)
        {
            // Vectorize the search query using the same TF-IDF vectorizer
            var queryVector = vectorizer.Transform(new[] { query }).FirstOrDefault(); // Transform the query into a TF-IDF vector

            // Check if the query vector is empty or null
            if (queryVector == null || queryVector.Count == 0)
            {
                Console.WriteLine("Query vector is empty. No results can be returned.");
                return new List<string>(); // Return an empty list if the query vector is not valid
            }

            // Compute cosine similarity between the query and each document
            var cosineSimilarity = new CosineSimilarity();
            var rankedResults = new List<(string, double)>();

            foreach (var docEntry in index)
            {
                var docVector = docEntry.Value;
                var docVectorArray = ToArray(docVector);
                var queryVectorArray = ToArray(queryVector);

                // Calculate the cosine similarity
                double similarity = cosineSimilarity.ComputeSimilarity(queryVectorArray, docVectorArray);

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
namespace Indexer
{
    public class SearchEngineCore
    {
        private Dictionary<string, Dictionary<string, double>> _index = new Dictionary<string, Dictionary<string, double>>();
        private TfIdfVectorizer _vectorizer;

        public SearchEngineCore()
        {
            _vectorizer = new TfIdfVectorizer();
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
                Files fileProcessor = null;

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
            var tfidfMatrix = _vectorizer.FitTransform(documents.ToArray());

            // Store the indexed documents (TF-IDF vectors)
            for (int i = 0; i < filePaths.Count; i++)
            {
                _index[filePaths[i]] = tfidfMatrix[i];  // Map the file path to its TF-IDF vector
            }

            Console.WriteLine("Folder indexing complete.");
        }

        private bool IsSupportedFileType(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();
            return extension == ".txt" || extension == ".csv" || extension == ".json" || extension == ".xml" || extension == ".html";
        }

        private double[] ToArray(Dictionary<string, double> vector)
        {
            var array = new double[_vectorizer.Vocabulary.Count];
            foreach (var term in vector)
            {
                array[_vectorizer.Vocabulary[term.Key]] = term.Value;
            }
            return array;
        }

        public void LoadIndex(string indexPath)
        {
            // Logic to load an existing index
        }

        public List<string> Search(string query, int k)
        {
            // Vectorize the search query using the same TF-IDF vectorizer
            var queryVector = _vectorizer.Transform(new[] { query }).FirstOrDefault(); // Transform the query into a TF-IDF vector

            // Check if the query vector is empty or null
            if (queryVector == null || queryVector.Count == 0)
            {
                Console.WriteLine("Query vector is empty. No results can be returned.");
                return new List<string>(); // Return an empty list if the query vector is not valid
            }

            // Compute cosine similarity between the query and each document
            var cosineSimilarity = new CosineSimilarity();
            var rankedResults = new List<(string, double)>();

            foreach (var docEntry in _index)
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
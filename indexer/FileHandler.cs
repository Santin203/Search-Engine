using Newtonsoft.Json;

namespace Indexer
{
    public class FileHandler
    {
        public static string StoreIndex(Dictionary<string, Dictionary<string, double>> indexData, Indexer vectorizer)
        {
            //Create a random number generator
            Random idGenerator = new Random();
            int id = idGenerator.Next(1000);

            //Get the name of the indexer
            string indexerName = vectorizer.GetType().Name;
            string fileName = $"{indexerName}{id}.json";

            // Save file in current directory
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            //Serialize data into a Json file
            string jsonSerialization = JsonConvert.SerializeObject(indexData, Formatting.Indented);
            
            // Write the file to disk and handle potential I/O exceptions
            try
            {
                File.WriteAllText(filePath, jsonSerialization);
                Console.WriteLine($"File written to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
            }
            
            return filePath;
        }

        public IEnumerable<string> GetFilesFromFolder(string folderPath)
        {
            // Make sure the folder exists before getting files
            if (Directory.Exists(folderPath))
            {
                return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            }
            else
            {
                Console.WriteLine("Folder does not exist.");
                return Enumerable.Empty<string>(); // Return an empty list if folder is invalid
            }
        }

        public string ReadFileContent(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            Files? fileProcessor = null;

            switch (extension)
            {
                case ".txt":
                    fileProcessor = new TxtFile();
                    break;
                case ".csv":
                    fileProcessor = new CSVFile();
                    break;
                case ".json":
                    fileProcessor = new JsonFile();
                    break;
                case ".xml":
                    fileProcessor = new XmlFile();
                    break;
                case ".html":
                    fileProcessor = new HTMLFile();
                    break;
                case ".pdf":
                    fileProcessor = new PdfFile();
                    break;
                default:
                    Console.WriteLine($"Unsupported file type: {extension}");
                    return string.Empty;
            }

            // Safely extract content and handle potential issues with file reading
            try
            {
                return fileProcessor.ExtractContent(filePath) ? fileProcessor.fileData : string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
using System.IO;
using System;
using Newtonsoft.Json;

namespace Indexer
{
    public class FileHandler
    {
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

        public IEnumerable<string> GetFilesFromFolder(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
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
                    break;
            }

            return fileProcessor?.ExtractContent(filePath) == true ? fileProcessor.fileData : string.Empty;
        }
    }
}
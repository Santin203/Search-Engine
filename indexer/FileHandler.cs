using System.IO;
using System;
using Newtonsoft.Json;

namespace Indexer
{
    public class FileHandler
    {
        public static string StoreIndex(List<Files> fileList)
        {
            //Create a random number generator
            Random idGenerator = new Random();
            int id = idGenerator.Next(1000);

            //Generate a file path
            string filePath  = "Indexer" + id.ToString() + ".json";
            string jsonSerialization = string.Empty;

            //Serialize data into a Json file
            jsonSerialization = JsonConvert.SerializeObject(fileList, Formatting.Indented);
            
            //Write file
            File.WriteAllText(filePath, jsonSerialization);
            Console.WriteLine($"File written to {filePath}");
            return filePath;
        }

        public static List<string> GetPaths(string path)
        {
            //Make list to store file paths
            List<string> filePaths = new List<string>{};

            string currentDirectory = path;
            string[] paths;
            string[] dirs;

            //If path == "", first time executing, look at current directory
            if(currentDirectory == "")
            {
                currentDirectory = Directory.GetCurrentDirectory();
            }

            //Get files from current directory
            paths = Directory.GetFiles(currentDirectory);

            //Convert array into list, store in filePaths
            filePaths = paths.ToList();
            
            //Get subdirectories
            dirs = Directory.GetDirectories(currentDirectory);

            //If 1 or more subdirectories
            if(dirs.Length != 0)
            {
                //Iterate through list
                foreach(string dir in dirs)
                {
                    filePaths.AddRange(GetPaths(dir));
                }
            }
            return(filePaths);
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
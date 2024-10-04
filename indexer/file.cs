using System;
using HtmlAgilityPack;
using UglyToad.PdfPig;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Xml;
using Porter2Stemmer;
using System.Text.Json;

namespace Indexer
{
    public abstract class Files
    {
        public string fileData;

        //Constructor for Files class, default values included
        public Files(string data = "")
        {
            fileData = data;
        }

        //Build instance of the Current Class
        public bool ExtractContent(string filePath)
        {
            Console.WriteLine("File build has started.");

            //Read data from file
            this.fileData = this.GetFileData(filePath);

            //File found, raw data stored
            if(fileData != null)
            {
                Console.WriteLine("File data read successfully!");

                string[] words = fileData.Split(" ");
                words = StemWords(words);
                
                fileData = string.Join(" ", words);
                Console.WriteLine("Words have been stemmed");

                return true;
            }
            //File not found
            else
            {
                Console.WriteLine("File data could not be read.");
                return false;
            }
        }

        //Get file name from user, return data
        protected string GetFileData(string filePath)
        {

            string fileData = string.Empty;


            //Try to read file
            try
            {
                fileData = GetRawText(filePath);
            }
            //File was not found
            catch(FileNotFoundException)
            {
                Console.WriteLine($"File {filePath} not found.");
            }
            //IO error
            catch (IOException ex)
            {
                Console.WriteLine($"An I/O error occurred: {ex.Message}");
            }
            
            return fileData ?? string.Empty;
        }

        protected abstract string GetRawText(string filePath);
        
        protected string RemoveBadChars(string rawText)
        {
            string[] specialChars = { "@", "#", "!", "$", ",", ".", ";", "(", ")", "[", "]", "{", "}", "\"", "'", "\r", "\n" };

            // Replace each special character with an empty string
            foreach (string specialChar in specialChars)
            {
                rawText = rawText.Replace(specialChar, " ");
            }
            return rawText;
        }
        protected string[] StemWords(string[] rawStrings)
        {
            // Make new instance of stemmer
            var stemmer = new EnglishPorter2Stemmer();

            // Traverse all strings in array
            for(int i = 0; i < rawStrings.Length; i++)
            {
                // Stem word in current i index
                var stemmed = stemmer.Stem(rawStrings[i]);

                // Assign stemmed word to i index
                rawStrings[i] = stemmed.Value;
            }
            return rawStrings;
        }
    }

    public class TxtFiles : Files
    {
        public TxtFiles()
        {
        }

        // Constructor for TxtFiles that calls the base constructor
        public TxtFiles(string data) 
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            // Read all text from file
            fileData = File.ReadAllText(filePath);

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class PdfFiles : Files
    {
        public PdfFiles()
        {
        }

        public PdfFiles(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string pageData;

            string fileData = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                // Iterate through pages
                foreach (var page in pdf.GetPages())
                {
                    // Raw text of the page's content stream.
                    pageData = page.Text;

                    // Concatenate with previous data collected
                    fileData = string.Join(" ", fileData, pageData);
                }
            }

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class HTMLFiles : Files
    {
        public HTMLFiles()
        {
        }

        public HTMLFiles(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            // Load the HTML file
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(filePath);

            // Extract the plain text content from the HTML
            string fileData = htmlDoc.DocumentNode.InnerText;

            // Remove unwanted special characters from the text
            string cleanedText = this.RemoveBadChars(fileData);

            // Return the cleaned plain text content
            return cleanedText;
        }
    }



    public class JsonFiles : Files
    {
        public JsonFiles()
        {
        }

        public JsonFiles(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            // Read the JSON file
            string fileData = File.ReadAllText(filePath);

            // Parse the JSON and extract the content (keys and values)
            Dictionary<string, object> jsonData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(fileData);
            List<string> keyValues = ExtractKeyValues(jsonData);

            // Join all keys and values into a single string
            string contentWithKeys = string.Join(" ", keyValues);

            // Remove any unwanted characters and return
            return this.RemoveBadChars(contentWithKeys);
        }

        private List<string> ExtractKeyValues(Dictionary<string, object> jsonData)
        {
            var keyValues = new List<string>();

            foreach (var entry in jsonData)
            {
                string key = entry.Key;

                if (entry.Value is JsonElement jsonElement)
                {
                    switch (jsonElement.ValueKind)
                    {
                        case JsonValueKind.Object:
                            // Recursively extract keys and values from nested objects
                            var nestedData = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
                            keyValues.Add(key);
                            keyValues.AddRange(ExtractKeyValues(nestedData));
                            break;
                        case JsonValueKind.Array:
                            // Extract values from arrays and append the key
                            keyValues.Add(key);
                            foreach (var element in jsonElement.EnumerateArray())
                            {
                                if (element.ValueKind == JsonValueKind.String)
                                {
                                    keyValues.Add(element.GetString());
                                }
                                else
                                {
                                    keyValues.Add(element.ToString());
                                }
                            }
                            break;
                        case JsonValueKind.String:
                            keyValues.Add(key);
                            keyValues.Add(jsonElement.GetString());
                            break;
                        case JsonValueKind.Number:
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                        case JsonValueKind.Null:
                            keyValues.Add(key);
                            keyValues.Add(jsonElement.ToString());
                            break;
                    }
                }
                else
                {
                    // For non-JsonElement values (if deserialization resulted in other object types)
                    keyValues.Add(key);
                    keyValues.Add(entry.Value.ToString());
                }
            }

            return keyValues;
        }
    }



    public class XmlFiles : Files
    {
        public XmlFiles()
        {
        }

        public XmlFiles(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);

            // Access the root element
            XmlElement root = xmlDoc.DocumentElement;

            // Recursively process each node and concatenate tag names and text content
            fileData = ExtractContent(root);

            return fileData;
        }

        private string ExtractContent(XmlNode node)
        {
            string result = "";

            // Only process element nodes, avoid things like #text nodes and XML declarations
            if (node.NodeType == XmlNodeType.Element)
            {
                // Add the tag name
                result += node.Name + " ";

                // If the element has inner text, add it
                if (!string.IsNullOrWhiteSpace(node.InnerText) && node.ChildNodes.Count == 1 && node.FirstChild is XmlText)
                {
                    result += node.InnerText + " ";
                }

                // Recursively process child nodes
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    result += ExtractContent(childNode);
                }
            }

            return result;
        }
    }





    public class CSVFiles : Files
    {
        public CSVFiles()
        {
        }

        public CSVFiles(string data)
            : base(data)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            fileData = File.ReadAllText(filePath);
            fileData = this.RemoveBadChars(fileData);

            return fileData;
        }
    }
}

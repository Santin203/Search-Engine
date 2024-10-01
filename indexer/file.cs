using System;
using HtmlAgilityPack;
using UglyToad.PdfPig;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Xml;
using Porter2Stemmer;

namespace Indexer
{
    public abstract class Files
    {
        public string fileData;
        public int nTerms;
        public List<(string term, int frequency)> terms; // Change from array to List<T>

        //Constructor for Files class, default values included
        public Files(string data = "", int termNumber = -1, List<(string term, int frequency)>? termsList = null)
        {
            fileData = data;
            nTerms = termNumber;
            terms = termsList ?? new List<(string, int)> { ("", -1) };  // Initialize if termsList is null
        }

        //Build instance of the Current Class
        public bool ExtractContent()
        {
            Console.WriteLine("File build has started.");

            //Read data from file
            this.fileData = this.GetFileData();

            //File found, raw data stored
            if(fileData != null)
            {
                Console.WriteLine("File data read successfully!");
                terms = ParseData(fileData);

                //Parsing of fileData successful
                if(terms[0].term != "")
                {
                    Console.WriteLine("File data parsed successfully!");
                    nTerms = terms.Count; // Change from Length to Count
                    return true;
                }
                //Error in parsing fileData
                else
                {
                    Console.WriteLine("File data could not be parsed.");
                    return false;
                }
            }
            //File not found
            else
            {
                Console.WriteLine("File data could not be read.");
                return false;
            }
        }

        //Get file name from user, return data
        protected string GetFileData()
        {
            Console.WriteLine("Please write the name of the file you want to read from: ");
            string fileName = Console.ReadLine() ?? string.Empty;

            string fileData = string.Empty;

            if(File.Exists(fileName))
            {
                //Try to read file
                try
                {
                    fileData = GetRawText(fileName);
                }
                //File was not found
                catch(FileNotFoundException)
                {
                    Console.WriteLine($"File {fileName} not found in current directory.");
                }
                //IO error
                catch (IOException ex)
                {
                    Console.WriteLine($"An I/O error occurred: {ex.Message}");
                }
            }
            return fileData ?? string.Empty;
        }

        protected abstract string GetRawText(string filePath);
        
        protected string RemoveBadChars(string rawText)
        {
            string[] specialChars = { "@", "#", "!", "$", ",", ".", ";", "(", ")", "[", "]", "{", "}", "\"", "'" };

            // Replace each special character with an empty string
            foreach (string specialChar in specialChars)
            {
                rawText = rawText.Replace(specialChar, "");
            }
            return rawText;
        }

        protected List<(string term, int frequency)> ParseData(string rawData)
        {
            // Initialize list to dummy value
            List<(string term, int frequency)> terms = new List<(string, int)>
            {
                ("", -1)
            };

            // Create tuple for copying and editing values
            (string term, int frequency) prevTuple;

            // Helper variables
            int n;
            bool found;

            // Write code for stemming algorithm here.
            string[] rawWords = rawData.Split(" ");
            rawWords = StemmWords(rawWords);

            // Traverse array of stemmed words and count the number of appearances
            foreach(string word in rawWords)
            {
                // If list still hasn't been initialized with real values
                if(terms.Count == 1 && terms[0].term == "")
                {
                    terms[0] = (word, 1);
                }
                // Else traverse list and search for current word
                else
                {
                    found = false;
                    for(int j = 0; j < terms.Count; j++)
                    {
                        // If word is inside current tuple, increase frequency
                        if(terms[j].term == word)
                        {
                            prevTuple = terms[j];

                            n = prevTuple.frequency;

                            terms[j] = (prevTuple.term, n + 1);
                            found = true;
                            break; // Break after updating to avoid unnecessary iterations
                        }
                    }
                    // Else add to list
                    if(!found)
                    {
                        terms.Add((word, 1));
                    }
                }
            }

            return terms;
        }

        protected string[] StemmWords(string[] rawStrings)
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
        // Constructor for TxtFiles that calls the base constructor
        public TxtFiles(string data, int termNumber, List<(string term, int frequency)> termsList) 
            : base(data, termNumber, termsList)
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
        public PdfFiles(string data, int termNumber, List<(string term, int frequency)> termsList)
            : base(data, termNumber, termsList)
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
        public HTMLFiles(string data, int termNumber, List<(string term, int frequency)> termsList)
            : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";
            // Make new class to store html doc
            HtmlDocument htmlDoc = new HtmlDocument();

            // Load file into object
            htmlDoc.Load(filePath);

            // Extract text content from html file
            fileData = htmlDoc.DocumentNode.InnerText;

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class JsonFiles : Files
    {
        public JsonFiles(string data, int termNumber, List<(string term, int frequency)> termsList)
            : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";

            // Read file text directly
            fileData = File.ReadAllText(filePath);

            // Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class XmlFiles : Files
    {
        public XmlFiles(string data, int termNumber, List<(string term, int frequency)> termsList)
            : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("path_to_xml_file.xml");

            // Access the root element
            XmlElement root = xmlDoc.DocumentElement;

            // Join the name of the root element twice (opening and closing tag) to the empty data string.
            string.Join(" ", fileData, root.Name, root.Name);

            // Iterate over all child nodes of the root element
            foreach (XmlNode node in root.ChildNodes)
            {
                // Join the name of the element twice (opening and closing tag) and inner text to the collected data.
                string.Join(" ", fileData, node.Name, node.Name);
                string.Join(" ", fileData, node.InnerText);
            }

            return fileData;
        }
    }

    public class CSVFiles : Files
    {
        public CSVFiles(string data, int termNumber, List<(string term, int frequency)> termsList)
            : base(data, termNumber, termsList)
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

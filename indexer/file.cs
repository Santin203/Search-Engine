using System;
using HtmlAgilityPack;
using UglyToad.PdfPig;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Xml;

namespace Indexer
{
    public abstract class Files
    {
        public string fileData;
        public int nTerms;
        public (string term, int frequency)[] terms;

        //Constructor for Files class, default values included
        public Files(string data = "", int termNumber = -1, (string term, int frequency)[]? termsList = null)
        {
            fileData = data;
            nTerms = termNumber;
            terms = termsList ?? new (string, int)[1] { ("", -1) };  // Initialize if termsList is null
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
                Console.WriteLine("File data read succesfully!");
                terms = ParseData(fileData);

                //Parsing of fileData succesfull
                if(terms[0].term != "")
                {
                    Console.WriteLine("File data parsed succesfully!");
                    nTerms = terms.Length;
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
                //Try read file
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
        protected (string term, int frequency)[] ParseData(string rawData)
        {
            //Intialize tuple array to dummy value
            (string term, int frequency)[] terms = new (string, int)[1]{("",-1)};
            int substringStart = 0;
            int substringEnd = 0;
            //Write code for stemming algorithm here.
            for(int i = 0; i < rawData.Length; i++)
            {

            }

            return(terms);
        }

        protected string StemmWord(string rawString)
        {
            return rawString;
        }
    }

    public class TxtFiles: Files
    {
        // Constructor for PdfFiles that calls the base constructor
        public TxtFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData;

            //Read all text from file
            fileData = File.ReadAllText(filePath);

            //Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class PdfFiles: Files
    {
        public PdfFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string pageData;

            string fileData = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                //Iterate through pages
                foreach (var page in pdf.GetPages())
                {
                    //raw text of the page's content stream.
                    pageData = page.Text;

                    //Concatinate with previous data collected
                    fileData = string.Join(" ",fileData, pageData);
                }
            }

            //Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class HTMLFiles: Files
    {
        public HTMLFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";
            //Make new class to store html doc
            HtmlDocument htmlDoc = new HtmlDocument();

            //Load file into object
            htmlDoc.Load(filePath);

            //Extract text content from html file
            fileData = htmlDoc.DocumentNode.InnerText;

            //Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class JsonFiles: Files
    {
        public JsonFiles(string data, int termNumber, (string term, int frequency)[] termsList)
        : base(data, termNumber, termsList)
        {
        }

        protected override string GetRawText(string filePath)
        {
            string fileData = "";

            //Read file text directly
            fileData = File.ReadAllText(filePath);

            //Remove special chars from string and return
            this.RemoveBadChars(fileData);
            return fileData;
        }
    }

    public class XmlFiles: Files
    {
        public XmlFiles(string data, int termNumber, (string term, int frequency)[] termsList)
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

            //Join the name of the root element twice (opening and closing tag) to the empty data string.
            string.Join(" ", fileData, root.Name, root.Name);

            // Iterate over all child nodes of the root element
            foreach (XmlNode node in root.ChildNodes)
            {
                // Join the name of the element twice (opening and closing tag) and inner text to the collected data.
                string.Join(" ",fileData, node.Name, node.Name);
                string.Join(" ", fileData, node.InnerText);
            }

            return fileData;
        }
    }

    public class CSVFiles: Files
    {
        public CSVFiles(string data, int termNumber, (string term, int frequency)[] termsList)
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
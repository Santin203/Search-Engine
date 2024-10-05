using System.Xml;

namespace Indexer
{
        public class XmlFile : Files
    {
        public XmlFile()
        {
        }

        public XmlFile(string data)
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
}
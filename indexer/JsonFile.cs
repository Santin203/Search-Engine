using System.Text.Json;
using Newtonsoft.Json;

namespace Indexer
{
    public class JsonFile : Files
    {
        public JsonFile()
        {
        }

        public JsonFile(string data)
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
}
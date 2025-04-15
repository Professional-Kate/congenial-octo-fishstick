using System.Text.Json;

namespace ContentHydrator.Readers
{
    /// <inheritdoc cref="IReader"/>
    public class JsonFileReader() : IReader
    {
        public Dictionary<string, object> Read(string filePath)
        {
            string fileText = File.ReadAllText(filePath);
            JsonDocument document = JsonDocument.Parse(fileText);

            Dictionary<string, object>? objects = JsonSerializer.Deserialize<Dictionary<string, object>>(document.RootElement.GetRawText());
            if (objects == null)
            {
                throw new ArgumentNullException();
            }

            return objects;
        }
    }
}
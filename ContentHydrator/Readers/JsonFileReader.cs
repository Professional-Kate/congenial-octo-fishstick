using System.Text.Json;

namespace ContentHydrator.Readers
{
    /// <inheritdoc cref="IReader"/>
    public class JsonFileReader() : IReader
    {
        public Dictionary<string, object> Read(string filePath)
        {
            JsonDocument document = JsonDocument.Parse(File.ReadAllText(filePath));

            return JsonSerializer.Deserialize<Dictionary<string, object>>(document.RootElement.GetRawText());
        }
    }
}
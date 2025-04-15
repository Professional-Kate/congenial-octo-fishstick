using System.Text.Json;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Readers
{
    /// <inheritdoc cref="IReader"/>
    public class JsonFileReader(IAssertNotNull assertNotNull) : IReader
    {
        public Dictionary<string, object> Read(string filePath)
        {
            string fileText = File.ReadAllText(filePath);
            JsonDocument document = JsonDocument.Parse(fileText);
            
            Dictionary<string, object>? objects = JsonSerializer.Deserialize<Dictionary<string, object>>(document.RootElement.GetRawText());
            assertNotNull.AssertObjectNotNull(objects);
                
            return objects!;
        }
    }
}
using System.Text.Json;
using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Readers
{
    /// <inheritdoc cref="IJsonReader"/>
    public class JsonReader(IObjectNullAssertion objectNullAssertion) : IJsonReader
    {
        public JsonDocument Read(string filePath)
        {
            string fileText = File.ReadAllText(filePath);
            JsonDocument document = JsonDocument.Parse(fileText);

            JsonDocument? objects = JsonSerializer.Deserialize<JsonDocument>(document.RootElement.GetRawText());
            objectNullAssertion.AssertNotNull(objects, nameof(objects));

            return objects!;
        }
    }
}
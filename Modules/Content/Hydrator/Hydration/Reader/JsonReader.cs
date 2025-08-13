using System.Text.Json;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Content.Hydrator.Hydration.Reader
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
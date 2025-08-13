using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Content.Hydrator.Hydration.Converter
{
    public class JsonSourceConverter<T>(JsonTypeInfo<T> typeInfo, IObjectNullAssertion objectNullAssertion) : IJsonConverter<T>
    {
        public T Convert(JsonDocument jsonDocument)
        {
            T? deserializedObject = jsonDocument.Deserialize(typeInfo);

            objectNullAssertion.AssertNotNull(deserializedObject, nameof(jsonDocument));

            return deserializedObject!;
        }
    }
}
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Converters
{
    public class JsonSourceConverter<T>(JsonTypeInfo<T> typeInfo, IAssertNotNull assertNotNull) : IJsonConverter<T>
    {
        public T Convert(JsonDocument jsonDocument)
        {
            T? deserializedObject = jsonDocument.Deserialize(typeInfo);
            
            assertNotNull.AssertObjectNotNull(deserializedObject);
            
            return deserializedObject!;
        }
    }
}
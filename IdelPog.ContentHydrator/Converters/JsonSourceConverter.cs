using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Converters
{
    public class JsonSourceConverter<T>(JsonTypeInfo<T> typeInfo, IAssertNotNull assertNotNull) : IJsonConverter<T>
    {
        public T Convert(string jsonString)
        {
            T? deserializedObject = JsonSerializer.Deserialize(jsonString, typeInfo);
            
            assertNotNull.AssertObjectNotNull(deserializedObject);
            
            return deserializedObject!;
        }
    }
}
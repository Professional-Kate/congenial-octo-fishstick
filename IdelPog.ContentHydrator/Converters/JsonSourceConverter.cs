using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace ContentHydrator.Converters
{
    public class JsonSourceConverter<T>(JsonTypeInfo<T> typeInfo) : IJsonConverter<T>
    {
        public T Convert(string jsonString)
        {
            T newData = JsonSerializer.Deserialize(jsonString, typeInfo);
            
            return newData;
        }
    }
}
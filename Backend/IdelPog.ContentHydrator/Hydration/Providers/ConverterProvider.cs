using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ContentHydrator.Converters;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Providers
{
    public class ConverterProvider(JsonSerializerContext context, IAssertNotNull assertNotNull) : IConverterProvider
    {
        public IJsonConverter<T> CreateConverter<T>()
        {
            JsonTypeInfo? typeInfo = context.GetTypeInfo(typeof(T));
            
            assertNotNull.AssertObjectNotNull(typeInfo);
            
            return new JsonSourceConverter<T>((JsonTypeInfo<T>)typeInfo!, assertNotNull);
        }
    }
}
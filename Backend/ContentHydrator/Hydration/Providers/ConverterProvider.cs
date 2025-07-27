using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using IdelPog.ContentHydrator.Converters;
using IdelPog.Validation.Assertions;

namespace IdelPog.ContentHydrator.Providers
{
    public class ConverterProvider(JsonSerializerContext context, IObjectNullAssertion objectNullAssertion) : IConverterProvider
    {
        public IJsonConverter<T> CreateConverter<T>()
        {
            JsonTypeInfo? typeInfo = context.GetTypeInfo(typeof(T));

            objectNullAssertion.AssertNotNull(typeInfo, nameof(typeInfo));

            return new JsonSourceConverter<T>((JsonTypeInfo<T>)typeInfo!, objectNullAssertion);
        }
    }
}
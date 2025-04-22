using ContentHydrator.Converters;

namespace ContentHydrator.Providers
{
    public class ConverterProvider : IConverterProvider
    {
        public IJsonConverter<T> CreateConverter<T>()
        {
            throw new NotImplementedException();
        }
    }
}
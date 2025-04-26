using ContentHydrator.Converters;

namespace ContentHydrator.Providers
{
    public interface IConverterProvider
    {
        public IJsonConverter<T> CreateConverter<T>();
    }
}
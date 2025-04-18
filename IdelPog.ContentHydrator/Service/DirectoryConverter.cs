using ContentHydrator.Converters;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    /// <inheritdoc cref="IDirectoryConverter{T}"/>
    public class DirectoryConverter<T>(IJsonReader jsonReader, IJsonConverter<T> jsonConverter) : IDirectoryConverter<T>
    {
        private readonly IJsonReader _jsonReader = jsonReader;
        private readonly IJsonConverter<T> _jsonConverter = jsonConverter;

        public IEnumerable<T> ConvertDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }
    }
}
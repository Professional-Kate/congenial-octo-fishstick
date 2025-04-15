using ContentHydrator.Converters;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    /// <inheritdoc cref="IDirectoryConverter{T}"/>
    public class DirectoryConverter<T>(IReader reader, IConverter<T> converter) : IDirectoryConverter<T>
    {
        private readonly IReader _reader = reader;
        private readonly IConverter<T> _converter = converter;

        public IEnumerable<T> ConvertDirectory(string directoryPath)
        {
            throw new NotImplementedException();
        }
    }
}
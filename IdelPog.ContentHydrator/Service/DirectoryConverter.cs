using System.Text.Json;
using ContentHydrator.Converters;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    /// <inheritdoc cref="IDirectoryConverter{T}"/>
    public class DirectoryConverter<T>(IJsonReader jsonReader, IJsonConverter<T> jsonConverter) : IDirectoryConverter<T>
    {
        public IEnumerable<T> ConvertDirectory(string directoryPath)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(directoryPath);
            foreach (string file in files)
            {
                JsonDocument pairs = jsonReader.Read(file);
                jsonConverter.Convert(pairs.ToString());
            }
            
            throw new NotImplementedException();
        }
    }
}
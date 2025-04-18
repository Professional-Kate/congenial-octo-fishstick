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
            if (Directory.Exists(directoryPath) == false)
            {
                throw new DirectoryNotFoundException(directoryPath);
            }
            
            return EnumerateFiles(directoryPath);
        }

        private IEnumerable<T> EnumerateFiles(string directoryPath)
        {
            IEnumerable<string> files = Directory.EnumerateFiles(directoryPath);
            foreach (string file in files)
            {
                JsonDocument pairs = jsonReader.Read(file);
                T convertedDTO = jsonConverter.Convert(pairs);
                
                yield return convertedDTO;
            }
        }
    }
}
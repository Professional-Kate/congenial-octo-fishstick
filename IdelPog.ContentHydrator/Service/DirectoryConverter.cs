using System.Text.Json;
using ContentHydrator.Assertions.Pipelines;
using ContentHydrator.Converters;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    /// <inheritdoc cref="IDirectoryConverter{T}"/>
    public class DirectoryConverter<T>(IJsonReader jsonReader, IJsonConverter<T> jsonConverter, IDirectoryAsserter directoryAsserter) : IDirectoryConverter<T>
    {
        public IEnumerable<T> ConvertDirectory(string directoryPath)
        {
            directoryAsserter.AssertDirectory(directoryPath);
            
            string[] files = Directory.GetFiles(directoryPath);
            
            directoryAsserter.AssertFiles(files, directoryPath);
            
            return EnumerateFiles(files);
        }

        private IEnumerable<T> EnumerateFiles(string[] files)
        {
            foreach (string file in files)
            {
                JsonDocument document = jsonReader.Read(file);
                T convertedDTO = jsonConverter.Convert(document);
                
                yield return convertedDTO;
            }
        }
    }
}
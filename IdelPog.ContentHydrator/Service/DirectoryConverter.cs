using System.Text.Json;
using ContentHydrator.Assertions;
using ContentHydrator.Converters;
using ContentHydrator.Readers;
using IdelPog.Validation.Assertions;

namespace ContentHydrator.Service
{
    /// <inheritdoc cref="IDirectoryConverter{T}"/>
    public class DirectoryConverter<T>(IJsonReader jsonReader, IJsonConverter<T> jsonConverter, IAssertFound assertFound, IAssertDirectoryNotEmpty notEmpty) : IDirectoryConverter<T>
    {
        public IEnumerable<T> ConvertDirectory(string directoryPath)
        {
            assertFound.AssertItemIsFound(directoryPath, () => Directory.Exists(directoryPath));
            
            string[] files = Directory.GetFiles(directoryPath);
            notEmpty.AssertNotEmpty(files);
            
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
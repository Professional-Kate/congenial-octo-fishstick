using System.Text.Json;
using ContentHydrator.Assertions.Pipelines;
using ContentHydrator.Converters;
using ContentHydrator.Providers;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    public class DirectoryConverter(IJsonReader jsonReader, IConverterProvider provider, IDirectoryAsserter directoryAsserter) : IDirectoryConverter
    {
        public IEnumerable<T> ConvertDirectory<T>(string directoryPath)
        {
            directoryAsserter.AssertDirectory(directoryPath);
            
            string[] filePaths = Directory.EnumerateFiles(directoryPath, "*.json").ToArray();
            
            directoryAsserter.AssertFiles(filePaths, directoryPath);
            
            return EnumerateFiles<T>(filePaths);
        }

        private IEnumerable<T> EnumerateFiles<T>(string[] files)
        {
            IJsonConverter<T> converter = provider.CreateConverter<T>();
            
            foreach (string file in files)
            {
                using JsonDocument document = jsonReader.Read(file);
                T convertedDTO = converter.Convert(document);
                
                yield return convertedDTO;
            }
        }
    }
}
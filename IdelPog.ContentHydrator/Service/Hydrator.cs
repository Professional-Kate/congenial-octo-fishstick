using System.Text.Json;
using ContentHydrator.Converters;
using ContentHydrator.Providers;
using ContentHydrator.Readers;

namespace ContentHydrator.Service
{
    public class Hydrator(IJsonReader reader, IConverterProvider provider) : IHydrator
    {
        public IEnumerable<T> HydrateFrom<T>(string sourceDirectory)
        {
            IEnumerable<string> filePaths = Directory.EnumerateFiles(sourceDirectory, "*.json");
            IJsonConverter<T> converter = provider.CreateConverter<T>();
            
            foreach (string filePath in filePaths)
            {
                using JsonDocument document = reader.Read(filePath);
                T createdObject = converter.Convert(document);
                
                yield return createdObject;
            }
        }
    }
}
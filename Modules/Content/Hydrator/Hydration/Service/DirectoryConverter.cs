using System.Text.Json;
using IdelPog.Content.Hydrator.Assertion.Pipeline;
using IdelPog.Content.Hydrator.Hydration.Converter;
using IdelPog.Content.Hydrator.Hydration.Provider;
using IdelPog.Content.Hydrator.Hydration.Reader;

namespace IdelPog.Content.Hydrator.Hydration.Service
{
    /// <inheritdoc cref="IDirectoryConverter"/>
    public class DirectoryConverter(IJsonReader jsonReader, IConverterProvider provider, IDirectoryAssertionPipeline directoryAssertionPipeline)
        : IDirectoryConverter
    {
        public IEnumerable<T> ConvertDirectory<T>(string directoryPath)
        {
            directoryAssertionPipeline.AssertDirectory(directoryPath);

            string[] filePaths = Directory.EnumerateFiles(directoryPath, "*.json").ToArray();

            directoryAssertionPipeline.AssertFiles(filePaths.Length, directoryPath);

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
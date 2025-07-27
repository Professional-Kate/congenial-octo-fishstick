using System.Text.Json;
using IdelPog.ContentHydrator.Assertions.Pipelines;
using IdelPog.ContentHydrator.Converters;
using IdelPog.ContentHydrator.Providers;
using IdelPog.ContentHydrator.Readers;

namespace IdelPog.ContentHydrator.Service
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
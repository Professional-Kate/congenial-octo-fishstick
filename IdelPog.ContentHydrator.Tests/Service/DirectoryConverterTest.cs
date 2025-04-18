using System.Text.Json;
using ContentHydrator.Converters;
using ContentHydrator.Readers;
using ContentHydrator.Service;
using ContentHydratorTests.TestObjects;
using Moq;

namespace ContentHydratorTests.Service
{
    [TestFixture]
    public class DirectoryConverterTest
    {
        private IDirectoryConverter<TestDTO> _directoryConverter { get; set; }
        private Mock<IJsonReader> _jsonReaderMock { get; set; }
        private Mock<IJsonConverter<TestDTO>> _jsonConverterMock { get; set; }

        private const string DIRECTORY_PATH = "Resources/DirectoryConverterFiles";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _jsonReaderMock = new Mock<IJsonReader>();
            _jsonConverterMock = new Mock<IJsonConverter<TestDTO>>();
            _directoryConverter = new DirectoryConverter<TestDTO>(_jsonReaderMock.Object, _jsonConverterMock.Object);
        }

        [Test]
        public void Positive_ConvertDirectory_ConvertsWholeDirectory()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Returns<string>(path =>
                {
                    string jsonStrong = File.ReadAllText(path);
                    JsonDocument json = JsonDocument.Parse(jsonStrong);
                    return json;
                });

            _jsonConverterMock.Setup(library => library.Convert(It.IsAny<JsonDocument>()))
                .Returns<JsonDocument>(dto =>
                {
                    TestDTO? document = dto.Deserialize<TestDTO>();

                    if (document == null)
                    {
                        Assert.Fail();
                        return null;
                    }
                    
                    return document;
                });
            
            List<TestDTO> expected =
            [
                new() { TestString = "One", TestInt = 1 },
                new() { TestString = "Two", TestInt = 2 }
            ];

            IEnumerable<TestDTO> returnedObjects = _directoryConverter.ConvertDirectory(DIRECTORY_PATH);
            
            Assert.Multiple(() =>
            {
                Assert.That(returnedObjects, Is.EquivalentTo(expected));
            });
        }

        [Test]
        public void Negative_ConvertDirectory_NoDirectoryFound_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>( () => _directoryConverter.ConvertDirectory("A/A"));
        }
    }
}
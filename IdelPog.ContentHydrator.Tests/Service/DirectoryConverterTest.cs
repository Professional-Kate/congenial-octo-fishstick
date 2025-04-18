using System.Text.Json;
using ContentHydrator.Assertions;
using ContentHydrator.Converters;
using ContentHydrator.Readers;
using ContentHydrator.Service;
using ContentHydratorTests.TestObjects;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentHydratorTests.Service
{
    [TestFixture]
    public class DirectoryConverterTest
    {
        private IDirectoryConverter<TestDTO> _directoryConverter { get; set; }
        private Mock<IJsonReader> _jsonReaderMock { get; set; }
        private Mock<IJsonConverter<TestDTO>> _jsonConverterMock { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        private const string DIRECTORY_PATH = "Resources/DirectoryConverterFiles";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _jsonReaderMock = new Mock<IJsonReader>();
            _jsonConverterMock = new Mock<IJsonConverter<TestDTO>>();
            _handlerMock = new Mock<IHandler>();
            _directoryConverter = new DirectoryConverter<TestDTO>(_jsonReaderMock.Object, _jsonConverterMock.Object, new AssertFound(_handlerMock.Object), new AssertDirectoryNotEmpty(_handlerMock.Object));
        }

        [SetUp]
        public void SetUp()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Returns<string>(path =>
                {
                    string jsonStrong = File.ReadAllText(path);
                    return JsonDocument.Parse(jsonStrong);
                });

            _jsonConverterMock.Setup(library => library.Convert(It.IsAny<JsonDocument>()))
                .Returns<JsonDocument>(dto =>
                {
                    TestDTO? finalTdo = dto.Deserialize<TestDTO>();
                    if (finalTdo == null)
                    {
                        throw new Exception("TEST FAIL - Deserialize DTO FAILED");
                    }
                    
                    return finalTdo;
                });
        }

        [Test]
        public void Positive_ConvertDirectory_ConvertsWholeDirectory()
        {
            List<TestDTO> expected =
            [
                new() { TestString = "One", TestInt = 1 },
                new() { TestString = "Two", TestInt = 2 }
            ];

            IEnumerable<TestDTO> returnedObjects = _directoryConverter.ConvertDirectory(DIRECTORY_PATH);
            
            Assert.That(returnedObjects, Is.EquivalentTo(expected));
        }
        
        [Test]
        public void Negative_ConvertDirectory_NoDirectoryFound_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<NotFoundException>()))
                .Throws(new NotFoundException("A/A"));
            
            Assert.Throws<NotFoundException>( () => _directoryConverter.ConvertDirectory("A/A"));
        }

        [Test]
        public void Negative_ConvertDictionary_EmptyDictionary_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<Exception>()))
                .Throws<Exception>();
            
            string emptyDirectory = Path.Combine(DIRECTORY_PATH, "TEMP");
            Directory.CreateDirectory(emptyDirectory);
            
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory(emptyDirectory));
            
            Directory.Delete(emptyDirectory, true);
        }
        
        [Test]
        public void Negative_ConvertDirectory_EmptyPath_Throws()
        {
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory(""));
        }

        [Test]
        public void Negative_ConvertDictionary_ReaderThrows()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Throws<Exception>();
            
            // We need to iterate once in order to throw the exception
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory(DIRECTORY_PATH).First());
        }

        [Test]
        public void Negative_ConvertDictionary_ConverterThrows()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Throws<Exception>();
            
            // We need to iterate once in order to throw the exception
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory(DIRECTORY_PATH).First());
        }
    }
}
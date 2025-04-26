using System.Text.Json;
using ContentHydrator.Assertions.Pipelines;
using ContentHydrator.Converters;
using ContentHydrator.Exceptions;
using ContentHydrator.Providers;
using ContentHydrator.Readers;
using ContentHydrator.Service;
using ContentHydratorTests.TestObjects;
using IdelPog.Validation.Exceptions;
using Moq;

namespace ContentHydratorTests.Service
{
    [TestFixture]
    public class DirectoryConverterTest
    {
        private IDirectoryConverter _directoryConverter { get; set; }
        private Mock<IJsonReader> _jsonReaderMock { get; set; }
        private Mock<IConverterProvider> _converterProviderMock { get; set; }
        private Mock<IJsonConverter<TestDTO>> _jsonConverterMock { get; set; }
        private Mock<IDirectoryAsserter> _directoryAsserter { get; set; }

        private const string DIRECTORY_PATH = "Resources/DirectoryConverterFiles";

        [SetUp]
        public void SetUp()
        {
            _jsonReaderMock = new Mock<IJsonReader>();
            _jsonConverterMock = new Mock<IJsonConverter<TestDTO>>();
            _directoryAsserter = new Mock<IDirectoryAsserter>();
            _converterProviderMock = new Mock<IConverterProvider>();
            _directoryConverter = new DirectoryConverter(_jsonReaderMock.Object, _converterProviderMock.Object, _directoryAsserter.Object);
            
            _converterProviderMock.Setup(library => library.CreateConverter<TestDTO>())
                .Returns(_jsonConverterMock.Object);
            
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
                        Assert.Fail();
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

            List<TestDTO> returnedObjects = _directoryConverter.ConvertDirectory<TestDTO>(DIRECTORY_PATH).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(returnedObjects, Is.EquivalentTo(expected));
                Assert.That(returnedObjects, Is.EqualTo(expected));
            });
            
            _jsonConverterMock.Verify(library => library.Convert(It.IsAny<JsonDocument>()), Times.Exactly(expected.Count));
            _jsonReaderMock.Verify(library => library.Read(It.IsAny<string>()), Times.Exactly(expected.Count));
        }
        
        [Test]
        public void Negative_ConvertDirectory_NoDirectoryFound_Throws()
        {
            _directoryAsserter.Setup(library => library.AssertDirectory(It.IsAny<string>()))
                .Throws(new NotFoundException("A/A"));
            
            Assert.Throws<NotFoundException>( () => _directoryConverter.ConvertDirectory<TestDTO>("A/A"));
        }

        [Test]
        public void Negative_ConvertDictionary_EmptyDictionary_Throws()
        {
            _directoryAsserter.Setup(library => library.AssertFiles(It.IsAny<string[]>(), It.IsAny<string>()))
                .Throws(new EmptyDirectoryException(DIRECTORY_PATH));
            
            string emptyDirectory = Path.Combine(DIRECTORY_PATH, "TEMP");
            Directory.CreateDirectory(emptyDirectory);
            
            Assert.Throws<EmptyDirectoryException>(() => _directoryConverter.ConvertDirectory<TestDTO>(emptyDirectory));
            
            Directory.Delete(emptyDirectory, true);
        }
        
        [Test]
        public void Negative_ConvertDirectory_EmptyPath_Throws()
        {
            Assert.Throws<ArgumentException>(() => _directoryConverter.ConvertDirectory<TestDTO>(""));
        }

        [Test]
        public void Negative_ConvertDictionary_ReaderThrows()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Throws<Exception>();
            
            // We need to iterate once in order to throw the exception
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory<TestDTO>(DIRECTORY_PATH).First());
        }

        [Test]
        public void Negative_ConvertDictionary_ConverterThrows()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Throws<Exception>();
            
            // We need to iterate once in order to throw the exception
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory<TestDTO>(DIRECTORY_PATH).First());
        }

        [Test]
        public void Negative_ConvertDictionary_MissesNonJsonFile_Throws()
        {
            // Three total files, 
            _directoryConverter.ConvertDirectory<TestDTO>(DIRECTORY_PATH).ToList();
            
            _jsonConverterMock.Verify(library => library.Convert(It.IsAny<JsonDocument>()), Times.Exactly(2));
            _jsonReaderMock.Verify(library => library.Read(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
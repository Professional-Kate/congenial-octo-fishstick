using System.Text.Json;
using ContentHydratorTests.TestObjects;
using IdelPog.ContentHydrator.Assertions;
using IdelPog.ContentHydrator.Assertions.Pipelines;
using IdelPog.ContentHydrator.Converters;
using IdelPog.ContentHydrator.Exceptions;
using IdelPog.ContentHydrator.Providers;
using IdelPog.ContentHydrator.Readers;
using IdelPog.ContentHydrator.Service;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace ContentHydratorTests.Service
{
    [TestFixture]
    public class DirectoryConverterTest
    {
        private IDirectoryConverter _directoryConverter { get; set; }
        private Mock<IJsonReader> _jsonReaderMock { get; set; }
        private Mock<IConverterProvider> _converterProviderMock { get; set; }
        private Mock<IJsonConverter<TestObject>> _jsonConverterMock { get; set; }
        private IDirectoryAssertionPipeline _directoryAsserter { get; set; }

        private const string DIRECTORY_PATH = "Resources/DirectoryConverterFiles";

        [SetUp]
        public void SetUp()
        {
            _jsonReaderMock = new Mock<IJsonReader>();
            _jsonConverterMock = new Mock<IJsonConverter<TestObject>>();
            _directoryAsserter = new DirectoryAssertionPipeline(new DirectoryAssertion(new ThrowHandler()), new ObjectNullAssertion(new ThrowHandler()));
            _converterProviderMock = new Mock<IConverterProvider>();
            _directoryConverter = new DirectoryConverter(_jsonReaderMock.Object, _converterProviderMock.Object, _directoryAsserter);

            _converterProviderMock.Setup(library => library.CreateConverter<TestObject>())
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
                    TestObject? finalTdo = dto.Deserialize<TestObject>();
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
            List<TestObject> expected =
            [
                new() { TestString = "One", TestInt = 1 },
                new() { TestString = "Two", TestInt = 2 }
            ];

            List<TestObject> returnedObjects = _directoryConverter.ConvertDirectory<TestObject>(DIRECTORY_PATH).ToList();

            Assert.That(returnedObjects, Is.EquivalentTo(expected));

            _jsonConverterMock.Verify(library => library.Convert(It.IsAny<JsonDocument>()), Times.Exactly(expected.Count));
            _jsonReaderMock.Verify(library => library.Read(It.IsAny<string>()), Times.Exactly(expected.Count));
        }

        [Test]
        public void Negative_ConvertDirectory_NoDirectoryFound_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryConverter.ConvertDirectory<TestObject>("A/A"));
        }

        [Test]
        public void Negative_ConvertDirectory_EmptyDirectory_Throws()
        {
            string emptyDirectory = Path.Combine(DIRECTORY_PATH, "TEMP");
            Directory.CreateDirectory(emptyDirectory);

            EmptyDirectoryException exception = Assert.Throws<EmptyDirectoryException>(() => _directoryConverter.ConvertDirectory<TestObject>(emptyDirectory));
            Assert.That(exception.Path, Is.EqualTo(emptyDirectory));

            Directory.Delete(emptyDirectory, true);
        }

        [Test]
        public void Negative_ConvertDirectory_EmptyPath_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryConverter.ConvertDirectory<TestObject>(""));
        }

        [Test]
        public void Negative_ConvertDirectory_ReaderThrows()
        {
            _jsonReaderMock.Setup(library => library.Read(It.IsAny<string>()))
                .Throws<Exception>();

            // We need to iterate once in order to throw the exception
            Assert.Throws<Exception>(() => _directoryConverter.ConvertDirectory<TestObject>(DIRECTORY_PATH).First());
        }

        [Test]
        public void Negative_ConvertDirectory_ConverterThrows()
        {
            _jsonConverterMock.Setup(library => library.Convert(It.IsAny<JsonDocument>()))
                .Throws<ArgumentNullException>();

            // We need to iterate once in order to throw the exception
            Assert.Throws<ArgumentNullException>(() => _directoryConverter.ConvertDirectory<TestObject>(DIRECTORY_PATH).First());
        }

        [Test]
        public void Negative_ConvertDirectory_MissesNonJsonFile()
        {
            // Three total files, 
            _directoryConverter.ConvertDirectory<TestObject>(DIRECTORY_PATH).ToList();

            _jsonConverterMock.Verify(library => library.Convert(It.IsAny<JsonDocument>()), Times.Exactly(2));
            _jsonReaderMock.Verify(library => library.Read(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
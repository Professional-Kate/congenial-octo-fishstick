using System.Text.Json;
using IdelPog.ContentHydrator.Readers;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace ContentHydratorTests.Readers
{
    [TestFixture]
    public class JsonReaderTest
    {
        private IJsonReader _jsonFileJsonReader { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _jsonFileJsonReader = new JsonReader(new ObjectNullAssertion(_handlerMock.Object));
        }

        private JsonDocument ReadFromTestFile(string fileName = "TwoKeys.json")
        {
            return _jsonFileJsonReader.Read($"Resources/{fileName}");
        }

        [Test]
        public void Positive_Read_ReadsJsonFile()
        {
            JsonDocument returnedValue = ReadFromTestFile();
            string testValue = returnedValue.RootElement.GetProperty("Test").ToString();

            Assert.That(testValue, Is.EqualTo("Testing"));
        }

        [Test]
        public void Positive_Read_ReadsMultipleKeys()
        {
            JsonDocument returnedValue = ReadFromTestFile();

            string testValue = returnedValue.RootElement.GetProperty("Test").ToString();
            string jasonValue = returnedValue.RootElement.GetProperty("Jason").ToString();

            Assert.Multiple(() =>
            {
                Assert.That(testValue, Is.EqualTo("Testing"));
                Assert.That(jasonValue, Is.EqualTo("21"));
            });
        }

        [Test]
        public void Positive_Read_ReadsNothingFromEmptyFile()
        {
            JsonDocument returnedValue = ReadFromTestFile("EmptyStructure.json");

            Assert.That(returnedValue, Is.Not.Null);
        }

        [Test]
        public void Negative_Read_NoFile_Throws()
        {
            Assert.Throws<FileNotFoundException>(() => ReadFromTestFile("ILostMyFile.json"));
        }

        [Test]
        public void Negative_Read_NullOrEmptyPath_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _jsonFileJsonReader.Read(null));
            Assert.Throws<ArgumentException>(() => _jsonFileJsonReader.Read(string.Empty));
        }

        [Test]
        public void Negative_Read_AssertNotNull_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws(new ArgumentNullException());

            Assert.Throws<ArgumentNullException>(() => _jsonFileJsonReader.Read(null));
        }
    }
}
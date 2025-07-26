using System.Text.Json;
using ContentHydratorTests.TestObjects;
using IdelPog.ContentHydrator.Converters;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace ContentHydratorTests.Converters
{
    [TestFixture]
    public class JsonSourceConverterTest
    {
        private JsonSourceConverter<TestDTO> _converter { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }
        private const string TEST_STRING = "testing";
        private const int TEST_NUMBER = 21;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _converter = new JsonSourceConverter<TestDTO>(TestHydrationContext.Default.TestDTO, new AssertNotNull(_handlerMock.Object));
        }

        private static IEnumerable<string> PositiveFlowDataSource()
        {
            // Normal test
            yield return $$"""
                           {
                              "TestString": "{{TEST_STRING}}",
                              "TestInt": {{TEST_NUMBER}}
                           }
                           """;

            // smol
            yield return $$"""{"TestString":"{{TEST_STRING}}","TestInt":{{TEST_NUMBER}}}""";
        }


        [TestCaseSource(nameof(PositiveFlowDataSource))]
        public void Positive_Convert_TestRunner(string json)
        {
            JsonDocument jsonDocument = JsonDocument.Parse(json);
            TestDTO testDTO = _converter.Convert(jsonDocument);

            Assert.Multiple(() =>
            {
                Assert.That(testDTO.TestInt, Is.EqualTo(TEST_NUMBER));
                Assert.That(testDTO.TestString, Is.EqualTo(TEST_STRING));
            });
        }

        [Test]
        public void Negative_Convert_MissingKeys_Throws()
        {
            string jason = "{}";

            JsonDocument jsonDocument = JsonDocument.Parse(jason);
            Assert.Throws<JsonException>(() => _converter.Convert(jsonDocument));
        }

        [Test]
        public void Negative_Convert_NullJson_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.Convert(null!));
        }

        [Test]
        public void Negative_Convert_EmptyString_Throws()
        {
            JsonDocument jsonDocument = JsonDocument.Parse("{}");
            Assert.Throws<JsonException>(() => _converter.Convert(jsonDocument));
        }

        [Test]
        public void Negative_Convert_BadTypes_Throws()
        {
            string jason = $$"""
                             {
                                "TestString": {{TEST_NUMBER}},
                                "TestInt": "{{TEST_STRING}}"
                             }
                             """;

            JsonDocument jsonDocument = JsonDocument.Parse(jason);
            Assert.Throws<JsonException>(() => _converter.Convert(jsonDocument));
        }

        [Test]
        public void Negative_Convert_AssertionThrows()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws<ArgumentNullException>();

            JsonDocument jsonDocument = JsonDocument.Parse("null");
            Assert.Throws<ArgumentNullException>(() => _converter.Convert(jsonDocument));
            _handlerMock.Verify(library => library.Handle(It.IsAny<ArgumentNullException>()), Times.Once);
        }
    }
}
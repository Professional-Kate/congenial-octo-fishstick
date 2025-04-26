using System.Text.Json;
using ContentHydrator.Converters;
using ContentHydrator.Providers;
using ContentHydratorTests.TestObjects;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;
using TestContext = ContentHydratorTests.TestObjects.TestContext;

namespace ContentHydratorTests.Providers
{
    [TestFixture]
    public class ConverterProviderTest
    {
        private IConverterProvider _converterProvider { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _converterProvider = new ConverterProvider(TestContext.Default, new AssertNotNull(_handlerMock.Object));
        }

        [Test]
        public void Positive_CreateConverter_CreatesCorrectConverter()
        {
            IJsonConverter<TestDTO> converter = _converterProvider.CreateConverter<TestDTO>();
            
            Assert.That(converter, Is.Not.Null);
            Assert.That(converter, Is.InstanceOf<IJsonConverter<TestDTO>>());
            _handlerMock.Verify(library => library.Handle(It.IsAny<ArgumentNullException>()), Times.Never);
        }

        [Test]
        public void Positive_CreateConverter_ConverterConverts()
        {
            JsonDocument jsonDocument = JsonDocument.Parse("""{"TestString": "TESTING", "TestInt": 10 }""");
            
            IJsonConverter<TestDTO> converter = _converterProvider.CreateConverter<TestDTO>();
            TestDTO result = converter.Convert(jsonDocument);
            
            Assert.Multiple(() =>
            {
                Assert.That(result.TestString, Is.EqualTo("TESTING"));
                Assert.That(result.TestInt, Is.EqualTo(10));
            });
        }
    }
}
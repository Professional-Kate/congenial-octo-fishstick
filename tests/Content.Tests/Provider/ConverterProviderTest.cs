using System.Text.Json;
using IdelPog.Content.Hydrator.Hydration.Converter;
using IdelPog.Content.Hydrator.Hydration.Provider;
using IdelPog.Content.Tests.TestObjects;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Handler.Interface;
using Moq;

namespace IdelPog.Content.Tests.Provider
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
            _converterProvider = new ConverterProvider(TestObjects.TestContext.Default, new ObjectNullAssertion(_handlerMock.Object));
        }

        [Test]
        public void Positive_CreateConverter_CreatesCorrectConverter()
        {
            IJsonConverter<TestObject> converter = _converterProvider.CreateConverter<TestObject>();

            Assert.That(converter, Is.Not.Null);
            Assert.That(converter, Is.InstanceOf<IJsonConverter<TestObject>>());
            _handlerMock.Verify(library => library.Handle(It.IsAny<ArgumentNullException>()), Times.Never);
        }

        [Test]
        public void Positive_CreateConverter_ConverterConverts()
        {
            JsonDocument jsonDocument = JsonDocument.Parse("""{"TestString": "TESTING", "TestInt": 10 }""");

            IJsonConverter<TestObject> converter = _converterProvider.CreateConverter<TestObject>();
            TestObject result = converter.Convert(jsonDocument);

            Assert.Multiple(() =>
            {
                Assert.That(result.TestString, Is.EqualTo("TESTING"));
                Assert.That(result.TestInt, Is.EqualTo(10));
            });
        }
    }
}
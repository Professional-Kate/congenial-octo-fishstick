using ContentHydrator.Converters;
using ContentHydrator.Providers;
using ContentHydrator.Readers;
using ContentHydrator.Service;
using ContentHydratorTests.TestObjects;
using Moq;

namespace ContentHydratorTests.Orchestration
{
    [TestFixture]
    public class HydratorTest
    {
        private IHydrator _hydrator { get; set; }
        private Mock<IJsonReader> _jsonReaderMock { get; set; }
        private Mock<IConverterProvider> _converterProviderMock { get; set; }
        private Mock<IJsonConverter<TestDTO>> _converterMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _converterMock = new Mock<IJsonConverter<TestDTO>>();
            _jsonReaderMock = new Mock<IJsonReader>();
            _converterProviderMock = new Mock<IConverterProvider>();
            _hydrator = new Hydrator(_jsonReaderMock.Object, _converterProviderMock.Object);

            _converterProviderMock.Setup(library => library.CreateConverter<TestDTO>())
                .Returns(_converterMock.Object);
        }

        [Test]
        public void Positive_HydrateTo_HydratesTestObjects()
        {
            TestDTO[] returnedObjects = _hydrator.HydrateFrom<TestDTO>("Resources").ToArray();

            _converterProviderMock.Verify(library => library.CreateConverter<TestDTO>(), Times.Once);
        }
    }
}
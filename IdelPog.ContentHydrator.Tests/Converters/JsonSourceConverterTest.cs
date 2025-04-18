using ContentHydrator.Converters;
using ContentHydratorTests.TestObjects;

namespace ContentHydratorTests.Converters
{
    [TestFixture]
    public class JsonSourceConverterTest
    {
        private JsonSourceConverter<TestDTO> _converter;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _converter = new JsonSourceConverter<TestDTO>(TestHydrationContext.Default.TestDTO);
        }
    }
}
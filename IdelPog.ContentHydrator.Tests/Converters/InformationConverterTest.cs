using ContentHydrator.Converters;
using ContentHydrator.DTO;

namespace ContentHydratorTests.Converters
{
    [TestFixture]
    public class InformationConverterTest
    {
        private InformationConverter _converter { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _converter = new InformationConverter();
        }

        [Test]
        public void Positive_Convert_ConvertsString()
        {
            InformationDTO expected = new("Testing", "The testing skill");

            // InformationDTO actual = _jsonConverter.Convert("");
        }
    }
}
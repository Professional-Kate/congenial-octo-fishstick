using System.Text.Json;
using ContentHydrator.Converters;
using ContentHydratorTests.TestObjects;

namespace ContentHydratorTests.Converters
{
    [TestFixture]
    public class JsonSourceConverterTest
    {
        private JsonSourceConverter<TestDTO> _converter { get; set; }
        private const string TEST_STRING = "testing";
        private const int TEST_NUMBER = 21;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _converter = new JsonSourceConverter<TestDTO>(TestHydrationContext.Default.TestDTO);
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
            TestDTO testDTO = _converter.Convert(json);
            
            Assert.Multiple(() =>
            {
                Assert.That(testDTO.TestInt, Is.EqualTo(TEST_NUMBER));
                Assert.That(testDTO.TestString, Is.EqualTo(TEST_STRING));
            });
        }

        [Test]
        public void Negative_Convert_InvalidJson_Throws()
        {
            string jason = $$"""
                            {
                               "TestString": "{{TEST_STRING}}",
                               "TestInt": {{TEST_NUMBER}}, 
                            }
                            """;
            
            Assert.Throws<JsonException>(() => _converter.Convert(jason));
        }

        [Test]
        public void Negative_Convert_MissingKeys_Throws()
        {
            string jason = "{}";
            
            Assert.Throws<JsonException>(() => _converter.Convert(jason));
        }

        [Test]
        public void Negative_Convert_NullJson_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _converter.Convert(null!));
        }

        [Test]
        public void Negative_Convert_EmptyString_Throws()
        {
            Assert.Throws<JsonException>(() => _converter.Convert(""));
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
            
            Assert.Throws<JsonException>(() => _converter.Convert(jason));
        }
    }
}
using ContentHydrator.Readers;

namespace ContentHydratorTests.Readers
{
    [TestFixture]
    public class JsonFileReaderTest
    {
        private readonly IReader _jsonFileReader = new JsonFileReader();

        private Dictionary<string, object> ReadFromTestFile(string fileName = "TwoKeys.json")
        {
            return _jsonFileReader.Read($"Resources/{fileName}");
        }

        [Test]
        public void Positive_Read_ReadsJsonFile()
        {
            Dictionary<string, object> returnedValue = ReadFromTestFile();
            string testValue = returnedValue["Test"].ToString();
            
            Assert.That(testValue, Is.EqualTo("Testing"));
        }

        [Test]
        public void Positive_Read_ReadsMultipleKeys()
        {
            Dictionary<string, object> returnedValue = ReadFromTestFile();
            
            string testValue = returnedValue["Test"].ToString();
            string jasonValue = returnedValue["Jason"].ToString();
            
            Assert.Multiple(() =>
            {
                Assert.That(testValue, Is.EqualTo("Testing"));
                Assert.That(jasonValue, Is.EqualTo("21"));
            });
        }

        [Test]
        public void Positive_Read_ReadsNothingFromEmptyFile()
        {
            Dictionary<string, object> returnedValue = ReadFromTestFile("EmptyStructure.json");
            
            Assert.Multiple(() =>
            {
                Assert.That(returnedValue, Is.Not.Null);
                Assert.That(returnedValue, Is.Empty);
            });
        }

        [Test]
        public void Negative_Read_NoFile_Throws()
        {
            Assert.Throws<FileNotFoundException>(() => ReadFromTestFile("ILostMyFile.json"));
        }

        [Test]
        public void Negative_Read_NullOrEmptyPath_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _jsonFileReader.Read(null));
            Assert.Throws<ArgumentException>(() => _jsonFileReader.Read(string.Empty));
        }
    }
}
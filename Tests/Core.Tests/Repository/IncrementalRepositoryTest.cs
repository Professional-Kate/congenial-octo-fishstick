using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Core.Tests.Repository
{
    [TestFixture]
    public sealed class IncrementalRepositoryTest
    {
        private IncrementalRepository<string> _stringRepository;
        private Mock<IDictionary<byte, string>> _dictionaryMock;
        private readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _dictionaryMock = new Mock<IDictionary<byte, string>>();
        }
        
        [SetUp]
        public void SetUp()
        { 
            _dictionaryMock.Reset();
            _stringRepository = new IncrementalRepository<string>(_dictionaryMock.Object, _repositoryAsserter);
        }

        [TearDown]
        public void TearDown()
        {
            _dictionaryMock.Verify();
            _dictionaryMock.VerifyNoOtherCalls();
        }

        private void VerifyDictionaryAdd(string value, byte key)
        {
            _dictionaryMock.Verify(library => library.Add(key, value), Times.Once);
        }

        private void SetupDictionaryContainsKey(byte key, bool contains)
        {
            _dictionaryMock.Setup(library => library.ContainsKey(key)).Returns(contains).Verifiable();
        }

        private void SetupDictionaryGet(string value, byte key)
        {
            _dictionaryMock.Setup(library => library[key]).Returns(value).Verifiable();
        }

        [Test]
        public void Positive_Add_AddsNewValue_IncrementsID()
        {
            byte id = _stringRepository.Add("4");
            Assert.That(id, Is.Zero);
            VerifyDictionaryAdd("4", 0);
            
            byte secondID = _stringRepository.Add("12");
            Assert.That(secondID, Is.EqualTo(1));
            VerifyDictionaryAdd("12", 1);
        }

        [Test]
        public void Negative_Add_ByteOverflow_Throws()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                byte id = _stringRepository.Add(i.ToString());
                
                Assert.That(id, Is.EqualTo(i));
                VerifyDictionaryAdd(i.ToString(), i);
            }
            
            Assert.Throws<OverflowException>(() => _stringRepository.Add("2324"));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _stringRepository.Add(null!));
        }

        [Test]
        public void Positive_Contains_ReturnsFalse()
        {
            SetupDictionaryContainsKey(0, false);
            
            bool contains = _stringRepository.Contains(0);
            
            Assert.That(contains, Is.False);
        }
        
        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            byte id = _stringRepository.Add("4");
            SetupDictionaryContainsKey(id, true);
            
            bool contains = _stringRepository.Contains(id);
            
            Assert.That(contains, Is.True);
            VerifyDictionaryAdd("4", id);
        }

        [Test]
        public void Positive_Get_ReturnsValue()
        {
            byte id = _stringRepository.Add("4");
            SetupDictionaryContainsKey(id, true);
            SetupDictionaryGet("4", id);

            string value = _stringRepository.Get(id);
            
            Assert.That(value, Is.EqualTo("4"));
            VerifyDictionaryAdd("4", id);
        }

        [Test]
        public void Negative_Get_NotFound_Throws()
        {
            SetupDictionaryContainsKey(0, false);
            
            Assert.Throws<NotFoundException<byte>>(() => _stringRepository.Get(0));
        }
    }
}
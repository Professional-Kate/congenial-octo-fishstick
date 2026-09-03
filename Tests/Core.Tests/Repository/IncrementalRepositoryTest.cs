using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Repository
{
    [TestFixture]
    public sealed class IncrementalRepositoryTest
    {
        private IncrementalRepository<string> _stringRepository;
        private readonly RepositoryAsserter _repositoryAsserter = new(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

        [SetUp]
        public void SetUp()
        { 
            _stringRepository = new IncrementalRepository<string>(_repositoryAsserter);
        }

        private void VerifyGet(string value, byte key)
        { 
            Assert.That(_stringRepository.Get(key), Is.EqualTo(value));
        }

        private void VerifyContains(byte key, bool contains)
        { 
            Assert.That(_stringRepository.Contains(key), Is.EqualTo(contains));
        }

        [Test]
        public void Positive_Add_AddsNewValue_IncrementsID()
        {
            byte id = _stringRepository.Add("4");
            Assert.That(id, Is.Zero);
            VerifyGet("4", 0);
            
            byte secondID = _stringRepository.Add("12");
            Assert.That(secondID, Is.EqualTo(1));
            VerifyGet("12", 1);
        }

        [Test]
        public void Negative_Add_ByteOverflow_Throws()
        {
            for (byte i = 0; i < byte.MaxValue; i++)
            {
                byte id = _stringRepository.Add(i.ToString());
                
                Assert.That(id, Is.EqualTo(i));
                VerifyGet(i.ToString(), i);
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
            bool contains = _stringRepository.Contains(0);
            
            Assert.That(contains, Is.False);
        }
        
        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            byte id = _stringRepository.Add("4");
            VerifyContains(id, true);
            
            bool contains = _stringRepository.Contains(id);
            
            Assert.That(contains, Is.True);
            VerifyGet("4", id);
        }

        [Test]
        public void Positive_Get_ReturnsValue()
        {
            byte id = _stringRepository.Add("4");
            VerifyContains(id, true);

            string value = _stringRepository.Get(id);
            
            VerifyGet("4", id);
            Assert.That(value, Is.EqualTo("4"));
            VerifyGet("4", id);
        }

        [Test]
        public void Negative_Get_NotFound_Throws()
        {
            VerifyContains(0, false);
            
            Assert.Throws<NotFoundException<byte>>(() => _stringRepository.Get(0));
        }
    }
}
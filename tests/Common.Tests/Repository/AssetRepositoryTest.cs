using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Common.Tests.Repository
{
    [TestFixture]
    public class AssetRepositoryTest
    {
        private IAssetRepository<int, string> _repository { get; set; }
        private IRepositoryAsserter _asserterMock { get; set; }

        [SetUp]
        public void Setup()
        {
            _asserterMock = new RepositoryAsserter(new FoundAssertion(new ThrowHandler()), new ObjectNullAssertion(new ThrowHandler()),
                new UniqueAssertion(new ThrowHandler()));

            _repository = new AssetRepository<int, string>(_asserterMock);
        }

        [Test]
        public void Positive_DefaultConstruction_CreatesRepositoryAsserter()
        {
            _repository = new AssetRepository<int, string>();

            _repository.Add(1, "10");

            Assert.Throws<DuplicateEntityException>(() => _repository.Add(1, "10"));
            NotFoundException<int> getException = Assert.Throws<NotFoundException<int>>(() => _repository.Get(2));
            Assert.That(getException.Key, Is.EqualTo(2));
            NotFoundException<int> removeException = Assert.Throws<NotFoundException<int>>(() => _repository.Remove(2));
            Assert.That(removeException.Key, Is.EqualTo(2));
        }

        [Test]
        public void Positive_Add_AddsIntPair()
        {
            _repository.Add(1, "1");

            bool contains = _repository.Contains(1);
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Positive_Remove_RemovesIntPair()
        {
            _repository.Add(1, "1");

            _repository.Remove(1);
            bool contains = _repository.Contains(1);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Positive_Remove_RemovesCorrectPair()
        {
            _repository.Add(1, "1");
            _repository.Add(2, "2");

            _repository.Remove(1);

            Assert.Multiple(() =>
            {
                Assert.That(_repository.Contains(2), Is.True);
                Assert.That(_repository.Contains(1), Is.False);
            });
        }

        [Test]
        public void Positive_Get_ReturnsCorrectValue()
        {
            _repository.Add(1, "1");
            _repository.Add(2, "2");

            string value = _repository.Get(1);
            Assert.That(value, Is.EqualTo("1"));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repository.Add(1, "1");

            bool contains = _repository.Contains(1);
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Positive_Contains_ReturnsFalse()
        {
            bool contains = _repository.Contains(1);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Negative_Add_KeyAlreadyExists_Throws()
        {
            const int key = 1;
            _repository.Add(key, "1");
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _repository.Add(key, "1"));
            Assert.That(exception.ID, Is.EqualTo("1"));
        }

        [Test]
        public void Negative_Add_KeyNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _repository.Add(1, null!));
        }

        [Test]
        public void Negative_Remove_NoKeyFound_Throws()
        {
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _repository.Remove(1));
            Assert.That(exception.Key, Is.EqualTo(1));
        }

        [Test]
        public void Negative_Remove_GetAfterRemove_Throws()
        {
            _repository.Add(1, "1");

            _repository.Remove(1);

            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _repository.Get(1));
            Assert.That(exception.Key, Is.EqualTo(1));
        }

        [Test]
        public void Negative_Get_NoKeyFound_Throws()
        {
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _repository.Get(1));
            Assert.That(exception.Key, Is.EqualTo(1));
        }
    }
}
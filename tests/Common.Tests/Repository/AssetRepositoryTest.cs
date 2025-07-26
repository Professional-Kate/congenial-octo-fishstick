using IdelPog.Common.Repository;
using IdelPog.Validation.Exceptions;
using Moq;

namespace IdelPog.Common.Tests.Repository
{
    [TestFixture]
    public class AssetRepositoryTest
    {
        private IAssetRepository<int, string> _repository { get; set; }
        private Mock<IRepositoryAsserter> _asserterMock { get; set; }

        [SetUp]
        public void Setup()
        {
            _asserterMock = new Mock<IRepositoryAsserter>();
            _repository = new AssetRepository<int, string>(_asserterMock.Object);
        }

        [Test]
        public void Positive_DefaultConstruction_CreatesRepositoryAsserter()
        {
            _repository = new AssetRepository<int, string>();

            _repository.Add(1, "10");

            Assert.Throws<DuplicateItemException>(() => _repository.Add(1, "10"));
            Assert.Throws<NotFoundException>(() => _repository.Get(2));
            Assert.Throws<NotFoundException>(() => _repository.Remove(2));
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
            _asserterMock.Setup(library => library.AssertUnique(It.IsAny<object>(), It.IsAny<Func<bool>>()))
                .Throws(new DuplicateItemException("1"));

            Assert.Throws<DuplicateItemException>(() => _repository.Add(1, "1"));
        }

        [Test]
        public void Negative_Add_KeyNull_Throws()
        {
            _asserterMock.Setup(library => library.AssertUnique(It.IsAny<object>(), It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();

            Assert.Throws<ArgumentNullException>(() => _repository.Add(1, null!));
        }

        [Test]
        public void Negative_Remove_NoKeyFound_Throws()
        {
            _asserterMock.Setup(library => library.AssertFound(It.IsAny<object>(), It.IsAny<Func<bool>>()))
                .Throws(new KeyNotFoundException("1"));

            Assert.Throws<KeyNotFoundException>(() => _repository.Remove(1));
        }

        [Test]
        public void Negative_Remove_GetAfterRemove_Throws()
        {
            _repository.Add(1, "1");

            _repository.Remove(1);

            Assert.Throws<KeyNotFoundException>(() => _repository.Get(1));
        }

        [Test]
        public void Negative_Get_NoKeyFound_Throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Get(1));
        }
    }
}
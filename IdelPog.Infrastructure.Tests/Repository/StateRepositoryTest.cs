using IdelPog.Infrastructure.Repository;
using IdelPog.Validation.Exceptions;
using Moq;

namespace IdelPog.Infrastructure.Tests.Repository
{
    [TestFixture]
    public class StateRepositoryTest
    {
        private IRepository<int, CloneableTestObject> _repository;
        private Mock<IRepositoryAsserter> _repositoryAsserterMock;

        private CloneableTestObject _cloneableTestObject { get; set; }
        private const string VALUE = "VALUE";
        private const int KEY = 1;

        [SetUp]
        public void Setup()
        {
            _repositoryAsserterMock = new Mock<IRepositoryAsserter>();
            _repository = new StateRepository<int, CloneableTestObject>(_repositoryAsserterMock.Object);
            _cloneableTestObject = new CloneableTestObject(VALUE);
        }
       
        [Test]
        public void Positive_Add_AddsItem()
        {
            _repository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject returnedObject = _repository.Get(KEY);
            
            Assert.That(returnedObject.GetValue(), Is.EqualTo(VALUE));
        }

        [Test]
        public void Negative_Add_DuplicateKey_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(It.IsAny<object>(), It.IsAny<Func<bool>>()))
                .Throws(new DuplicateItemException(KEY));
            
            Assert.Throws<DuplicateItemException>(() => _repository.Add(KEY, _cloneableTestObject));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(null!, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Add(KEY, null!));
        }

        [Test]
        public void Positive_Remove_RemovesItem()
        {
            _repository.Add(KEY, _cloneableTestObject);
            _repository.Remove(KEY);
            
            bool contains = _repository.Contains(KEY);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Negative_Remove_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Remove(KEY));
        }

        [Test]
        public void Positive_Get_ReturnsItem()
        {
            _repository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject returnedObject = _repository.Get(KEY);
            
            Assert.That(returnedObject.GetValue(), Is.EqualTo(VALUE));
        }

        [Test]
        public void Negative_Get_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Get(KEY));
        }

        [Test]
        public void Positive_Update_UpdatesItem()
        {
            const string newValue = "NEWER VALUE";
            _repository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject newTestObject = new(newValue);
            _repository.Update(KEY, newTestObject);
            
            CloneableTestObject returnedObject = _repository.Get(KEY);
            Assert.That(returnedObject.GetValue(), Is.EqualTo(newValue));
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Update(KEY, _cloneableTestObject));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Update(KEY, null!));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repository.Add(KEY, _cloneableTestObject);
            
            bool  contains = _repository.Contains(KEY);
            
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Negative_Contains_NotFound_ReturnsFalse()
        {
            bool contains = _repository.Contains(KEY);
            
            Assert.That(contains, Is.False);
        }
    }
}
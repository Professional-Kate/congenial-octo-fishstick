using IdelPog.Common.Repository;
using IdelPog.Validation.Exceptions;
using Moq;

namespace IdelPog.Common.Tests.Repository
{
    [TestFixture]
    public class StateRepositoryTest
    {
        private IStateRepository<int, CloneableTestObject> _stateRepository;
        private Mock<IRepositoryAsserter> _repositoryAsserterMock;

        private CloneableTestObject _cloneableTestObject { get; set; }
        private const string VALUE = "VALUE";
        private const int KEY = 1;

        [SetUp]
        public void Setup()
        {
            _repositoryAsserterMock = new Mock<IRepositoryAsserter>();
            _stateRepository = new StateRepository<int, CloneableTestObject>(_repositoryAsserterMock.Object);
            _cloneableTestObject = new CloneableTestObject(VALUE);
        }
        
        [Test]
        public void Positive_DefaultConstruction_CreatesRepositoryAsserter()
        {
            _stateRepository = new StateRepository<int, CloneableTestObject>();
            
            _stateRepository.Add(1, _cloneableTestObject);
            
            Assert.Throws<DuplicateItemException>(() => _stateRepository.Add(1, _cloneableTestObject));
            Assert.Throws<NotFoundException>(() => _stateRepository.Get(2));
            Assert.Throws<NotFoundException>(() => _stateRepository.Remove(2));
            Assert.Throws<NotFoundException>(() => _stateRepository.Update(2, _cloneableTestObject));
        }
       
        [Test]
        public void Positive_Add_AddsItem()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject returnedObject = _stateRepository.Get(KEY);
            
            Assert.That(returnedObject.GetValue(), Is.EqualTo(VALUE));
        }

        [Test]
        public void Negative_Add_DuplicateKey_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(It.IsAny<object>(), It.IsAny<Func<bool>>()))
                .Throws(new DuplicateItemException(KEY));
            
            Assert.Throws<DuplicateItemException>(() => _stateRepository.Add(KEY, _cloneableTestObject));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(null!, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _stateRepository.Add(KEY, null!));
        }

        [Test]
        public void Positive_Remove_RemovesItem()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);
            _stateRepository.Remove(KEY);
            
            bool contains = _stateRepository.Contains(KEY);
            Assert.That(contains, Is.False);
        }

        [Test]
        public void Negative_Remove_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _stateRepository.Remove(KEY));
        }

        [Test]
        public void Positive_Get_ReturnsItem()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject returnedObject = _stateRepository.Get(KEY);
            
            Assert.That(returnedObject.GetValue(), Is.EqualTo(VALUE));
        }

        [Test]
        public void Negative_Get_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _stateRepository.Get(KEY));
        }

        [Test]
        public void Positive_Update_UpdatesItem()
        {
            const string newValue = "NEWER VALUE";
            _stateRepository.Add(KEY, _cloneableTestObject);
            
            CloneableTestObject newTestObject = new(newValue);
            _stateRepository.Update(KEY, newTestObject);
            
            CloneableTestObject returnedObject = _stateRepository.Get(KEY);
            Assert.That(returnedObject.GetValue(), Is.EqualTo(newValue));
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _stateRepository.Update(KEY, _cloneableTestObject));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _stateRepository.Update(KEY, null!));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);
            
            bool  contains = _stateRepository.Contains(KEY);
            
            Assert.That(contains, Is.True);
        }

        [Test]
        public void Negative_Contains_NotFound_ReturnsFalse()
        {
            bool contains = _stateRepository.Contains(KEY);
            
            Assert.That(contains, Is.False);
        }
    }
}
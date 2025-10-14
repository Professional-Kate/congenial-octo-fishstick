using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Repository
{
    [TestFixture]
    public sealed class StateRepositoryTest
    {
        private IStateRepository<int, CloneableTestObject> _stateRepository;
        private IRepositoryAsserter _asserterMock;

        private CloneableTestObject _cloneableTestObject { get; set; }
        private const string VALUE = "VALUE";
        private const int KEY = 1;

        [SetUp]
        public void Setup()
        {
            _asserterMock = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _stateRepository = new StateRepository<int, CloneableTestObject>(_asserterMock);
            _cloneableTestObject = new CloneableTestObject(VALUE);
        }

        [Test]
        public void Positive_DefaultConstruction_CreatesRepositoryAsserter()
        {
            _stateRepository = new StateRepository<int, CloneableTestObject>();

            _stateRepository.Add(1, _cloneableTestObject);

            Assert.Throws<DuplicateEntityException>(() => _stateRepository.Add(1, _cloneableTestObject));
            Assert.Throws<NotFoundException<int>>(() => _stateRepository.Get(2));
            Assert.Throws<NotFoundException<int>>(() => _stateRepository.Remove(2));
            Assert.Throws<NotFoundException<int>>(() => _stateRepository.Update(2, _cloneableTestObject));
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
            _stateRepository.Add(KEY, _cloneableTestObject);
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _stateRepository.Add(KEY, _cloneableTestObject));
            Assert.That(exception.ID, Is.EqualTo(_cloneableTestObject));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
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
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _stateRepository.Remove(KEY));
            Assert.That(exception.Key, Is.EqualTo(KEY));
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
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _stateRepository.Get(KEY));
            Assert.That(exception.Key, Is.EqualTo(KEY));
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
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _stateRepository.Update(KEY, _cloneableTestObject));
            Assert.That(exception.Key, Is.EqualTo(KEY));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);
            Assert.Throws<ArgumentNullException>(() => _stateRepository.Update(KEY, null!));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _stateRepository.Add(KEY, _cloneableTestObject);

            bool contains = _stateRepository.Contains(KEY);

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
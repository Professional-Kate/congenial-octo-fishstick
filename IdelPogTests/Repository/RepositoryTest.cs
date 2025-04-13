using IdelPog.Engine.Models;
using IdelPog.Engine.Repository;
using IdelPog.Engine.Validation.Exceptions;
using IdelPog.Engine.Validation.Pipelines;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Repository
{
    [TestFixture]
    public class RepositoryTest
    {
        private IRepository<int, Currency> _repository;
        private Mock<IRepositoryAsserter> _repositoryAsserterMock;

        private Currency _currency { get; set; }
        private const int KEY = 1;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currency = CurrencyFactory.CreateFood();
        }
        
        [SetUp]
        public void Setup()
        {
            _repositoryAsserterMock = new Mock<IRepositoryAsserter>();
            _repository = new Repository<int, Currency>(_repositoryAsserterMock.Object);
        }
       
        [Test]
        public void Positive_Add_AddsItem()
        {
            _repository.Add(KEY, _currency);
            
            Currency returnedCurrency = _repository.Get(KEY);
            Assert.Multiple(() =>
            {
                Assert.That(_currency.Amount, Is.EqualTo(returnedCurrency.Amount));
                Assert.That(_currency.CurrencyType, Is.EqualTo(returnedCurrency.CurrencyType));
            });
        }

        [Test]
        public void Negative_Add_DuplicateKey_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(_currency, It.IsAny<Func<bool>>()))
                .Throws(new DuplicateItemException(KEY));
            
            Assert.Throws<DuplicateItemException>(() => _repository.Add(KEY, _currency));
        }

        [Test]
        public void Negative_Add_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertUnique(null, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Add(KEY, null));
        }

        [Test]
        public void Positive_Remove_RemovesItem()
        {
            _repository.Add(KEY, _currency);
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
            _repository.Add(KEY, _currency);
            
            Currency returnedCurrency = _repository.Get(KEY);
            Assert.Multiple(() =>
            {
                Assert.That(_currency.Amount, Is.EqualTo(returnedCurrency.Amount));
                Assert.That(_currency.CurrencyType, Is.EqualTo(returnedCurrency.CurrencyType));
            });
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
            _repository.Add(KEY, _currency);
            Currency newCurrency = new(_currency.CurrencyType, 100);
            _repository.Update(KEY, newCurrency);
            
            Currency returnedCurrency = _repository.Get(KEY);
            Assert.Multiple(() =>
            {
                Assert.That(newCurrency.Amount, Is.EqualTo(returnedCurrency.Amount));
                Assert.That(newCurrency.CurrencyType, Is.EqualTo(returnedCurrency.CurrencyType));
            });
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws(new NotFoundException(KEY));
            
            Assert.Throws<NotFoundException>(() => _repository.Update(KEY, _currency));
        }

        [Test]
        public void Negative_Update_NullValue_Throws()
        {
            _repositoryAsserterMock.Setup(library => library.AssertFound(KEY, It.IsAny<Func<bool>>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _repository.Update(KEY, null));
        }

        [Test]
        public void Positive_Contains_ReturnsTrue()
        {
            _repository.Add(KEY, _currency);
            
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
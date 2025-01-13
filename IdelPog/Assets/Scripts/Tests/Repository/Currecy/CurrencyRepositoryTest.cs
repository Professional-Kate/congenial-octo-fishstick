using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Structures.Enums;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Repository
{
    [TestFixture(CurrencyType.FOOD)]
    public class CurrencyRepositoryTest
    {
        private CurrencyRepository _currencyRepository { get; set; }
        private Currency _foodCurrency  { get; set; }
        private CurrencyType _currencyType { get; }

        public CurrencyRepositoryTest(CurrencyType currencyType)
        {
            _currencyType = currencyType;
        }

        [SetUp]
        public void Setup()
        {
            _currencyRepository = new CurrencyRepository();
            _foodCurrency = new Currency(_currencyType);
        }
        
        [Test]
        public void Positive_Add_AddsCurrencyIntoRepository()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);

            Currency currency = _currencyRepository.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, currency.Amount);
        }
        
        [Test]
        public void Negative_AddWithNullCurrency_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyRepository.Add(_currencyType, null));
        }

        [Test]
        public void Negative_AddWithBadType_ThrowsException()
        {
            Currency badCurrency = new(CurrencyType.NO_TYPE);
            
            Assert.Throws<NoTypeException>(() => _currencyRepository.Add(CurrencyType.NO_TYPE, badCurrency));
        }

        [Test]
        public void Negative_AddDuplicateCurrency_ThrowsException()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            
            Assert.Throws<ArgumentException>(() => _currencyRepository.Add(_currencyType, _foodCurrency));
        }

        [Test]
        public void Positive_Remove_RemovesCurrencyFromRepository()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            _currencyRepository.Remove(_currencyType);
            
            Assert.Throws<NotFoundException>(() => _currencyRepository.Get(_currencyType));
        }

        [Test]
        public void Positive_Remove_RemovesCurrencyWithCorrectTag()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            
            Currency currency = CurrencyFactory.CreateWood();
            _currencyRepository.Add(CurrencyType.WOOD, currency);

            
            _currencyRepository.Remove(_currencyType);
            Currency woodCurrency = _currencyRepository.Get(currency.CurrencyType);
            Assert.IsNotNull(woodCurrency);
        }

        [Test]
        public void Negative_RemoveWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _currencyRepository.Remove(CurrencyType.NO_TYPE));
        }

        [Test]
        public void Negative_RemoveNonExistingCurrency_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _currencyRepository.Remove(_currencyType));
        }

        [Test]
        public void Positive_Get_ReturnsCurrency()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            
            Currency currency = _currencyRepository.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, currency.Amount);
        }

        [Test]
        public void Positive_Get_ReturnsClone()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            Currency currency = _currencyRepository.Get(_currencyType);

            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreNotEqual(_foodCurrency, currency);
        }
        
        
        [Test]
        public void Positive_Get_ReturnsCorrectCurrency()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);
            
            Currency woodCurrency = CurrencyFactory.CreateWood();
            _currencyRepository.Add(CurrencyType.WOOD, woodCurrency);
            
            Currency foodCurrency = _currencyRepository.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, foodCurrency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, foodCurrency.Amount);
            Assert.AreNotEqual(woodCurrency, foodCurrency);
        }

        [Test]
        public void Negative_GetWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _currencyRepository.Get(CurrencyType.NO_TYPE));
        }

        [Test]
        public void Negative_GetNonExistingCurrency_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _currencyRepository.Get(CurrencyType.FOOD));
        }

        [Test]
        public void Positive_Update_UpdatesCurrency()
        {
            _currencyRepository.Add(_currencyType, _foodCurrency);

            const int newAmount = 100;
            _foodCurrency.SetAmount(newAmount);
            
            _currencyRepository.Update(_currencyType, _foodCurrency);
            
            Assert.AreEqual(newAmount, _foodCurrency.Amount);
        }

        [Test]
        public void Negative_Update_NonExisting_Throws()
        {
            Assert.Throws<NotFoundException>(() => _currencyRepository.Update(_currencyType, _foodCurrency));
        }

        [Test]
        public void Negative_Update_NullCurrency_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyRepository.Update(_currencyType, null));
        }

        [Test]
        public void Negative_Update_BadTag_Throws()
        {
            Assert.Throws<NoTypeException>(() => _currencyRepository.Update(CurrencyType.NO_TYPE, _foodCurrency));
        }
    }
}

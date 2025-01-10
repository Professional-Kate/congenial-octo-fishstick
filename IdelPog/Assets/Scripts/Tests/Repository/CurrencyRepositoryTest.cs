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
        private CurrencyRepository _currencyRepositoryWrite { get; set; }
        private Currency _foodCurrency  { get; set; }
        private CurrencyType _currencyType { get; }

        public CurrencyRepositoryTest(CurrencyType currencyType)
        {
            _currencyType = currencyType;
        }

        [SetUp]
        public void Setup()
        {
            _currencyRepositoryWrite = new CurrencyRepository();
            _foodCurrency = new Currency(_currencyType);
        }
        
        [Test]
        public void Positive_Add_AddsCurrencyIntoRepository()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);

            Currency currency = _currencyRepositoryWrite.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, currency.Amount);
        }
        
        [Test]
        public void Negative_AddWithNullCurrency_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyRepositoryWrite.Add(_currencyType, null));
        }

        [Test]
        public void Negative_AddWithBadType_ThrowsException()
        {
            Currency badCurrency = new(CurrencyType.NO_TYPE);
            
            Assert.Throws<NoTypeException>(() => _currencyRepositoryWrite.Add(CurrencyType.NO_TYPE, badCurrency));
        }

        [Test]
        public void Negative_AddDuplicateCurrency_ThrowsException()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            
            Assert.Throws<ArgumentException>(() => _currencyRepositoryWrite.Add(_currencyType, _foodCurrency));
        }

        [Test]
        public void Positive_Remove_RemovesCurrencyFromRepository()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            _currencyRepositoryWrite.Remove(_currencyType);
            
            Assert.Throws<NotFoundException>(() => _currencyRepositoryWrite.Get(_currencyType));
        }

        [Test]
        public void Positive_Remove_RemovesCurrencyWithCorrectTag()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            
            Currency currency = CurrencyFactory.CreateWood();
            _currencyRepositoryWrite.Add(CurrencyType.WOOD, currency);

            
            _currencyRepositoryWrite.Remove(_currencyType);
            Currency woodCurrency = _currencyRepositoryWrite.Get(currency.CurrencyType);
            Assert.IsNotNull(woodCurrency);
        }

        [Test]
        public void Negative_RemoveWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _currencyRepositoryWrite.Remove(CurrencyType.NO_TYPE));
        }

        [Test]
        public void Negative_RemoveNonExistingCurrency_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _currencyRepositoryWrite.Remove(_currencyType));
        }

        [Test]
        public void Positive_Get_ReturnsCurrency()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            
            Currency currency = _currencyRepositoryWrite.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, currency.Amount);
        }

        [Test]
        public void Positive_Get_ReturnsClone()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            Currency currency = _currencyRepositoryWrite.Get(_currencyType);

            Assert.AreEqual(_foodCurrency.CurrencyType, currency.CurrencyType);
            Assert.AreNotEqual(_foodCurrency, currency);
        }
        
        
        [Test]
        public void Positive_Get_ReturnsCorrectCurrency()
        {
            _currencyRepositoryWrite.Add(_currencyType, _foodCurrency);
            
            Currency woodCurrency = CurrencyFactory.CreateWood();
            _currencyRepositoryWrite.Add(CurrencyType.WOOD, woodCurrency);
            
            Currency foodCurrency = _currencyRepositoryWrite.Get(_currencyType);
            Assert.AreEqual(_foodCurrency.CurrencyType, foodCurrency.CurrencyType);
            Assert.AreEqual(_foodCurrency.Amount, foodCurrency.Amount);
            Assert.AreNotEqual(woodCurrency, foodCurrency);
        }

        [Test]
        public void Negative_GetWithBadType_ThrowsException()
        {
            Assert.Throws<NoTypeException>(() => _currencyRepositoryWrite.Get(CurrencyType.NO_TYPE));
        }

        [Test]
        public void Negative_GetNonExistingCurrency_ThrowsException()
        {
            Assert.Throws<NotFoundException>(() => _currencyRepositoryWrite.Get(CurrencyType.FOOD));
        }
    }
}

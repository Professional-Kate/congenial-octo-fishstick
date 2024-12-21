using System;
using System.Collections.Generic;
using IdelPog.Model;
using IdelPog.Structures;
using NUnit.Framework;

namespace Tests.Repository
{
    [TestFixture(CurrencyType.FOOD)]
    public class CurrencyRepositoryTest
    {
        private TestableCurrencyRepository _currencyRepository { get; set; }
        private readonly Dictionary<CurrencyType, Currency> _testRepository = new();
        private Currency _currency  { get; set; }
        private CurrencyType _currencyType { get; }

        public CurrencyRepositoryTest(CurrencyType currencyType)
        {
            _currencyType = currencyType;
        }

        [OneTimeSetUp]
        public void Setup()
        {
            _currencyRepository = new TestableCurrencyRepository(_testRepository);
            _currency = new Currency(_currencyType);
        }

        [TearDown]
        public void TearDown()
        {
            _testRepository.Clear();
        }
        
        [Test]
        public void Positive_Add_AddsCurrencyIntoRepository()
        {
            bool success = _currencyRepository.Add(_currency);
            Assert.IsTrue(success);

            Currency currency = _testRepository[_currencyType];
            Assert.AreEqual(_currency, currency);
        }

        [Test]
        public void Positive_Add_AddsCurrencyWithCorrectTag()
        {
            _currencyRepository.Add(_currency);
            
            bool addedCurrency = _testRepository.ContainsKey(_currencyType);
            Assert.IsTrue(addedCurrency);
        }
        
        [Test]
        public void Negative_AddWithNullCurrency_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyRepository.Add(null));
        }

        [Test]
        public void Negative_AddWithBadType_ThrowsException()
        {
            Currency badCurrency = new(CurrencyType.NO_TYPE);
            
            Assert.Throws<ArgumentException>(() => _currencyRepository.Add(badCurrency));
        }

        [Test]
        public void Negative_AddDuplicateCurrency_ThrowsException()
        {
            _testRepository.Add(_currencyType, _currency);
            
            Assert.Throws<ArgumentException>(() => _currencyRepository.Add(_currency));
        }
    }
}

using System;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Service
{
    [TestFixture(CurrencyType.FOOD, 10)]
    public class CurrencyServiceTest
    {
        private TestableCurrencyService _currencyService { get; set; }
        private Mock<ICurrencyRepository> _currencyRepositoryMock { get; set; }
        private Currency _currency { get; set; }
        
        private readonly CurrencyType _currencyType;
        private readonly int _amount;

        public CurrencyServiceTest(CurrencyType currencyType, int amount)
        {
            _currencyType = CurrencyType.FOOD;
            _amount = amount;
        }
      
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _currencyService = new TestableCurrencyService(_currencyRepositoryMock.Object);
            _currency = new Currency(_currencyType);
        }
        
        [SetUp]
        public void Setup()
        {
            _currency = new Currency(_currencyType);
            _currencyRepositoryMock.Setup(library => library.Get(_currencyType)).Returns(_currency);
        }


        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            ServiceResponse serviceResponse = _currencyService.AddAmount(_currencyType, _amount);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(_amount, _currency.Amount);
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                _currencyService.AddAmount(_currencyType, _amount);
                Assert.AreEqual(_amount * i, _currency.Amount);
            }
        }

        [TestCase(-10)]
        [TestCase(0)]
        public void Negative_AddAmount_BadAmount_ReturnsBadServiceResponse(int amount)
        {
            ServiceResponse serviceResponse = _currencyService.AddAmount(_currencyType, amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            
            Assert.AreNotEqual(_amount, _currency.Amount);
        }

        [Test]
        public void Negative_AddAmount_UnknownCurrency_ReturnsBadServiceResponse()
        {
            _currencyRepositoryMock.Setup(library => library.Get(CurrencyType.WOOD))
                .Throws<NotFoundException>();
            
            ServiceResponse serviceResponse = _currencyService.AddAmount(CurrencyType.WOOD, _amount);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
        }
    }
}
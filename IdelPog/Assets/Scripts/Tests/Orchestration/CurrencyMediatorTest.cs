using System;
using System.Linq;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Orchestration.Currency;
using IdelPog.Repository.Currency;
using IdelPog.Service.Currency;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using Moq;
using NUnit.Framework;
using Tests.Utils;
using UnityEngine;

namespace Tests.Orchestration
{
    [TestFixture]
    public class CurrencyMediatorTest
    {
        private ICurrencyMediator _currencyService { get; set; }
        private Mock<ICurrencyRepository> _currencyRepositoryMock { get; set; }
        private Currency _foodCurrency { get; set; }
        private Currency _woodCurrency { get; set; }

        private const int Amount = 10;

        private static CurrencyTrade _addFoodTrade { get; set; }
        private static CurrencyTrade _removeFoodTrade { get; set; }
        private static CurrencyTrade _addWoodTrade { get; set; } 
        private static CurrencyTrade _removeWoodTrade { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CreateTrades();
        }
        
        [SetUp]
        public void TearDown()
        {
            _foodCurrency = CurrencyFactory.CreateFood();
            _woodCurrency = CurrencyFactory.CreateWood();
            
            SetupMock();
        }

        private void SetupMock()
        {
            _currencyRepositoryMock = new Mock<ICurrencyRepository>();
            _currencyService = new CurrencyMediator(new CurrencyService(), _currencyRepositoryMock.Object);
            
            _currencyRepositoryMock.Setup(library => library.Get(CurrencyType.FOOD)).Returns(_foodCurrency);
            _currencyRepositoryMock.Setup(library => library.Get(CurrencyType.WOOD)).Returns(_woodCurrency);
        }

        private void CreateTrades()
        {
            _addFoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.FOOD, ActionType.ADD);
            _removeFoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.FOOD, ActionType.REMOVE);
            _addWoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.WOOD, ActionType.ADD);
            _removeWoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.WOOD, ActionType.REMOVE);
        }
        
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] trades = Enumerable.Repeat(_addFoodTrade, tradeCount).ToArray();

            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Debug.Log(serviceResponse.Message);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(tradeCount * Amount, _foodCurrency.Amount);
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_UpdatesAmount(int tradeCount)
        {
            _foodCurrency.SetAmount(tradeCount * Amount + 10); // Not allowed to go negative or to zero, which is why the +10 is there.
            
            CurrencyTrade[] trades = Enumerable.Repeat(_removeFoodTrade, tradeCount).ToArray();
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Debug.Log(serviceResponse.Message);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(10, _foodCurrency.Amount);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_NoPassedTrades_ReturnsSuccess()
        {
            CurrencyTrade[] trades = Array.Empty<CurrencyTrade>();
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Assert.True(serviceResponse.IsSuccess);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_CurrencyNotFound_ReturnsFailure()
        {
            _currencyRepositoryMock.Setup(library => library.Get(_foodCurrency.CurrencyType))
                .Throws<NotFoundException>();
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(_addFoodTrade);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_OneCurrencyNotFound_ReturnsFailure()
        {
            _currencyRepositoryMock.Setup(library => library.Get(_foodCurrency.CurrencyType))
                .Throws<NotFoundException>();
            
            CurrencyTrade[] trades = { _addFoodTrade, _addWoodTrade };
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            Assert.AreEqual(0, _woodCurrency.Amount);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_PassedTradesResultInZeroAmount_ReturnsFail()
        {
            CurrencyTrade[] trades = { _addWoodTrade, _removeWoodTrade, _addFoodTrade, _removeWoodTrade };
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.AreEqual(0, _woodCurrency.Amount);
            Assert.AreEqual(0, _foodCurrency.Amount);
        }
        
        [Test]
        public void Positive_ProcessCurrencyUpdate_MultipleTypeUpdates_UpdatesAmount()
        {
            // certain upgrades will cost multiple currency / give multiple currency for buying. This test is to prove it works.
            CurrencyTrade[] trades = { _removeFoodTrade, _removeWoodTrade, _addFoodTrade, _addWoodTrade, _addFoodTrade, _addWoodTrade };
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(10, _woodCurrency.Amount);
            Assert.AreEqual(10, _foodCurrency.Amount); 
        }
        
        [TestCase(0, ActionType.ADD)]
        [TestCase(-10, ActionType.ADD)]
        [TestCase(-100, ActionType.ADD)]
        [TestCase(0, ActionType.REMOVE)]
        [TestCase(-10, ActionType.REMOVE)]
        [TestCase(-100, ActionType.REMOVE)]
        public void Negative_ProcessCurrencyUpdate_BadAmounts_NoUpdates(int badAmount, ActionType action)
        {
            CurrencyTrade trade = TestUtils.CreateTrade(badAmount, _foodCurrency.CurrencyType, action);

            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trade);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.NotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
        }
        
        [Test]
        public void Negative_ProcessCurrencyUpdate_ArrayFails_NoUpdates()
        {
            // First action is okay, 2nd action should stop processing for all actions 
            CurrencyTrade[] trades = { _addFoodTrade, _removeWoodTrade, _addFoodTrade };
            
            ServiceResponse serviceResponse = _currencyService.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.NotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            Assert.AreEqual(0, _woodCurrency.Amount);
        }
    }
}
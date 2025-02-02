using System;
using System.Linq;
using IdelPog.Model;
using IdelPog.Orchestration;
using IdelPog.Repository;
using IdelPog.Service;
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
        private ICurrencyMediator _currencyMediator { get; set; }
        private Mock<IRepository<CurrencyType, Currency>> _repositoryMock { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
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
        public void Setup()
        {
            _foodCurrency = CurrencyFactory.CreatePeople();
            _woodCurrency = CurrencyFactory.CreateGold();
            
            SetupMock();
        }

        private void SetupMock()
        {
            _repositoryMock = new Mock<IRepository<CurrencyType, Currency>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyMediator = new CurrencyMediator(_currencyServiceMock.Object, _repositoryMock.Object);

            _repositoryMock.Setup(library => library.Get(CurrencyType.PEOPLE)).Returns((Currency) _foodCurrency.Clone());
            _repositoryMock.Setup(library => library.Get(CurrencyType.GOLD)).Returns((Currency) _woodCurrency.Clone());
            
            _repositoryMock.Setup(library => library.Contains(It.IsAny<CurrencyType>())).Returns(true);

            _currencyServiceMock.Setup(library => library.AddAmount(It.IsAny<Currency>(), It.IsAny<int>()))
                .Callback<Currency, int>((currency, amount) =>
                {
                    int newAmount = currency.Amount + amount;
                    currency.SetAmount(newAmount);
                });

            _currencyServiceMock.Setup(library => library.RemoveAmount(It.IsAny<Currency>(), It.IsAny<int>()))
                .Callback<Currency, int>((currency, amount) =>
                {
                    int newAmount = currency.Amount - amount;
                    currency.SetAmount(newAmount);
                });

            _repositoryMock.Setup(library => library.Update(It.IsAny<CurrencyType>(), It.IsAny<Currency>()))
                .Callback<CurrencyType, Currency>((type, currency) =>
                {
                    switch (type)
                    {
                        case CurrencyType.PEOPLE:
                            _foodCurrency = currency;
                            break;
                        case CurrencyType.GOLD:
                            _woodCurrency = currency;
                            break;
                    }
                });
        }

        private static void CreateTrades()
        {
            _addFoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.PEOPLE, ActionType.ADD);
            _removeFoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.PEOPLE, ActionType.REMOVE);
            _addWoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.GOLD, ActionType.ADD);
            _removeWoodTrade = TestUtils.CreateTrade(Amount, CurrencyType.GOLD, ActionType.REMOVE);
        }

        private void VerifyUpdateCall(int amount)
        {
            _repositoryMock.Verify(library => library.Update(It.IsAny<CurrencyType>(), It.IsAny<Currency>()), Times.Exactly(amount));
        }

        private void VerifyContainsCalls(int amount)
        {
            _repositoryMock.Verify(library => library.Contains(It.IsAny<CurrencyType>()), Times.Exactly(amount));
        }

        private void VerifyGetCalls(int amount)
        {
            _repositoryMock.Verify(library => library.Get(It.IsAny<CurrencyType>()), Times.Exactly(amount));
        }
        
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] trades = Enumerable.Repeat(_addFoodTrade, tradeCount).ToArray();

            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(tradeCount * Amount, _foodCurrency.Amount);

            VerifyContainsCalls(1);
            VerifyGetCalls(1);
            VerifyUpdateCall(1);
        }
        
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] removeTrades = Enumerable.Repeat(_removeFoodTrade, tradeCount).ToArray();
            CurrencyTrade[] addTrades = Enumerable.Repeat(_addFoodTrade, tradeCount + 1).ToArray();
            
            _currencyMediator.ProcessCurrencyUpdate(addTrades);
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(removeTrades);
            
            Debug.Log(serviceResponse.Message);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(10, _foodCurrency.Amount);

            VerifyContainsCalls(2);
            VerifyGetCalls(2);
            VerifyUpdateCall(2);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_NoPassedTrades_ReturnsSuccess()
        {
            CurrencyTrade[] trades = Array.Empty<CurrencyTrade>();
            
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.True(serviceResponse.IsSuccess);
            
            VerifyContainsCalls(0);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_CurrencyNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(library => library.Contains(CurrencyType.PEOPLE)).Returns(false);

            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(_addFoodTrade);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            
            VerifyContainsCalls(1);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_OneCurrencyNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(library => library.Contains(CurrencyType.GOLD)).Returns(false);

            CurrencyTrade[] trades = { _addFoodTrade, _addWoodTrade };
            
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.IsNotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            Assert.AreEqual(0, _woodCurrency.Amount);
            
            VerifyContainsCalls(2);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_PassedTradesResultInZeroAmount_ReturnsFail()
        {
            CurrencyTrade[] trades = { _addWoodTrade, _addFoodTrade, _removeWoodTrade };
            
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.AreEqual(0, _woodCurrency.Amount);
            Assert.AreEqual(0, _foodCurrency.Amount);
            
            VerifyContainsCalls(2);
            VerifyGetCalls(2);
            VerifyUpdateCall(0);
        }
        
        [Test]
        public void Positive_ProcessCurrencyUpdate_MultipleTypeUpdates_UpdatesAmount()
        {
            // certain upgrades will cost multiple currency / give multiple currency for buying. This test is to prove it works.
            CurrencyTrade[] trades = { _removeFoodTrade, _removeWoodTrade, _addFoodTrade, _addWoodTrade, _addFoodTrade, _addWoodTrade };
            
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.True(serviceResponse.IsSuccess);
            Assert.AreEqual(10, _woodCurrency.Amount);
            Assert.AreEqual(10, _foodCurrency.Amount); 
            
            VerifyUpdateCall(2); // two currency = 2 update calls
            VerifyContainsCalls(2);
            VerifyGetCalls(2);
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

            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trade);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.NotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            
            VerifyContainsCalls(0);
            VerifyGetCalls(0);
            VerifyUpdateCall(0);
        }
        
        [Test]
        public void Negative_ProcessCurrencyUpdate_ArrayFails_NoUpdates()
        {
            // First action is okay, 2nd action should stop processing for all actions 
            CurrencyTrade[] trades = { _addFoodTrade, _removeWoodTrade, _addFoodTrade };
            
            ServiceResponse serviceResponse = _currencyMediator.ProcessCurrencyUpdate(trades);
            
            Assert.False(serviceResponse.IsSuccess);
            Assert.NotNull(serviceResponse.Message);
            Assert.AreEqual(0, _foodCurrency.Amount);
            Assert.AreEqual(0, _woodCurrency.Amount);
            
            VerifyContainsCalls(2);
            VerifyGetCalls(2);
            VerifyUpdateCall(0);
        }
    }
}
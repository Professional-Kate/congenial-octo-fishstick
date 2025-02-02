using System.Linq;
using IdelPog.Controller;
using IdelPog.Model;
using IdelPog.Orchestration;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using Moq;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Controller
{
    [TestFixture(CurrencyType.PEOPLE, 10)]
    public class CurrencyControllerTest
    {
        // TODO: when I implement a logging framework, ensure this class logs the ServiceResponse output.
        // TODO: most of these tests maybe aren't needed. Functionality is now mainly handed by the ICurrencyMediator.
        private CurrencyController _currencyController { get; set; }
        private Mock<ICurrencyMediator> _currencyServiceMock { get; set; }
        private Currency _foodCurrency { get; set; }
        private Currency _woodCurrency { get; set; }

        private readonly CurrencyType _currencyType;
        private readonly int _amount;
        
        private static CurrencyTrade _addFoodTrade { get; set; }
        private static CurrencyTrade _removeFoodTrade { get; set; }
        private static CurrencyTrade _addWoodTrade { get; set; } 
        private static CurrencyTrade _removeWoodTrade { get; set; }

        public CurrencyControllerTest(CurrencyType currencyType, int amount)
        {
            _currencyType = currencyType;
            _amount = amount;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _foodCurrency = new Currency(_currencyType);
            _woodCurrency = new Currency(CurrencyType.GOLD);
            _addFoodTrade = TestUtils.CreateTrade(_amount, _currencyType, ActionType.ADD);
            _removeFoodTrade = TestUtils.CreateTrade(_amount, _currencyType, ActionType.REMOVE);
            _addWoodTrade = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD);
            _removeWoodTrade = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.REMOVE);
        }

        [SetUp]
        public void Setup()
        {
            _foodCurrency.SetAmount(0);
            _woodCurrency.SetAmount(0);
            _currencyServiceMock = new Mock<ICurrencyMediator>();
            _currencyController = new CurrencyController(_currencyServiceMock.Object);
        }

        private void SetupMock(CurrencyTrade[] trades)
        {
            _currencyServiceMock.Setup(library => library.ProcessCurrencyUpdate(trades))
                .Callback<CurrencyTrade[]>(currencyTrades =>
                {
                    foreach (CurrencyTrade currencyTrade in currencyTrades)
                    {
                        switch (currencyTrade.Action)
                        {
                            case ActionType.ADD:
                                int newAmount = currencyTrade.Amount + _foodCurrency.Amount;
                                _foodCurrency.SetAmount(newAmount);
                                break;
                            case ActionType.REMOVE:
                                _foodCurrency.SetAmount(currencyTrade.Amount);
                                newAmount = currencyTrade.Amount - _foodCurrency.Amount;
                                _foodCurrency.SetAmount(newAmount);
                                break;
                        }
                    }
                })
                .Returns(ServiceResponse.Success);
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
            SetupMock(trades);
           
            _currencyController.UpdateCurrency(trades);
            
            Assert.AreEqual(tradeCount * _amount, _foodCurrency.Amount);
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_UpdatesAmount(int tradeCount)
        {
            CurrencyTrade[] trades = Enumerable.Repeat(_removeFoodTrade, tradeCount).ToArray();
            SetupMock(trades);
            
            _currencyController.UpdateCurrency(trades);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_MultipleTypeUpdates_UpdatesAmount()
        {
            CurrencyTrade[] trades = { _addWoodTrade, _addFoodTrade, _removeFoodTrade, _addFoodTrade, _addWoodTrade, _removeFoodTrade, _removeWoodTrade, _removeWoodTrade, _addFoodTrade };
            SetupMock(trades);
            
            _currencyController.UpdateCurrency(trades);
            
            Assert.AreEqual(0, _woodCurrency.Amount);
            Assert.AreEqual(10, _foodCurrency.Amount); // this is 10 because I add an extra _addFoodTrade call just to ensure correctness
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void Negative_ProcessCurrencyUpdate_BadAmounts_NoUpdates(int badAmount)
        {
            CurrencyTrade trade = TestUtils.CreateTrade(badAmount, _currencyType, ActionType.ADD);

            _currencyServiceMock.Setup(library => library.ProcessCurrencyUpdate(trade))
                .Returns(ServiceResponse.Failure(""));

            _currencyController.UpdateCurrency(trade);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_ArrayFails_NoUpdates()
        {
            // First action is okay, 2nd action should stop processing for all actions 
            CurrencyTrade[] trades = { _addFoodTrade, _removeWoodTrade, _addFoodTrade };
            
            _currencyServiceMock.Setup(library => library.ProcessCurrencyUpdate(trades))
                .Returns(ServiceResponse.Failure(""));
            
            _currencyServiceMock.Setup(library => library.ProcessCurrencyUpdate(trades))
                .Returns(ServiceResponse.Success);
            
            _currencyController.UpdateCurrency(trades);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
            Assert.AreEqual(0, _woodCurrency.Amount);
        }
    }
}
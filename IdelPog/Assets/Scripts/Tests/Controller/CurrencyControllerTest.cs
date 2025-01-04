using System.Linq;
using IdelPog.Model;
using IdelPog.Service;
using IdelPog.Service.Currency;
using IdelPog.Structures;
using Moq;
using NUnit.Framework;

namespace Tests.Controller
{
    [TestFixture(CurrencyType.FOOD, 10)]
    public class CurrencyControllerTest
    {
        // TODO: when I implement a logging framework, ensure this class logs the ServiceResponse output.
        private TestableCurrencyController _currencyController { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Currency _foodCurrency { get; set; }
        private Currency _woodCurrency { get; set; }

        private readonly CurrencyType _currencyType;
        private readonly int _amount;
        
        private static CurrencyTrade _addFoodTrade { get; set; }
        private static CurrencyTrade _removeFoodTrade { get; set; }

        public CurrencyControllerTest(CurrencyType currencyType, int amount)
        {
            _currencyType = currencyType;
            _amount = amount;
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _foodCurrency = new Currency(_currencyType);
            _woodCurrency = new Currency(CurrencyType.WOOD);
            _addFoodTrade = CreateTrade(_amount, _currencyType, ActionType.ADD);
            _removeFoodTrade = CreateTrade(_amount, _currencyType, ActionType.REMOVE);
            
        }

        [SetUp]
        public void Setup()
        {
            _foodCurrency.SetAmount(0);
            _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyController = new TestableCurrencyController(_currencyServiceMock.Object);
            SetupMock(_currencyType, _foodCurrency);
            SetupMock(CurrencyType.WOOD, _woodCurrency);
        }

        private void SetupMock(CurrencyType type, Currency currency)
        {
            _currencyServiceMock.Setup(library => library.AddAmount(type, _amount))
                .Callback<CurrencyType, int>((currencyType, amount) =>
                {
                    if (currencyType == type)
                    {
                        int newAmount = amount + currency.Amount;
                        currency.SetAmount(newAmount);
                    }
                })
                .Returns(ServiceResponse.Success);
            
            _currencyServiceMock.Setup(library => library.RemoveAmount(type, _amount))
                .Callback<CurrencyType, int>((currencyType, amount) =>
                {
                    if (currencyType == type)
                    {
                        currency.SetAmount(amount); // the Currency is not allowed to get negative, so this is needed.
                        
                        int newAmount = amount - currency.Amount;
                        currency.SetAmount(newAmount);
                    }
                })
                .Returns(ServiceResponse.Success);

        }

        private void VerifyAddAmount(CurrencyType type, int expected)
        {
            _currencyServiceMock.Verify(library => library.AddAmount(type, _amount), Times.Exactly(expected));
        }
        
        private void VerifyRemoveAmount(CurrencyType type, int expected)
        {
            _currencyServiceMock.Verify(library => library.RemoveAmount(type, _amount), Times.Exactly(expected));
        }

        private CurrencyTrade CreateTrade(int amount, CurrencyType type, ActionType action)
        {
            return new CurrencyTrade(amount, type, action);
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
            
            _currencyController.ProcessCurrencyUpdate(trades);
            
            Assert.AreEqual(tradeCount * _amount, _foodCurrency.Amount);
            VerifyAddAmount(_currencyType, tradeCount);
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
            
            _currencyController.ProcessCurrencyUpdate(trades);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
            VerifyRemoveAmount(_currencyType, tradeCount);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_MultipleTypeUpdates_UpdatesAmount()
        {
            CurrencyTrade addWood = CreateTrade(10, CurrencyType.WOOD, ActionType.ADD);
            CurrencyTrade removeWood = CreateTrade(10, CurrencyType.WOOD, ActionType.REMOVE);
            
            CurrencyTrade[] trades = { addWood, _addFoodTrade, _removeFoodTrade, _addFoodTrade, addWood, _removeFoodTrade, removeWood, removeWood, _addFoodTrade };
            
            _currencyController.ProcessCurrencyUpdate(trades);
            
            Assert.AreEqual(0, _woodCurrency.Amount);
            Assert.AreEqual(10, _foodCurrency.Amount); // this is 10 because I add an extra _addFoodTrade call just to ensure correctness
            
            VerifyRemoveAmount(_currencyType, 2);
            VerifyRemoveAmount(CurrencyType.WOOD, 2);
            VerifyAddAmount(_currencyType, 3);
            VerifyAddAmount(CurrencyType.WOOD, 2);
        }

        [TestCase(0)]
        [TestCase(-10)]
        [TestCase(-100)]
        public void Negative_ProcessCurrencyUpdate_BadAmounts_NoUpdates(int badAmount)
        {
            _currencyServiceMock.Setup(library => library.AddAmount(_currencyType, _amount))
                .Returns(ServiceResponse.Failure(""));
            
            CurrencyTrade trade = CreateTrade(badAmount, _currencyType, ActionType.ADD);

            _currencyController.ProcessCurrencyUpdate(trade);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
            VerifyAddAmount(_currencyType, 0);
            VerifyRemoveAmount(_currencyType, 0);
        }
    }
}
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
        private TestableCurrencyController _currencyController { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Currency _foodCurrency { get; set; }

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
            _addFoodTrade = new CurrencyTrade(_amount, _currencyType, ActionType.ADD);
            _removeFoodTrade = new CurrencyTrade(_amount, _currencyType, ActionType.REMOVE);
        }

        [SetUp]
        public void Setup()
        {
            _foodCurrency.SetAmount(0);
            _currencyServiceMock = new Mock<ICurrencyService>();
            _currencyController = new TestableCurrencyController(_currencyServiceMock.Object);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_UpdatesAmount(int tradeCount)
        {
            _currencyServiceMock.Setup(library => library.AddAmount(_currencyType, _amount))
                .Callback<CurrencyType, int>((currencyType, amount) =>
                {
                    if (currencyType == _currencyType)
                    {
                        int newAmount = amount + _foodCurrency.Amount;
                        _foodCurrency.SetAmount(newAmount);
                    }
                })
                .Returns(ServiceResponse.Success);
            
            CurrencyTrade[] trades = Enumerable.Repeat(_addFoodTrade, tradeCount).ToArray();
            
            _currencyController.ProcessCurrencyUpdate(trades);
            
            Assert.AreEqual(tradeCount * _amount, _foodCurrency.Amount);
            _currencyServiceMock.Verify(library => library.AddAmount(_currencyType, _amount), Times.Exactly(tradeCount));
        }
        
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(15)]
        [TestCase(20)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_UpdatesAmount(int tradeCount)
        {
            _currencyServiceMock.Setup(library => library.RemoveAmount(_currencyType, _amount))
                .Callback<CurrencyType, int>((currencyType, amount) =>
                {
                    if (currencyType == _currencyType)
                    {
                        _foodCurrency.SetAmount(amount); // the Currency is not allowed to get negative, so this is needed.
                        
                        int newAmount = amount - _foodCurrency.Amount;
                        _foodCurrency.SetAmount(newAmount);
                    }
                })
                .Returns(ServiceResponse.Success);
            
            CurrencyTrade[] trades = Enumerable.Repeat(_removeFoodTrade, tradeCount).ToArray();
            
            _currencyController.ProcessCurrencyUpdate(trades);
            
            Assert.AreEqual(0, _foodCurrency.Amount);
            _currencyServiceMock.Verify(library => library.RemoveAmount(_currencyType, _amount), Times.Exactly(tradeCount));
        }
    }
}
using IdelPog.Model;
using IdelPog.Service;
using NUnit.Framework;
using Tests.Utils;

namespace Tests.Service
{
    [TestFixture]
    public class CurrencyServiceTest
    {
        private ICurrencyService _currencyService { get; set; }
        private Currency _foodCurrency { get; set; }

        private const int Amount = 10;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyService = new CurrencyService();
        }
        
        [SetUp]
        public void Setup()
        {
            _foodCurrency = CurrencyFactory.CreateFood();
        }
        
        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            _currencyService.AddAmount(_foodCurrency, Amount);
            
            Assert.AreEqual(Amount, _foodCurrency.Amount); 
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                _currencyService.AddAmount(_foodCurrency, Amount);
                Assert.AreEqual(Amount * i, _foodCurrency.Amount);
            }
        }

        [TestCase(-10)]
        [TestCase(0)]
        public void Negative_AddAmount_BadAmount_ReturnsBadServiceResponse(int amount)
        {
            _currencyService.AddAmount(_foodCurrency, amount);
            
            Assert.AreNotEqual(Amount, _foodCurrency.Amount);
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmountFromCurrency()
        {
            _foodCurrency.SetAmount(Amount + 1); // Currency can't go negative, so we need this
            
            _currencyService.RemoveAmount(_foodCurrency, Amount);
            
            Assert.AreEqual(1, _foodCurrency.Amount);
        }

        [Test]
        public void Positive_RemoveAmount_CallingMultipleTimes_RemovesAmountFromCurrency()
        {
            _foodCurrency.SetAmount(Amount);
            
            for (int i = 10; i > 1; i--)
            {
                Assert.AreEqual(i, _foodCurrency.Amount);
                _currencyService.RemoveAmount(_foodCurrency, 1);
            }
        }
    }
}
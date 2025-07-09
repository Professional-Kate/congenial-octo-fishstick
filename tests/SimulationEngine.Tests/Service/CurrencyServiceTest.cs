using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
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
            _currencyService = new CurrencyService(new AssertPositive(new ThrowHandler()));
        }
        
        [SetUp]
        public void Setup()
        {
            _foodCurrency = CurrencyFactory.CreateGold();
        }
        
        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            _currencyService.AddAmount(_foodCurrency, Amount);
            
            Assert.That(Amount, Is.EqualTo(_foodCurrency.Amount)); 
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                _currencyService.AddAmount(_foodCurrency, Amount);
                Assert.That(Amount * i, Is.EqualTo(_foodCurrency.Amount));
            }
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_AddAmount_NegativeAmount_Throws(int amount)
        {
            Assert.Throws<NegativeNumberException>(() => _currencyService.AddAmount(_foodCurrency, amount));
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmountFromCurrency()
        {
            _foodCurrency.SetAmount(Amount + 1); // Currency can't go negative, so we need this
            
            _currencyService.RemoveAmount(_foodCurrency, Amount);
            
            Assert.That(1, Is.EqualTo(_foodCurrency.Amount));
        }

        [Test]
        public void Positive_RemoveAmount_CallingMultipleTimes_RemovesAmountFromCurrency()
        {
            _foodCurrency.SetAmount(Amount);
            
            for (int i = 10; i > 1; i--)
            {
                Assert.That(i, Is.EqualTo(_foodCurrency.Amount));
                _currencyService.RemoveAmount(_foodCurrency, 1);
            }
        }
        
        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_RemoveAmount_NegativeAmount_Throws(int amount)
        {
            Assert.Throws<NegativeNumberException>(() => _currencyService.RemoveAmount(_foodCurrency, amount));
        }
    }
}
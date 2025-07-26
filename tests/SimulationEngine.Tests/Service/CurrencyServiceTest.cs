using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyServiceTest
    {
        private ICurrencyService _currencyService { get; set; }
        private Currency _goldCurrency { get; set; }

        private const int Amount = 10;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyService = new CurrencyService(new AssertPositive(new ThrowHandler()), new AssertEnoughCurrency(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _goldCurrency = CurrencyFactory.CreateGold();
        }

        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            Assert.DoesNotThrow(() => _currencyService.AddAmount(_goldCurrency, Amount));

            Assert.That(_goldCurrency.Amount, Is.EqualTo(Amount));
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                Assert.DoesNotThrow(() => _currencyService.AddAmount(_goldCurrency, Amount));
                Assert.That(_goldCurrency.Amount, Is.EqualTo(Amount * i));
            }
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_AddAmount_NegativeAmount_Throws(int amount)
        {
            Assert.Throws<NegativeNumberException>(() => _currencyService.AddAmount(_goldCurrency, amount));
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmountFromCurrency()
        {
            _goldCurrency.SetAmount(Amount);

            Assert.DoesNotThrow(() => _currencyService.RemoveAmount(_goldCurrency, Amount));

            Assert.That(_goldCurrency.Amount, Is.EqualTo(0));
        }

        [TestCase(-1)]
        [TestCase(-10)]
        public void Negative_RemoveAmount_NegativeAmount_Throws(int amount)
        {
            Assert.Throws<NegativeNumberException>(() => _currencyService.RemoveAmount(_goldCurrency, amount));
        }
    }
}
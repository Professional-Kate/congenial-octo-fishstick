using IdelPog.Core.Validation.Handler;
using IdelPog.Currency.Assertion;
using IdelPog.Currency.Service;
using IdelPog.Currency.Service.Interface;
using IdelPog.Currency.Tests.Factory;

namespace IdelPog.Currency.Tests.Service
{
    [TestFixture]
    public class CurrencyServiceTest
    {
        private ICurrencyService _currencyService { get; set; }
        private IdelPog.Currency.Contracts.Currency _goldCurrency { get; set; }

        private const uint AMOUNT = 10;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyService = new CurrencyService(new CurrencyAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _goldCurrency = CurrencyFactory.CreateGold();
        }

        [Test]
        public void Positive_AddAmount_AddsAmountToCurrency()
        {
            Assert.DoesNotThrow(() => _currencyService.AddAmount(_goldCurrency, AMOUNT));

            Assert.That(_goldCurrency.Amount, Is.EqualTo(AMOUNT));
        }

        [Test]
        public void Positive_AddAmount_CallingMultipleTimes_AddsAmountToCurrency()
        {
            for (int i = 1; i <= 10; i++)
            {
                Assert.DoesNotThrow(() => _currencyService.AddAmount(_goldCurrency, AMOUNT));
                Assert.That(_goldCurrency.Amount, Is.EqualTo(AMOUNT * i));
            }
        }

        [Test]
        public void Positive_RemoveAmount_RemovesAmountFromCurrency()
        {
            _goldCurrency.Amount = AMOUNT;

            Assert.DoesNotThrow(() => _currencyService.RemoveAmount(_goldCurrency, AMOUNT));

            Assert.That(_goldCurrency.Amount, Is.EqualTo(0));
        }
    }
}
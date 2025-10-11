using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory;

namespace IdelPog.Currency.Tests.Factory
{
    [TestFixture]
    public sealed class CurrencyUpdateResponseFactoryTest
    {
        private CurrencyUpdateResponseFactory _currencyUpdateResponseFactory { get; set; }
        private Contracts.Currency _goldCurrency;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            IHandler throwHandler = new ThrowHandler();
            _currencyUpdateResponseFactory = new CurrencyUpdateResponseFactory(new ObjectNullAssertion(throwHandler), new CollectionAssertion(throwHandler));

            _goldCurrency = new Contracts.Currency(CurrencyType.GOLD, 0);
        }

        private static void AssertResponse(CurrencyUpdateResponse currencyUpdateResponse, Contracts.Currency currency)
        {
            Assert.Multiple(() =>
            {
                Assert.That(currency.Amount, Is.EqualTo(currencyUpdateResponse.CurrencyAmount));
                Assert.That(currency.CurrencyType, Is.EqualTo(currencyUpdateResponse.CurrencyType));
            });
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateResponseFactory.CreateFrom([_goldCurrency]);

            Assert.That(responses, Has.Count.EqualTo(1));
            
            AssertResponse(responses[0], _goldCurrency);
        }

        [Test]
        public void Positive_CreateFrom_ConvertsMultipleTrades()
        {
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateResponseFactory.CreateFrom([_goldCurrency, _goldCurrency]);
            
            Assert.That(responses, Has.Count.EqualTo(2));
            
            AssertResponse(responses[0], _goldCurrency);
            AssertResponse(responses[1], _goldCurrency);
        }

        [Test]
        public void Negative_CreateFrom_EmptyTrades_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateResponseFactory.CreateFrom([]));

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(Contracts.Currency)));
        }

        [Test]
        public void Negative_CreateFrom_NullTrades_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateResponseFactory.CreateFrom(null!));
        }
    }
}
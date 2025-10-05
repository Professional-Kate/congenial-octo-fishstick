using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Currency.Factory;
using IdelPog.Currency.Factory.Interface;

namespace IdelPog.Currency.Tests.Factory
{
    [TestFixture]
    public sealed class CurrencyUpdateResponseFactoryTest
    {
        private ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory { get; set; }
        private CurrencyUpdate _currencyUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            IHandler throwHandler = new ThrowHandler();
            _currencyUpdateResponseFactory = new CurrencyUpdateResponseFactory(new ObjectNullAssertion(throwHandler), new CollectionAssertion(throwHandler));

            _currencyUpdate = new CurrencyUpdate { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GOLD };
        }

        private static void AssertResponse(CurrencyUpdateResponse currencyUpdateResponse, CurrencyUpdate currencyTrade)
        {
            Assert.Multiple(() =>
            {
                Assert.That(currencyTrade.ActionType, Is.EqualTo(currencyUpdateResponse.ActionType));
                Assert.That(currencyTrade.Amount, Is.EqualTo(currencyUpdateResponse.Amount));
                Assert.That(currencyTrade.CurrencyType, Is.EqualTo(currencyUpdateResponse.CurrencyType));
            });
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateResponseFactory.CreateFrom([_currencyUpdate]);

            Assert.That(responses, Has.Count.EqualTo(1));
            
            AssertResponse(responses[0], _currencyUpdate);
        }

        [Test]
        public void Positive_CreateFrom_ConvertsMultipleTrades()
        {
            IReadOnlyList<CurrencyUpdateResponse> responses = _currencyUpdateResponseFactory.CreateFrom([_currencyUpdate, _currencyUpdate]);
            
            Assert.That(responses, Has.Count.EqualTo(2));
            
            AssertResponse(responses[0], _currencyUpdate);
            AssertResponse(responses[1], _currencyUpdate);
        }

        [Test]
        public void Negative_CreateFrom_EmptyTrades_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateResponseFactory.CreateFrom([]));

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
        }

        [Test]
        public void Negative_CreateFrom_NullTrades_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateResponseFactory.CreateFrom(null!));
        }
    }
}
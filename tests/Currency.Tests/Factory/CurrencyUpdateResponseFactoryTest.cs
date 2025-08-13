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
    public class CurrencyUpdateResponseFactoryTest
    {
        private ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory { get; set; }
        private IReadOnlyList<CurrencyUpdate> _currencyTrades { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            IHandler throwHandler = new ThrowHandler();
            _currencyUpdateResponseFactory = new CurrencyUpdateResponseFactory(new ObjectNullAssertion(throwHandler), new CollectionAssertion(throwHandler));

            _currencyTrades =
            [
                CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.ADD),
                CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.REMOVE),
                CurrencyUpdateFactory.Create(uint.MaxValue, CurrencyType.GOLD, ActionType.REMOVE),
                // The factory doesn't care about negatives. It should be verified elsewhere if negative numbers are an issue
                CurrencyUpdateFactory.Create(uint.MinValue, CurrencyType.GOLD, ActionType.REMOVE)
            ];
        }

        private void AssertCollection(CurrencyUpdateResponse currencyUpdateResponse, IReadOnlyList<CurrencyUpdate> currencyTrades)
        {
            for (int i = 0; i < currencyUpdateResponse.CurrencyUpdates.Length; i++)
            {
                CurrencyUpdate currencyUpdates = currencyUpdateResponse.CurrencyUpdates[i];
                CurrencyUpdate currencyUpdate = currencyTrades[i];

                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdates.ActionType, Is.EqualTo(currencyUpdate.ActionType));
                    Assert.That(currencyUpdates.Amount, Is.EqualTo(currencyUpdate.Amount));
                    Assert.That(currencyUpdates.CurrencyType, Is.EqualTo(currencyUpdate.CurrencyType));
                });
            }
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            CurrencyUpdateResponse responses = _currencyUpdateResponseFactory.CreateFrom(_currencyTrades);

            Assert.That(responses.CurrencyUpdates, Has.Length.EqualTo(_currencyTrades.Count));

            AssertCollection(responses, _currencyTrades);
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
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;

namespace IdelPogTests.Service
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
                TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD),
                TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.REMOVE),
                TestUtils.CreateTrade(uint.MaxValue, CurrencyType.GOLD, ActionType.REMOVE),
                // The factory doesn't care about negatives. It should be verified elsewhere if negative numbers are an issue
                TestUtils.CreateTrade(uint.MinValue, CurrencyType.GOLD, ActionType.REMOVE)
            ];
        }

        private void AssertCollection(IReadOnlyList<CurrencyUpdateResponse> currencyUpdateDTOs, IReadOnlyList<CurrencyUpdate> currencyTrades)
        {
            for (int i = 0; i < currencyUpdateDTOs.Count; i++)
            {
                CurrencyUpdateResponse currencyUpdateResponse = currencyUpdateDTOs[i];
                CurrencyUpdate currencyUpdate = currencyTrades[i];

                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdateResponse.Action, Is.EqualTo(currencyUpdate.Action));
                    Assert.That(currencyUpdateResponse.Amount, Is.EqualTo(currencyUpdate.Amount));
                    Assert.That(currencyUpdateResponse.CurrencyType, Is.EqualTo(currencyUpdate.CurrencyType));
                });
            }
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateResponse> updateDTOs = _currencyUpdateResponseFactory.CreateFrom(_currencyTrades);

            Assert.That(updateDTOs, Has.Count.EqualTo(_currencyTrades.Count));

            AssertCollection(updateDTOs, _currencyTrades);
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
using IdelPog.SimulationEngine.Flows.Currency;
using IdelPog.SimulationEngine.Flows.Currency.Assertions;
using IdelPog.SimulationEngine.Flows.Currency.Exceptions;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyUpdateFactoryTest
    {
        private ICurrencyUpdateFactory _currencyUpdateFactory { get; set; }
        private IReadOnlyList<CurrencyTrade> _currencyTrades { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _handlerMock = new Mock<IHandler>();
            _currencyUpdateFactory = new CurrencyUpdateFactory(new AssertNotNull(_handlerMock.Object), new AssertCollectionNotEmpty(_handlerMock.Object));
            
            _currencyTrades =
            [
                TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD),
                TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.REMOVE),
                TestUtils.CreateTrade(int.MaxValue, CurrencyType.GOLD, ActionType.REMOVE),
                // The factory doesn't care about negatives. It should be verified elsewhere if negative numbers are an issue
                TestUtils.CreateTrade(-10, CurrencyType.GOLD, ActionType.ADD),
                TestUtils.CreateTrade(-10, CurrencyType.GOLD, ActionType.REMOVE),
                TestUtils.CreateTrade(int.MinValue, CurrencyType.GOLD, ActionType.REMOVE)
            ];
        }

        private void AssertCollection(IReadOnlyList<CurrencyUpdateDTO> currencyUpdateDTOs, IReadOnlyList<CurrencyTrade> currencyTrades)
        {
            for (int i = 0; i < currencyUpdateDTOs.Count; i++)
            {
                CurrencyUpdateDTO currencyUpdateDTO = currencyUpdateDTOs[i];
                CurrencyTrade currencyTrade = currencyTrades[i];
                
                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdateDTO.Action, Is.EqualTo(currencyTrade.Action));
                    Assert.That(currencyUpdateDTO.Amount, Is.EqualTo(currencyTrade.Amount));
                    Assert.That(currencyUpdateDTO.Currency, Is.EqualTo(currencyTrade.Currency));
                });
            }
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs =_currencyUpdateFactory.CreateFrom(_currencyTrades);
            
            Assert.That(updateDTOs, Has.Count.EqualTo(_currencyTrades.Count));
            
            AssertCollection(updateDTOs, _currencyTrades);
        }

        [Test]
        public void Negative_CreateFrom_EmptyTrades_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<CollectionEmptyException>()))
                .Throws(new CollectionEmptyException());
            
            Assert.Throws<CollectionEmptyException>(() => _currencyUpdateFactory.CreateFrom([]));
        }
        
        [Test]
        public void Negative_CreateFrom_NullTrades_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateFactory.CreateFrom(null!));
        }
    }
}
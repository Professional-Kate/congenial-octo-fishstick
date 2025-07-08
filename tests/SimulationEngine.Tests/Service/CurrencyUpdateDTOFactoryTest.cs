using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Service
{
    [TestFixture]
    public class CurrencyUpdateDTOFactoryTest
    {
        private ICurrencyUpdateDTOFactory _currencyUpdateDTOFactory { get; set; }
        private IReadOnlyList<CurrencyUpdate> _currencyTrades { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _handlerMock = new Mock<IHandler>();
            _currencyUpdateDTOFactory = new CurrencyUpdateDTOFactory(new AssertNotNull(_handlerMock.Object), new AssertCollectionNotEmpty(_handlerMock.Object));
            
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

        private void AssertCollection(IReadOnlyList<CurrencyUpdateDTO> currencyUpdateDTOs, IReadOnlyList<CurrencyUpdate> currencyTrades)
        {
            for (int i = 0; i < currencyUpdateDTOs.Count; i++)
            {
                CurrencyUpdateDTO currencyUpdateDTO = currencyUpdateDTOs[i];
                CurrencyUpdate currencyUpdate = currencyTrades[i];
                
                Assert.Multiple(() =>
                {
                    Assert.That(currencyUpdateDTO.Action, Is.EqualTo(currencyUpdate.Action));
                    Assert.That(currencyUpdateDTO.Amount, Is.EqualTo(currencyUpdate.Amount));
                    Assert.That(currencyUpdateDTO.Currency, Is.EqualTo(currencyUpdate.CurrencyType));
                });
            }
        }

        [Test]
        public void Positive_CreateFrom_ConvertsTradeIntoUpdate()
        {
            IReadOnlyList<CurrencyUpdateDTO> updateDTOs =_currencyUpdateDTOFactory.CreateFrom(_currencyTrades);
            
            Assert.That(updateDTOs, Has.Count.EqualTo(_currencyTrades.Count));
            
            AssertCollection(updateDTOs, _currencyTrades);
        }

        [Test]
        public void Negative_CreateFrom_EmptyTrades_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<CollectionEmptyException>()))
                .Throws(new CollectionEmptyException(typeof(CurrencyUpdate)));
            
            Assert.Throws<CollectionEmptyException>(() => _currencyUpdateDTOFactory.CreateFrom([]));
        }
        
        [Test]
        public void Negative_CreateFrom_NullTrades_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateDTOFactory.CreateFrom(null!));
        }
    }
}
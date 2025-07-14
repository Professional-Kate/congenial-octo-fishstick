using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Controller
{
    [TestFixture]
    public class CurrencyControllerTest
    {
        private ICurrencyController _currencyController { get; set; }
        private Mock<ICurrencyUpdateMediator> _currencyUpdateMediatorMock { get; set; }
        private Mock<ICurrencyCreationMediator> _currencyCreationMediatorMock { get; set; }

        private List<CurrencyUpdate> _currencyTrades { get; set; }
        private List<CurrencyCreation> _currencyCreations { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _currencyTrades =
            [
                TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD),
                TestUtils.CreateTrade(5, CurrencyType.GOLD, ActionType.REMOVE),
                TestUtils.CreateTrade(10, CurrencyType.GEMS, ActionType.ADD),
                TestUtils.CreateTrade(5, CurrencyType.GEMS, ActionType.REMOVE)
            ];

            _currencyCreations =
            [
                new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 10},
                new CurrencyCreation { CurrencyType = CurrencyType.GEMS, StartingAmount = 10}
            ];
        }

        [SetUp]
        public void Setup()
        {
            _currencyUpdateMediatorMock = new Mock<ICurrencyUpdateMediator>();
            _currencyCreationMediatorMock = new Mock<ICurrencyCreationMediator>();
            _currencyController = new CurrencyController(_currencyUpdateMediatorMock.Object, _currencyCreationMediatorMock.Object);
        }

        [Test]
        public void Positive_UpdateCurrency_InvokesMediator()
        {
            _currencyController.UpdateCurrency(_currencyTrades);
            
            _currencyUpdateMediatorMock.Verify(library => library.ProcessCurrencyUpdate(_currencyTrades), Times.Once);
        }

        [Test]
        public void Positive_UpdateCurrency_DoesNotSuppressExceptions()
        {
            _currencyUpdateMediatorMock.Setup(library => library.ProcessCurrencyUpdate(_currencyTrades))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _currencyController.UpdateCurrency(_currencyTrades));
        }
        
        [Test]
        public void Positive_CreateCurrency_InvokesMediator()
        {
            _currencyController.CreateCurrency(_currencyCreations);
            
            _currencyCreationMediatorMock.Verify(library => library.CreateCurrency(_currencyCreations), Times.Once);
        }

        [Test]
        public void Positive_CreateCurrency_DoesNotSuppressExceptions()
        {
            _currencyCreationMediatorMock.Setup(library => library.CreateCurrency(_currencyCreations))
                .Throws<Exception>();
            
            Assert.Throws<Exception>(() => _currencyController.CreateCurrency(_currencyCreations));
        }
    }
}
using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPogTests.Utils;
using Moq;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class CurrencyUpdateMediatorTest
    {
        private ICurrencyUpdateMediator _currencyUpdateMediator { get; set; }
        private Mock<IStateRepository<CurrencyType, Currency>> _repositoryMock { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Mock<ICurrencyUpdateDispatcher>  _dispatcherMock { get; set; }
        private Mock<ICurrencyUpdateSummarizer> _currencyUpdateSummarizerMock { get; set; }

        private CurrencyUpdate _addGoldUpdate;
        private CurrencyUpdate _removeGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryMock = new Mock<IStateRepository<CurrencyType, Currency>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _dispatcherMock = new Mock<ICurrencyUpdateDispatcher>();
            _currencyUpdateSummarizerMock = new Mock<ICurrencyUpdateSummarizer>();
            
            IHandler throwHandler = new ThrowHandler();
            _currencyUpdateMediator = new CurrencyUpdateMediator(_currencyServiceMock.Object, _repositoryMock.Object, _dispatcherMock.Object, _currencyUpdateSummarizerMock.Object, new AssertPositive(throwHandler), new AssertCollectionNotEmpty(throwHandler), new AssertFound(throwHandler));

            _addGoldUpdate = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.ADD);
            _removeGoldUpdate = TestUtils.CreateTrade(10, CurrencyType.GOLD, ActionType.REMOVE);
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_SingleUpdate_UpdatesOneCurrency()
        {
            IReadOnlyList<CurrencyUpdate> currencyUpdate = [_addGoldUpdate];
            Currency goldCurrency = new(_addGoldUpdate.CurrencyType, 0);
            
            _repositoryMock.Setup(library => library.Contains(_addGoldUpdate.CurrencyType)).Returns(true);
            
            _repositoryMock.Setup(library => library.Get(_addGoldUpdate.CurrencyType)).Returns(goldCurrency);
            
            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(currencyUpdate)).Returns(currencyUpdate.ToArray);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.ProcessCurrencyUpdate(currencyUpdate));
            
            _repositoryMock.Verify(library => library.Contains(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Get(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Update(_addGoldUpdate.CurrencyType, goldCurrency));
            _repositoryMock.VerifyNoOtherCalls();

            _currencyServiceMock.Verify(library => library.AddAmount(goldCurrency, _addGoldUpdate.Amount), Times.Once);
            _dispatcherMock.Verify(library => library.Dispatch(currencyUpdate),  Times.Once);
            _currencyUpdateSummarizerMock.Verify(library => library.GetSummary(currencyUpdate), Times.Once);
        }
    }
}
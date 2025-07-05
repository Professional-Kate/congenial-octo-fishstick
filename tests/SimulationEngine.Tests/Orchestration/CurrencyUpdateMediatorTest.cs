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

        private Currency _goldCurrency;
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
            _goldCurrency = new Currency(_addGoldUpdate.CurrencyType, 0);
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _currencyServiceMock.Reset();
            _dispatcherMock.Reset();
            _currencyUpdateSummarizerMock.Reset();
            _goldCurrency = new Currency(_addGoldUpdate.CurrencyType, 0);
        }

        private void TestRunner(IReadOnlyList<CurrencyUpdate> updates, CurrencyUpdate[] summaryUpdates) 
        {
            _repositoryMock.Setup(library => library.Contains(_addGoldUpdate.CurrencyType)).Returns(true);
            
            _repositoryMock.Setup(library => library.Get(_addGoldUpdate.CurrencyType)).Returns(_goldCurrency);
            
            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(updates)).Returns(summaryUpdates);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.ProcessCurrencyUpdate(updates));
            
            _repositoryMock.Verify(library => library.Contains(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Get(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Update(_addGoldUpdate.CurrencyType, _goldCurrency), Times.Once);
            _repositoryMock.VerifyNoOtherCalls();

            _dispatcherMock.Verify(library => library.Dispatch(summaryUpdates),  Times.Once);
            _dispatcherMock.VerifyNoOtherCalls();
            
            _currencyUpdateSummarizerMock.Verify(library => library.GetSummary(updates), Times.Once);
            _currencyUpdateSummarizerMock.VerifyNoOtherCalls();
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_AddAmountToCurrency(int amountOfUpdates)
        {
            IReadOnlyList<CurrencyUpdate> currencyUpdate = Enumerable.Repeat(_addGoldUpdate, amountOfUpdates).ToList();
            CurrencyUpdate[] summaryUpdate = [new() { Action = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = _addGoldUpdate.Amount * amountOfUpdates }];
            
            TestRunner(currencyUpdate, summaryUpdate);

            _currencyServiceMock.Verify(library => library.AddAmount(_goldCurrency, _addGoldUpdate.Amount * amountOfUpdates), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void Positive_ProcessCurrencyUpdate_SingleRemoveUpdate_RemovesAmountFromCurrency(int amountOfUpdates)
        {
            _goldCurrency.SetAmount(_addGoldUpdate.Amount * amountOfUpdates);
            IReadOnlyList<CurrencyUpdate> currencyUpdate = Enumerable.Repeat(_removeGoldUpdate, amountOfUpdates).ToList();
            CurrencyUpdate[] summaryUpdate = [new() { Action = ActionType.REMOVE, CurrencyType = CurrencyType.GOLD, Amount = _addGoldUpdate.Amount * amountOfUpdates }];
            
            TestRunner(currencyUpdate, summaryUpdate);
            
            _currencyServiceMock.Verify(library => library.RemoveAmount(_goldCurrency, _addGoldUpdate.Amount * amountOfUpdates), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }
    }
}
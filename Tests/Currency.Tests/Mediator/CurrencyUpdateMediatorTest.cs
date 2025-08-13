using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Currency.Exceptions;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Mediator;
using IdelPog.Currency.Service.Interface;
using Moq;

namespace IdelPog.Currency.Tests.Mediator
{
    [TestFixture]
    public class CurrencyUpdateMediatorTest
    {
        private IBatchMediator<CurrencyUpdate> _currencyUpdateMediator { get; set; }
        private Mock<IStateRepository<CurrencyType, IdelPog.Currency.Contracts.Currency>> _repositoryMock { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Mock<IDispatchOne<CurrencyUpdateResponse>> _dispatcherMock { get; set; }
        private Mock<ICurrencyUpdateSummarizer> _currencyUpdateSummarizerMock { get; set; }
        private Mock<ICurrencyUpdateResponseFactory> _currencyUpdateDTOFactoryMock { get; set; }

        private IdelPog.Currency.Contracts.Currency _goldCurrency;
        private CurrencyUpdate _addGoldUpdate;
        private CurrencyUpdate _removeGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryMock = new Mock<IStateRepository<CurrencyType, IdelPog.Currency.Contracts.Currency>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _dispatcherMock = new Mock<IDispatchOne<CurrencyUpdateResponse>>();
            _currencyUpdateSummarizerMock = new Mock<ICurrencyUpdateSummarizer>();
            _currencyUpdateDTOFactoryMock = new Mock<ICurrencyUpdateResponseFactory>();

            IHandler throwHandler = new ThrowHandler();
            _currencyUpdateMediator = new CurrencyUpdateMediator(_repositoryMock.Object, _currencyServiceMock.Object, _dispatcherMock.Object,
                _currencyUpdateSummarizerMock.Object, _currencyUpdateDTOFactoryMock.Object,
                new CollectionAssertion(throwHandler), new FoundAssertion(throwHandler), new ObjectNullAssertion(throwHandler));

            _addGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.ADD);
            _removeGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.REMOVE);
            _goldCurrency = new IdelPog.Currency.Contracts.Currency(_addGoldUpdate.CurrencyType, 0);
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _currencyServiceMock.Reset();
            _dispatcherMock.Reset();
            _currencyUpdateSummarizerMock.Reset();
            _goldCurrency.Amount = 0;
        }

        private void TestRunner(IReadOnlyList<CurrencyUpdate> updates, CurrencyUpdate[] summaryUpdates)
        {
            CurrencyUpdateResponse responses = new() { CurrencyUpdates = summaryUpdates };

            _repositoryMock.Setup(library => library.Contains(_addGoldUpdate.CurrencyType)).Returns(true);

            _repositoryMock.Setup(library => library.Get(_addGoldUpdate.CurrencyType)).Returns(_goldCurrency);

            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(updates)).Returns(summaryUpdates);

            _currencyUpdateDTOFactoryMock.Setup(library => library.CreateFrom(summaryUpdates))
                .Returns(responses);

            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages(updates));

            _repositoryMock.Verify(library => library.Contains(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Get(_addGoldUpdate.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Update(_addGoldUpdate.CurrencyType, _goldCurrency), Times.Once);

            _dispatcherMock.Verify(library => library.Dispatch(responses), Times.Once);
            _dispatcherMock.VerifyNoOtherCalls();

            _currencyUpdateSummarizerMock.Verify(library => library.GetSummary(updates), Times.Once);
            _currencyUpdateSummarizerMock.VerifyNoOtherCalls();
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_ProcessCurrencyUpdate_MultipleAddUpdates_AddAmountToCurrency(uint amountOfUpdates)
        {
            IReadOnlyList<CurrencyUpdate> currencyUpdate = Enumerable.Repeat(_addGoldUpdate, (int) amountOfUpdates).ToList();
            CurrencyUpdate[] summaryUpdate =
                [new() { ActionType = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = _addGoldUpdate.Amount * amountOfUpdates }];

            TestRunner(currencyUpdate, summaryUpdate);

            _currencyServiceMock.Verify(library => library.AddAmount(_goldCurrency, _addGoldUpdate.Amount * amountOfUpdates), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_ProcessCurrencyUpdate_MultipleRemoveUpdates_RemovesAmountFromCurrency(uint amountOfUpdates)
        {
            _goldCurrency.Amount = _addGoldUpdate.Amount * amountOfUpdates;
            IReadOnlyList<CurrencyUpdate> currencyUpdate = Enumerable.Repeat(_removeGoldUpdate, (int) amountOfUpdates).ToList();
            CurrencyUpdate[] summaryUpdate =
                [new() { ActionType = ActionType.REMOVE, CurrencyType = CurrencyType.GOLD, Amount = _addGoldUpdate.Amount * amountOfUpdates }];

            TestRunner(currencyUpdate, summaryUpdate);

            _currencyServiceMock.Verify(library => library.RemoveAmount(_goldCurrency, _addGoldUpdate.Amount * amountOfUpdates), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        [TestCase(1u)]
        [TestCase(10u)]
        [TestCase(100u)]
        public void Positive_ProcessCurrencyUpdate_MixedUpdates(uint amountOfUpdates)
        {
            List<CurrencyUpdate> currencyUpdates = [];
            currencyUpdates.AddRange(Enumerable.Repeat(_addGoldUpdate, (int) amountOfUpdates));
            currencyUpdates.AddRange(Enumerable.Repeat(_removeGoldUpdate, (int) amountOfUpdates));
            currencyUpdates.Add(new CurrencyUpdate { ActionType = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = 10 });

            CurrencyUpdate[] summaryUpdates = [new() { ActionType = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = 10 }];

            TestRunner(currencyUpdates, summaryUpdates);

            _currencyServiceMock.Verify(library => library.AddAmount(_goldCurrency, 10), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ProcessCurrencyUpdate_SingleAddUpdate_MultipleCurrencies()
        {
            IdelPog.Currency.Contracts.Currency gems = new(CurrencyType.GEMS, 0);
            CurrencyUpdate addGemsUpdate = new() { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };

            IReadOnlyList<CurrencyUpdate> currencyUpdate = [_addGoldUpdate, addGemsUpdate];
            CurrencyUpdate[] summaryUpdate =
            [
                new() { ActionType = ActionType.ADD, CurrencyType = CurrencyType.GOLD, Amount = _addGoldUpdate.Amount },
                new() { ActionType = ActionType.ADD, CurrencyType = CurrencyType.GEMS, Amount = addGemsUpdate.Amount }
            ];

            _repositoryMock.Setup(library => library.Contains(addGemsUpdate.CurrencyType)).Returns(true);

            _repositoryMock.Setup(library => library.Get(addGemsUpdate.CurrencyType)).Returns(gems);

            TestRunner(currencyUpdate, summaryUpdate);

            _currencyServiceMock.Verify(library => library.AddAmount(_goldCurrency, _addGoldUpdate.Amount), Times.Once);
            _currencyServiceMock.Verify(library => library.AddAmount(gems, addGemsUpdate.Amount), Times.Once);
            _currencyServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_GetSummary_ReturnsNothing_Throws()
        {
            CurrencyUpdate addGemsUpdate = new() { ActionType = ActionType.ADD, Amount = 10, CurrencyType = CurrencyType.GEMS };
            CurrencyUpdate removeGemsUpdate = new() { ActionType = ActionType.REMOVE, Amount = 10, CurrencyType = CurrencyType.GEMS };

            IReadOnlyList<CurrencyUpdate> currencyUpdates = [_addGoldUpdate, _removeGoldUpdate, addGemsUpdate, removeGemsUpdate];

            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(currencyUpdates)).Returns([]);

            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateMediator.HandleMessages(currencyUpdates));

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateMediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_EmptyCollection_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateMediator.HandleMessages([]));

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_CurrencyNotFound_Throws()
        {
            _repositoryMock.Setup(library => library.Contains(_addGoldUpdate.CurrencyType)).Returns(false);
            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(new[] { _addGoldUpdate })).Returns([_addGoldUpdate]);

            NotFoundException<CurrencyType> exception =
                Assert.Throws<NotFoundException<CurrencyType>>(() => _currencyUpdateMediator.HandleMessages([_addGoldUpdate]));

            Assert.That(exception.Key, Is.EqualTo(_addGoldUpdate.CurrencyType));
        }

        [Test]
        public void Negative_ProcessCurrencyUpdate_Remove_NotEnoughCurrency_Throws()
        {
            _goldCurrency.Amount = 1;
            _repositoryMock.Setup(library => library.Contains(_removeGoldUpdate.CurrencyType)).Returns(true);
            _repositoryMock.Setup(library => library.Get(_removeGoldUpdate.CurrencyType)).Returns(_goldCurrency);

            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(new[] { _removeGoldUpdate })).Returns([_removeGoldUpdate]);

            _currencyServiceMock.Setup(library =>
                    library.RemoveAmount(It.Is<IdelPog.Currency.Contracts.Currency>(currency => currency.CurrencyType == _goldCurrency.CurrencyType), _removeGoldUpdate.Amount))
                .Throws(new NotEnoughCurrencyException(_goldCurrency.CurrencyType, _goldCurrency.Amount, _removeGoldUpdate.Amount));

            NotEnoughCurrencyException exception =
                Assert.Throws<NotEnoughCurrencyException>(() => _currencyUpdateMediator.HandleMessages([_removeGoldUpdate]));

            Assert.Multiple(() =>
            {
                Assert.That(exception.CurrencyTypeContext, Is.EqualTo(_removeGoldUpdate.CurrencyType));
                Assert.That(exception.CurrencyAmount, Is.EqualTo(_goldCurrency.Amount));
                Assert.That(exception.RemoveAmount, Is.EqualTo(_removeGoldUpdate.Amount));
            });

            _currencyServiceMock.Verify(library => library.RemoveAmount(_goldCurrency, _removeGoldUpdate.Amount), Times.Once);

            _dispatcherMock.VerifyNoOtherCalls();
            _currencyServiceMock.VerifyNoOtherCalls();
        }
    }
}
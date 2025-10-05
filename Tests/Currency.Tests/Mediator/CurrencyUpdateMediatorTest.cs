using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Mediator;
using IdelPog.Currency.Service.Interface;
using Moq;

namespace IdelPog.Currency.Tests.Mediator
{
    [TestFixture]
    public sealed class CurrencyUpdateMediatorTest
    {
        private IBatchMediator<CurrencyUpdate> _currencyUpdateMediator { get; set; }
        private Mock<IStateRepository<CurrencyType, Contracts.Currency>> _repositoryMock { get; set; }
        private Mock<ICurrencyService> _currencyServiceMock { get; set; }
        private Mock<IDispatchMany<CurrencyUpdateResponse>> _dispatcherMock { get; set; }
        private Mock<ICurrencyUpdateSummarizer> _currencyUpdateSummarizerMock { get; set; }
        private Mock<ICurrencyUpdateResponseFactory> _currencyUpdateDTOFactoryMock { get; set; }

        private Contracts.Currency _goldCurrency;
        private CurrencyUpdate _addGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryMock = new Mock<IStateRepository<CurrencyType, Contracts.Currency>>();
            _currencyServiceMock = new Mock<ICurrencyService>();
            _dispatcherMock = new Mock<IDispatchMany<CurrencyUpdateResponse>>();
            _currencyUpdateSummarizerMock = new Mock<ICurrencyUpdateSummarizer>();
            _currencyUpdateDTOFactoryMock = new Mock<ICurrencyUpdateResponseFactory>();

            ThrowHandler throwHandler = new();
            _currencyUpdateMediator = new CurrencyUpdateMediator(_repositoryMock.Object, _currencyServiceMock.Object, _dispatcherMock.Object,
                _currencyUpdateSummarizerMock.Object, _currencyUpdateDTOFactoryMock.Object,
                new CollectionAssertion(throwHandler), new FoundAssertion(throwHandler));

            _addGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.ADD);
            _goldCurrency = CreateCurrency(_addGoldUpdate.CurrencyType);
        }

        [SetUp]
        public void SetUp()
        {
            _repositoryMock.Reset();
            _currencyServiceMock.Reset();
            _dispatcherMock.Reset();
            _currencyUpdateSummarizerMock.Reset();
        }

        private static Contracts.Currency CreateCurrency(CurrencyType type)
        {
            Contracts.Currency currency = new(type, 0);
            return currency;
        }

        private void SetupSummarizer(CurrencyUpdate[] summerizedUpdates, params CurrencyUpdate[] updates)
        {
            _currencyUpdateSummarizerMock.Setup(library => library.GetSummary(updates)).Returns(summerizedUpdates);
        }

        private void SetupRepositoryMock(Contracts.Currency currency)
        {
            _repositoryMock.Setup(library => library.Contains(currency.CurrencyType)).Returns(true);
            _repositoryMock.Setup(library => library.Get(currency.CurrencyType)).Returns(currency);
        }
        
        private void VerifyRepositoryMock(Contracts.Currency currency)
        {
            _repositoryMock.Verify(library => library.Contains(currency.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Get(currency.CurrencyType), Times.Once);
            _repositoryMock.Verify(library => library.Update(currency.CurrencyType, currency), Times.Once);
        }

        private void VerifyRepositoryNoMoreCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyDispatcher()
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<CurrencyUpdateResponse[]>()), Times.Once);
            VerifyDispatcherNoMoreCalls();
        }

        private void VerifyDispatcherNoMoreCalls()
        {
            _dispatcherMock.VerifyNoOtherCalls();
        }

        private void VerifyCurrencyServiceAdd()
        {
            _currencyServiceMock.Verify(library => library.AddAmount(It.IsAny<Contracts.Currency>(), It.IsAny<uint>()));
        }
        
        private void VerifyCurrencyServiceRemove()
        {
            _currencyServiceMock.Verify(library => library.RemoveAmount(It.IsAny<Contracts.Currency>(), It.IsAny<uint>()));
        }

        private void VerifyCurrencyServiceNoMoreCalls()
        {
            _currencyServiceMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleAddUpdates_AddAmountToCurrency()
        {
            CurrencyUpdate[] updates = [_addGoldUpdate, _addGoldUpdate];
            SetupSummarizer([_addGoldUpdate with { Amount = 20 }], updates);
            SetupRepositoryMock(_goldCurrency);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages(updates));
            
            VerifyRepositoryMock(_goldCurrency);
            VerifyRepositoryNoMoreCalls();
            VerifyDispatcher();
            VerifyCurrencyServiceAdd();
            VerifyCurrencyServiceNoMoreCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleRemoveUpdates_RemovesAmountFromCurrency()
        {
            CurrencyUpdate removeUpdate = _addGoldUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizer([removeUpdate], removeUpdate);
            SetupRepositoryMock(_goldCurrency);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([removeUpdate]));
            
            VerifyRepositoryMock(_goldCurrency);
            VerifyRepositoryNoMoreCalls();
            VerifyDispatcher();
            VerifyCurrencyServiceRemove();
            VerifyCurrencyServiceNoMoreCalls();
        }

        [Test]
        public void Positive_HandleMessages_MixedUpdates()
        {
            CurrencyUpdate removeUpdate = _addGoldUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizer([_addGoldUpdate], removeUpdate, _addGoldUpdate);
            SetupRepositoryMock(_goldCurrency);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([removeUpdate, _addGoldUpdate]));
            
            VerifyRepositoryMock(_goldCurrency);
            VerifyRepositoryNoMoreCalls();
            VerifyDispatcher();
            VerifyCurrencyServiceAdd();
            VerifyCurrencyServiceNoMoreCalls();
        }

        [Test]
        public void Positive_HandleMessages_SingleAddUpdate_MultipleCurrencies()
        {
            Contracts.Currency gemsCurrency = CreateCurrency(CurrencyType.GEMS);
            CurrencyUpdate gemsUpdate = _addGoldUpdate with { CurrencyType = CurrencyType.GEMS };
            SetupSummarizer([_addGoldUpdate, gemsUpdate], gemsUpdate, _addGoldUpdate);
            SetupRepositoryMock(_goldCurrency);
            SetupRepositoryMock(gemsCurrency);
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([gemsUpdate, _addGoldUpdate]));
            
            VerifyRepositoryMock(_goldCurrency);
            VerifyRepositoryMock(gemsCurrency);
            VerifyRepositoryNoMoreCalls();
            VerifyDispatcher();
            VerifyCurrencyServiceAdd();
            VerifyCurrencyServiceNoMoreCalls();
        }

        [Test]
        public void Negative_HandleMessages_GetSummary_ReturnsNothing_Throws()
        {
            SetupSummarizer([], _addGoldUpdate);
            SetupRepositoryMock(_goldCurrency);
            
            Assert.Throws<EmptyCollectionException>(() => _currencyUpdateMediator.HandleMessages([_addGoldUpdate]));
            
            VerifyRepositoryNoMoreCalls();
            VerifyCurrencyServiceNoMoreCalls();
            VerifyDispatcherNoMoreCalls();
        }

        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateMediator.HandleMessages(null!));
            
            VerifyRepositoryNoMoreCalls();
            VerifyCurrencyServiceNoMoreCalls();
            VerifyDispatcherNoMoreCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateMediator.HandleMessages([]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
            
            VerifyRepositoryNoMoreCalls();
            VerifyCurrencyServiceNoMoreCalls();
            VerifyDispatcherNoMoreCalls();
        }

        [Test]
        public void Negative_HandleMessages_CurrencyNotFound_Throws()
        {
            SetupSummarizer([_addGoldUpdate], _addGoldUpdate);
            _repositoryMock.Setup(library => library.Contains(_addGoldUpdate.CurrencyType)).Returns(false);

            Assert.Throws<NotFoundException<CurrencyType>>(() => _currencyUpdateMediator.HandleMessages([_addGoldUpdate]));
            
            _repositoryMock.Verify(library => library.Contains(_addGoldUpdate.CurrencyType));
            VerifyRepositoryNoMoreCalls();
            VerifyCurrencyServiceNoMoreCalls();
            VerifyDispatcherNoMoreCalls();
        }
    }
}
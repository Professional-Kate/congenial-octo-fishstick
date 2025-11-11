using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Mediator;
using IdelPog.Currency.Service.Interface;
using Moq;

namespace IdelPog.Currency.Tests.Mediator
{
    [TestFixture]
    public sealed class CurrencyUpdateMediatorTest
    {
        private IBatchMediator<CurrencyUpdate> _currencyUpdateMediator;
        private Mock<IDispatchMany<CurrencyUpdateResponse>> _dispatcherMock;
        private Mock<ICurrencyUpdateService> _currencyUpdateServiceMock;

        private CurrencyUpdate _addGoldUpdate;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _dispatcherMock = new Mock<IDispatchMany<CurrencyUpdateResponse>>();
            _currencyUpdateServiceMock = new Mock<ICurrencyUpdateService>();

            _currencyUpdateMediator = new CurrencyUpdateMediator(_currencyUpdateServiceMock.Object, _dispatcherMock.Object, new CollectionAssertion());

            _addGoldUpdate = CurrencyUpdateFactory.Create(10, CurrencyType.GOLD, ActionType.ADD);
        }

        [SetUp]
        public void SetUp()
        {
            _dispatcherMock.Reset();
            _currencyUpdateServiceMock.Reset();
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

        private void VerifyUpdateServiceCalled(params CurrencyUpdate[] updates)
        {
            _currencyUpdateServiceMock.Verify(library => library.ApplyUpdates(updates), Times.Once);
            _currencyUpdateServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleAddUpdates_AddAmountToCurrency()
        {
            CurrencyUpdate[] updates = [_addGoldUpdate, _addGoldUpdate];
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages(updates));
            
            VerifyDispatcher();
            VerifyUpdateServiceCalled(updates);
        }

        [Test]
        public void Positive_HandleMessages_MultipleRemoveUpdates_RemovesAmountFromCurrency()
        {
            CurrencyUpdate removeUpdate = _addGoldUpdate with { ActionType = ActionType.REMOVE };
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([removeUpdate]));
            
            VerifyDispatcher();
            VerifyUpdateServiceCalled(removeUpdate);
        }

        [Test]
        public void Positive_HandleMessages_MixedUpdates()
        {
            CurrencyUpdate removeUpdate = _addGoldUpdate with { ActionType = ActionType.REMOVE };
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([removeUpdate, _addGoldUpdate]));
            
            VerifyDispatcher();
            VerifyUpdateServiceCalled(removeUpdate, _addGoldUpdate);
        }

        [Test]
        public void Positive_HandleMessages_SingleAddUpdate_MultipleCurrencies()
        {
            CurrencyUpdate gemsUpdate = _addGoldUpdate with { CurrencyType = CurrencyType.GEMS };
            
            Assert.DoesNotThrow(() => _currencyUpdateMediator.HandleMessages([gemsUpdate, _addGoldUpdate]));
            
            VerifyDispatcher();
            VerifyUpdateServiceCalled(gemsUpdate, _addGoldUpdate);
            
        }

        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyUpdateMediator.HandleMessages(null!));
            
            VerifyDispatcherNoMoreCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyUpdateMediator.HandleMessages([]));
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyUpdate)));
            
            VerifyDispatcherNoMoreCalls();
        }
    }
}
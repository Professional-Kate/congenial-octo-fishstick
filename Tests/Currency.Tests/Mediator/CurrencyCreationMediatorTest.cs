using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Mediator;
using Moq;

namespace IdelPog.Currency.Tests.Mediator
{
    [TestFixture]
    public class CurrencyCreationMediatorTest
    {
        private IBatchMediator<CurrencyCreation> _currencyCreationMediator;
        private Mock<IStateRepository<CurrencyType, Contracts.Currency>> _repositoryMock;
        private Mock<IDispatchMany<CurrencyCreationResponse>> _responseDispatcherMock;
        private Mock<ICurrencyCreationResponseFactory> _responseFactoryMock;

        private CurrencyCreation _createGold;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _repositoryMock = new Mock<IStateRepository<CurrencyType, Contracts.Currency>>();
            _responseDispatcherMock = new Mock<IDispatchMany<CurrencyCreationResponse>>();
            _responseFactoryMock = new Mock<ICurrencyCreationResponseFactory>();
            _currencyCreationMediator = new CurrencyCreationMediator(_repositoryMock.Object, _responseDispatcherMock.Object,
                _responseFactoryMock.Object, new ObjectNullAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()),
                new UniqueAssertion(new ThrowHandler()));

            _createGold = new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 0 };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void SetupRepository(bool contains, CurrencyType currencyType)
        {
            _repositoryMock.Setup(library => library.Contains(currencyType)).Returns(contains);
        }

        private void VerifyRepository(CurrencyType currencyType)
        {
            _repositoryMock.Verify(library => library.Contains(currencyType), Times.Once);
            _repositoryMock.Verify(library => library.Add(currencyType, It.IsAny<Contracts.Currency>()), Times.Once);
        }

        private void VerifyNoMoreRepositoryCalls()
        {
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyDispatcherWasCalled()
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.IsAny<CurrencyCreationResponse[]>()), Times.Once);
            VerifyNoMoreDispatcherCalls();
        }

        private void VerifyNoMoreDispatcherCalls()
        {
            _responseDispatcherMock.VerifyNoOtherCalls();
        }
        
        [TestCase(0u)]
        [TestCase(10u)]
        [TestCase(uint.MaxValue)]
        public void Positive_CreateCurrency_SingleValidCommand_CreatesCurrency(uint amount)
        {
            Assert.DoesNotThrow(() => _currencyCreationMediator.HandleMessages([_createGold with { StartingAmount = amount }]));
            
            VerifyRepository(_createGold.CurrencyType);
            VerifyNoMoreRepositoryCalls();
            VerifyDispatcherWasCalled();
        }

        [Test]
        public void Positive_CreateCurrency_MultipleValidCommands_CreatesCurrency()
        {
            Assert.DoesNotThrow(() => _currencyCreationMediator.HandleMessages([_createGold, _createGold with {  CurrencyType = CurrencyType.GEMS }]));
            
            VerifyRepository(_createGold.CurrencyType);
            VerifyRepository(CurrencyType.GEMS);
            VerifyNoMoreRepositoryCalls();
            VerifyDispatcherWasCalled();
        }

        [Test]
        public void Negative_CreateCurrency_CurrencyAlreadyExists_Throws()
        {
            SetupRepository(true, _createGold.CurrencyType);
            
            Assert.Throws<DuplicateEntityException>(() => _currencyCreationMediator.HandleMessages([_createGold]));
            
            _repositoryMock.Verify(library => library.Contains(_createGold.CurrencyType), Times.Once);
            VerifyNoMoreRepositoryCalls();
            VerifyNoMoreDispatcherCalls();
        }

        [Test]
        public void Negative_CreateCurrency_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyCreationMediator.HandleMessages(null!));
            
            VerifyNoMoreRepositoryCalls();
            VerifyNoMoreDispatcherCalls();
        }

        [Test]
        public void Negative_CreateCurrency_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _currencyCreationMediator.HandleMessages([]));
            
            VerifyNoMoreRepositoryCalls();
            VerifyNoMoreDispatcherCalls();
        }
    }
}
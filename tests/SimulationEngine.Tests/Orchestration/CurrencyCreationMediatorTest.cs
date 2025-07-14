using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;
using Moq;
using Range = Moq.Range;

namespace IdelPogTests.Orchestration
{
    [TestFixture]
    public class CurrencyCreationMediatorTest
    {
        private ICurrencyCreationMediator  _currencyCreationMediator;
        private Mock<IStateRepository<CurrencyType, Currency>> _stateRepositoryMock;
        private Mock<ICurrencyCreationDispatcher>  _currencyCreationDispatcherMock;

        private CurrencyCreation _createGold;
        private CurrencyCreation _createGems;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateRepositoryMock = new Mock<IStateRepository<CurrencyType, Currency>>();
            _currencyCreationDispatcherMock = new Mock<ICurrencyCreationDispatcher>();
            _currencyCreationMediator = new CurrencyCreationMediator(_stateRepositoryMock.Object, _currencyCreationDispatcherMock.Object, new AssertNotNull(new ThrowHandler()), new AssertCollectionNotEmpty(new ThrowHandler()),  new AssertNonDuplicate(new ThrowHandler()), new AssertPositive(new ThrowHandler()));

            _createGold = new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 10 };
            _createGems = new CurrencyCreation { CurrencyType = CurrencyType.GEMS, StartingAmount = 15 };
        }

        [TearDown]
        public void TearDown()
        {
            _stateRepositoryMock.Reset();
            _currencyCreationDispatcherMock.Reset();
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void Positive_CreateCurrency_SingleValidCommand_CreatesCurrency(int amount)
        {
            CurrencyCreation currencyCreation = new() { CurrencyType = CurrencyType.GOLD, StartingAmount = amount };
            _currencyCreationMediator.CreateCurrency([currencyCreation]);
            
            _stateRepositoryMock.Verify(library => library.Add(currencyCreation.CurrencyType, It.IsAny<Currency>()), Times.Once);
            _stateRepositoryMock.Verify(library => library.Contains(currencyCreation.CurrencyType), Times.Once);
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(new[] { currencyCreation }), Times.Once);
        }
        
        [Test]
        public void Positive_CreateCurrency_MultipleValidCommands_CreatesCurrency()
        {
            _currencyCreationMediator.CreateCurrency([_createGold, _createGems]);
            
            _stateRepositoryMock.Verify(library => library.Add(It.IsInRange(CurrencyType.GOLD, CurrencyType.GEMS, Range.Inclusive), It.IsAny<Currency>()), Times.Exactly(2));
            _stateRepositoryMock.Verify(library => library.Contains(It.IsInRange(CurrencyType.GOLD, CurrencyType.GEMS, Range.Inclusive)), Times.Exactly(2));
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(new[] { _createGold, _createGems }), Times.Once);
        }

        [Test]
        public void Negative_CreateCurrency_CurrencyAlreadyExists_Throws()
        {
            _stateRepositoryMock.SetupSequence(library => library.Contains(CurrencyType.GOLD))
                .Returns(false)
                .Returns(true);
            
            _currencyCreationMediator.CreateCurrency([_createGold]);
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(new[] { _createGold }), Times.Once);
            
            Assert.Throws<DuplicateItemException>(() => _currencyCreationMediator.CreateCurrency([_createGold]));
            
            _stateRepositoryMock.Verify(library => library.Add(CurrencyType.GOLD, It.IsAny<Currency>()), Times.Exactly(1));
            _stateRepositoryMock.Verify(library => library.Contains(CurrencyType.GOLD), Times.Exactly(2));
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_CreateCurrency_SingleCommand_NegativeNumber_Throws()
        {
            CurrencyCreation[] creation = [new() { CurrencyType = CurrencyType.GOLD, StartingAmount = -1 }];
            
            NegativeNumberException exception = Assert.Throws<NegativeNumberException>(() => _currencyCreationMediator.CreateCurrency(creation));
            Assert.That(exception.NumberSource, Is.EqualTo(typeof(CurrencyCreation)));
            
            _stateRepositoryMock.VerifyNoOtherCalls();
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_CreateCurrency_DuplicatedRequest_Throws()
        {
            Assert.Throws<DuplicateItemException>(() => _currencyCreationMediator.CreateCurrency([_createGold, _createGold]));
            
            _stateRepositoryMock.Verify(library => library.Contains(CurrencyType.GOLD), Times.Exactly(2));
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();

        }
        
        [Test]
        public void Negative_CreateCurrency_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _currencyCreationMediator.CreateCurrency(null!));
            
            _stateRepositoryMock.VerifyNoOtherCalls();
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();
        }
        
        [Test]
        public void Negative_CreateCurrency_EmptyCollection_Throws()
        {
            EmptyCollectionException exception = Assert.Throws<EmptyCollectionException>(() => _currencyCreationMediator.CreateCurrency([]));
            
            _stateRepositoryMock.VerifyNoOtherCalls();
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();
            
            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyCreation[])));
        }
    }
}
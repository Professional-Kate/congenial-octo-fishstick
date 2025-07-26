using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.SimulationEngine.Currency.Factories;
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
        private ICurrencyCreationMediator _currencyCreationMediator;
        private Mock<IStateRepository<CurrencyType, Currency>> _stateRepositoryMock;
        private Mock<IDispatchMany<CurrencyCreationDTO>> _currencyCreationDispatcherMock;
        private Mock<ICurrencyCreationDTOFactory> _currencyCreationDTOFactoryMock;

        private CurrencyCreation _createGold;
        private CurrencyCreation _createGems;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateRepositoryMock = new Mock<IStateRepository<CurrencyType, Currency>>();
            _currencyCreationDispatcherMock = new Mock<IDispatchMany<CurrencyCreationDTO>>();
            _currencyCreationDTOFactoryMock = new Mock<ICurrencyCreationDTOFactory>();
            _currencyCreationMediator = new CurrencyCreationMediator(_stateRepositoryMock.Object, _currencyCreationDispatcherMock.Object,
                _currencyCreationDTOFactoryMock.Object, new ObjectNullAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()),
                new UniqueAssertion(new ThrowHandler()), new NumberAssertion(new ThrowHandler()));

            _createGold = new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 10 };
            _createGems = new CurrencyCreation { CurrencyType = CurrencyType.GEMS, StartingAmount = 15 };
        }

        [TearDown]
        public void TearDown()
        {
            _stateRepositoryMock.Reset();
            _currencyCreationDispatcherMock.Reset();
        }

        public CurrencyCreationDTO[] CurrencyCreationConverter(CurrencyCreation[] currencyCreations)
        {
            List<CurrencyCreationDTO> currencyCreationDTOs = new(currencyCreations.Length);

            foreach (CurrencyCreation currencyCreation in currencyCreations)
            {
                currencyCreationDTOs.Add(new CurrencyCreationDTO { Amount = currencyCreation.StartingAmount, CurrencyType = currencyCreation.CurrencyType });
            }

            return currencyCreationDTOs.ToArray();
        }

        [TestCase(0)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void Positive_CreateCurrency_SingleValidCommand_CreatesCurrency(int amount)
        {
            CurrencyCreation currencyCreation = new() { CurrencyType = CurrencyType.GOLD, StartingAmount = amount };
            CurrencyCreationDTO[] currencyCreationDTOs = CurrencyCreationConverter([currencyCreation]);
            _currencyCreationDTOFactoryMock.Setup(library => library.CreateFrom(new[] { currencyCreation }))
                .Returns(currencyCreationDTOs);

            _currencyCreationMediator.CreateCurrency([currencyCreation]);

            _stateRepositoryMock.Verify(library => library.Add(currencyCreation.CurrencyType, It.IsAny<Currency>()), Times.Once);
            _stateRepositoryMock.Verify(library => library.Contains(currencyCreation.CurrencyType), Times.Once);
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(currencyCreationDTOs), Times.Once);
        }

        [Test]
        public void Positive_CreateCurrency_MultipleValidCommands_CreatesCurrency()
        {
            CurrencyCreation[] currencyCreations = [_createGold, _createGems];
            CurrencyCreationDTO[] currencyCreationDTOs = CurrencyCreationConverter(currencyCreations);
            _currencyCreationDTOFactoryMock.Setup(library => library.CreateFrom(currencyCreations))
                .Returns(currencyCreationDTOs);

            _currencyCreationMediator.CreateCurrency([_createGold, _createGems]);

            _stateRepositoryMock.Verify(library => library.Add(It.IsInRange(CurrencyType.GOLD, CurrencyType.GEMS, Range.Inclusive), It.IsAny<Currency>()),
                Times.Exactly(2));

            _stateRepositoryMock.Verify(library => library.Contains(It.IsInRange(CurrencyType.GOLD, CurrencyType.GEMS, Range.Inclusive)), Times.Exactly(2));
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(currencyCreationDTOs), Times.Once);
        }

        [Test]
        public void Negative_CreateCurrency_CurrencyAlreadyExists_Throws()
        {
            CurrencyCreationDTO[] currencyCreationDTOs = CurrencyCreationConverter([_createGold]);
            _currencyCreationDTOFactoryMock.Setup(library => library.CreateFrom(new[] { _createGold }))
                .Returns(currencyCreationDTOs);

            _stateRepositoryMock.SetupSequence(library => library.Contains(CurrencyType.GOLD))
                .Returns(false)
                .Returns(true);

            _currencyCreationMediator.CreateCurrency([_createGold]);
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(currencyCreationDTOs), Times.Once);

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
            Assert.That(exception.Number, Is.EqualTo(-1));

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

            Assert.That(exception.CollectionType, Is.EqualTo(typeof(CurrencyCreation)));
        }
    }
}
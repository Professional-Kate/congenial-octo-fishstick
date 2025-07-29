using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Currency.Responses;
using IdelPog.SimulationEngine.Models;
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
        private Mock<IDispatchMany<CurrencyCreationResponse>> _currencyCreationDispatcherMock;
        private Mock<ICurrencyCreationResponseFactory> _currencyCreationDTOFactoryMock;

        private CurrencyCreation _createGold;
        private CurrencyCreation _createGems;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _stateRepositoryMock = new Mock<IStateRepository<CurrencyType, Currency>>();
            _currencyCreationDispatcherMock = new Mock<IDispatchMany<CurrencyCreationResponse>>();
            _currencyCreationDTOFactoryMock = new Mock<ICurrencyCreationResponseFactory>();
            _currencyCreationMediator = new CurrencyCreationMediator(_stateRepositoryMock.Object, _currencyCreationDispatcherMock.Object,
                _currencyCreationDTOFactoryMock.Object, new ObjectNullAssertion(new ThrowHandler()), new CollectionAssertion(new ThrowHandler()),
                new UniqueAssertion(new ThrowHandler()));

            _createGold = new CurrencyCreation { CurrencyType = CurrencyType.GOLD, StartingAmount = 10 };
            _createGems = new CurrencyCreation { CurrencyType = CurrencyType.GEMS, StartingAmount = 15 };
        }

        [TearDown]
        public void TearDown()
        {
            _stateRepositoryMock.Reset();
            _currencyCreationDispatcherMock.Reset();
        }

        public CurrencyCreationResponse[] CurrencyCreationConverter(CurrencyCreation[] currencyCreations)
        {
            List<CurrencyCreationResponse> currencyCreationDTOs = new(currencyCreations.Length);

            foreach (CurrencyCreation currencyCreation in currencyCreations)
            {
                currencyCreationDTOs.Add(new CurrencyCreationResponse { Amount = currencyCreation.StartingAmount, CurrencyType = currencyCreation.CurrencyType });
            }

            return currencyCreationDTOs.ToArray();
        }

        [TestCase(0u)]
        [TestCase(10u)]
        [TestCase(uint.MaxValue)]
        public void Positive_CreateCurrency_SingleValidCommand_CreatesCurrency(uint amount)
        {
            CurrencyCreation currencyCreation = new() { CurrencyType = CurrencyType.GOLD, StartingAmount = amount };
            CurrencyCreationResponse[] currencyCreationDTOs = CurrencyCreationConverter([currencyCreation]);
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
            CurrencyCreationResponse[] currencyCreationDTOs = CurrencyCreationConverter(currencyCreations);
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
            CurrencyCreationResponse[] currencyCreationDTOs = CurrencyCreationConverter([_createGold]);
            _currencyCreationDTOFactoryMock.Setup(library => library.CreateFrom(new[] { _createGold }))
                .Returns(currencyCreationDTOs);

            _stateRepositoryMock.SetupSequence(library => library.Contains(CurrencyType.GOLD))
                .Returns(false)
                .Returns(true);

            _currencyCreationMediator.CreateCurrency([_createGold]);
            _currencyCreationDispatcherMock.Verify(library => library.Dispatch(currencyCreationDTOs), Times.Once);

            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _currencyCreationMediator.CreateCurrency([_createGold]));
            Assert.That(exception.ID, Is.EqualTo(_createGold));

            _stateRepositoryMock.Verify(library => library.Add(CurrencyType.GOLD, It.IsAny<Currency>()), Times.Exactly(1));
            _stateRepositoryMock.Verify(library => library.Contains(CurrencyType.GOLD), Times.Exactly(2));
            _currencyCreationDispatcherMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Negative_CreateCurrency_DuplicatedRequest_Throws()
        {
            DuplicateEntityException exception =
                Assert.Throws<DuplicateEntityException>(() => _currencyCreationMediator.CreateCurrency([_createGold, _createGold]));

            Assert.That(exception.ID, Is.EqualTo(_createGold));

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
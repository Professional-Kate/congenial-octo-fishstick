using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class CombatantCreationMediatorTest
    {
        private CombatantCreationMediator _mediator;
        private Mock<ICombatantRepository> _repositoryMock;
        private Mock<ICombatantEntityFactory> _factoryMock;
        private Mock<IDispatchMany<CombatantCreationResponse>> _responseDispatcherMock;
        
        private CombatantCreation _combatantCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            _factoryMock = new Mock<ICombatantEntityFactory>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantCreationResponse>>();
            
            _mediator = new CombatantCreationMediator(_repositoryMock.Object, _factoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new CardAsserter(new NumberAssertion()));

            _combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.BEAR);
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _factoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyMocks()
        {
            _repositoryMock.VerifyAll();
            _repositoryMock.VerifyNoOtherCalls();
            _factoryMock.VerifyAll();
            _factoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.VerifyAll();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupFactory(CombatantCreation combatantCreation, byte combatantID)
        {
            _factoryMock.Setup(library => library.CreateEntity(combatantCreation, combatantID)).Returns(TestCombatantEntityFactory.CreateCombatantEntity(combatantID, true, combatantCreation));
        }

        private void VerifyRepository(CombatantType combatantType)
        {
            _repositoryMock.Verify(library => library.Add(It.Is<CombatantEntity>(entity => entity.CombatantType == combatantType)), Times.Once);
        }

        private void VerifyDispatcher(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        private void SetupRepositoryNextCombatantID()
        {
            _repositoryMock.SetupSequence(library => library.NextCombatantID).Returns(1).Returns(2);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesSingleCombatant()
        { 
            SetupRepositoryNextCombatantID();
            SetupFactory(_combatantCreation, 1);
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantCreation]));

            VerifyDispatcher(1);
            VerifyRepository(_combatantCreation.CombatantType);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesCombatants()
        {
            SetupRepositoryNextCombatantID();
            CombatantCreation humanCreation = _combatantCreation with { CombatantType = CombatantType.HUMAN };
            SetupFactory(_combatantCreation,1);
            SetupFactory(humanCreation, 2);
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantCreation, humanCreation]));

            VerifyDispatcher(2);
            VerifyRepository(_combatantCreation.CombatantType);
            VerifyRepository(humanCreation.CombatantType);
            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));

            VerifyMocks();
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));

            VerifyMocks();
        }

        [Test]
        public void Negative_HandleMessages_CombatantHasZeroSpeed_Throws()
        {
            AgilityCard zeroSpeedCard = _combatantCreation.AgilityCard with { Speed = 0 };
            CombatantCreation zeroSpeedCombatant = _combatantCreation with { AgilityCard = zeroSpeedCard };

            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([zeroSpeedCombatant]));
            Assert.That(exception.Source, Is.EqualTo(nameof(zeroSpeedCard.Speed)));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_HandleMessages_CombatantHasZeroHealth_Throws()
        {
            StatCard zeroHealthStatCard = _combatantCreation.StatCard with { Health = 0 };
            CombatantCreation zeroHealthCombatant = _combatantCreation with { StatCard =  zeroHealthStatCard };

            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([zeroHealthCombatant]));
            Assert.That(exception.Source, Is.EqualTo(nameof(zeroHealthStatCard.Health)));
            
            VerifyMocks();
        }
    }
}
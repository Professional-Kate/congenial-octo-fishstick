using IdelPog.Combat.Assertion;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Response;
using IdelPog.Combat.Combatant.Mediator;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class CombatantCreationMediatorTest
    {
        private CombatantCreationMediator _mediator;
        private Mock<IIncrementalRepository<CombatantDefinition>> _repositoryMock;
        private Mock<IDispatchMany<CombatantCreationResponse>> _responseDispatcherMock;
        
        private CombatantCreation _combatantCreation;
        private CombatantDefinition _combatantDefinition;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<IIncrementalRepository<CombatantDefinition>>();
            _responseDispatcherMock = new Mock<IDispatchMany<CombatantCreationResponse>>();
            
            _mediator = new CombatantCreationMediator(_repositoryMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), new CardAsserter(new NumberAssertion()));

            _combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.BEAR);
            _combatantDefinition = new CombatantDefinition
            {
                CombatantID = 0,
                AgilityCard = _combatantCreation.AgilityCard,
                StatCard = _combatantCreation.StatCard,
                CombatantType = _combatantCreation.CombatantType
            };
        }

        [SetUp]
        public void Setup()
        {
            _repositoryMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _repositoryMock.VerifyAll();
            _repositoryMock.VerifyNoOtherCalls();
            _responseDispatcherMock.VerifyAll();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupRepositoryGetID()
        { 
            _repositoryMock.Setup(library => library.GetID()).Returns(0).Verifiable();
        }

        private void VerifyRepositoryAdd(CombatantDefinition combatantDefinition)
        {
            _repositoryMock.Verify(library => library.Add(combatantDefinition), Times.Once);
        }

        private void VerifyDispatcher(int length)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<CombatantCreationResponse[]>(collection => collection.Length == length)), Times.Once);
        }

        [Test]
        public void Positive_HandleMessages_CreatesSingleCombatant()
        {
            SetupRepositoryGetID();
                
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantCreation]));

            VerifyRepositoryAdd(_combatantDefinition);
            VerifyDispatcher(1);
        }
        
        [Test]
        public void Positive_HandleMessages_CreatesCombatants()
        {
            SetupRepositoryGetID();
            
            CombatantCreation humanCreation = _combatantCreation with { CombatantType = CombatantType.HUMAN };
            
            Assert.DoesNotThrow(() => _mediator.HandleMessages([_combatantCreation, humanCreation]));

            VerifyDispatcher(2);
            VerifyRepositoryAdd(_combatantDefinition);
            VerifyRepositoryAdd(_combatantDefinition with { CombatantType = CombatantType.HUMAN });
        }

        [Test]
        public void Negative_HandleMessages_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _mediator.HandleMessages([]));
        }
        
        [Test]
        public void Negative_HandleMessages_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _mediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessages_CombatantHasZeroSpeed_Throws()
        {
            AgilityCard zeroSpeedCard = _combatantCreation.AgilityCard with { Speed = 0 };
            CombatantCreation zeroSpeedCombatant = _combatantCreation with { AgilityCard = zeroSpeedCard };

            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([zeroSpeedCombatant]));
            Assert.That(exception.Source, Is.EqualTo(nameof(zeroSpeedCard.Speed)));
        }
        
        [Test]
        public void Negative_HandleMessages_CombatantHasZeroHealth_Throws()
        {
            StatCard zeroHealthStatCard = _combatantCreation.StatCard with { Health = 0 };
            CombatantCreation zeroHealthCombatant = _combatantCreation with { StatCard =  zeroHealthStatCard };

            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _mediator.HandleMessages([zeroHealthCombatant]));
            Assert.That(exception.Source, Is.EqualTo(nameof(zeroHealthStatCard.Health)));
        }
    }
}
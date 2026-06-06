using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Combat.Service.Queue.Interface;
using IdelPog.Core.Contracts;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class BasicEncounterDeckMediatorTest
    {
        private BasicEncounterDeckMediator _basicEncounterDeckMediator;
        private Mock<IFriendlyStatusAssigner> _friendlyStatusAssignerMock;
        private Mock<IInitialAbilityScheduler> _attackSchedulerMock;
        private Mock<ICombatQueueRunner> _combatQueueRunnerMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        private Mock<ITearDownService> _tearDownServiceMock;
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;
        private CombatantStateChange _combatantStateChange;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyStatusAssignerMock = new Mock<IFriendlyStatusAssigner>();
            _attackSchedulerMock = new Mock<IInitialAbilityScheduler>();
            _combatQueueRunnerMock = new Mock<ICombatQueueRunner>();
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            _tearDownServiceMock = new Mock<ITearDownService>();
            _responseDispatcherMock = new Mock<IDispatchMany<BasicEncounterDeckResponse>>();
            
            _basicEncounterDeckMediator = new BasicEncounterDeckMediator(_friendlyStatusAssignerMock.Object, _attackSchedulerMock.Object, _combatQueueRunnerMock.Object, _combatStateServiceMock.Object, _combatantLoggerMock.Object, _responseDispatcherMock.Object, new CollectionAssertion(), _tearDownServiceMock.Object);
            _basicEncounterDeck = new BasicEncounterDeck 
            {
                FriendlyCombatantIDs = [1],
                EnemyCombatantIDs = [2]
            };

            CombatantCreation combatantCreation = new()
            {
                CombatantType = CombatantType.WOLF, Information = new Information { Name = "A", Description = "B" },
                StatCard = new StatCard { Health = 10 },
                AgilityCard = new AgilityCard { Speed = 3, Initiative = 1 }
            };

            AttackingCombatant attackingCombatant = new() { AbilityType = AbilityType.SLASH, CombatantID = 1, DamageDealt = 100 };
            _combatantStateChange = new CombatantStateChange { AttackingCombatant = attackingCombatant, CombatantID = 2, IsAlive = true, IsFriendly = true, CombatantCreation = combatantCreation, Tick = 1 };
        }

        [SetUp]
        public void Setup()
        {
            _friendlyStatusAssignerMock.Reset();
            _attackSchedulerMock.Reset();
            _combatQueueRunnerMock.Reset();
            _combatStateServiceMock.Reset();
            _combatantLoggerMock.Reset();
            _tearDownServiceMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyMocks()
        {
            _friendlyStatusAssignerMock.Verify();
            _friendlyStatusAssignerMock.VerifyNoOtherCalls();
            _attackSchedulerMock.Verify();
            _attackSchedulerMock.VerifyNoOtherCalls();
            _combatQueueRunnerMock.Verify();
            _combatQueueRunnerMock.VerifyNoOtherCalls();
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatantLoggerMock.Verify();
            _combatantLoggerMock.VerifyNoOtherCalls();
            _tearDownServiceMock.Verify();
            _tearDownServiceMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupGetStateChanges(params CombatantStateChange[] combatantStateChanges)
        {
            _combatantLoggerMock.Setup(library => library.GetStateChanges()).Returns(combatantStateChanges).Verifiable();
        }

        private void VerifyRunDeck(BasicEncounterDeck basicEncounterDeck, Times times)
        {
            _combatQueueRunnerMock.Verify(library => library.RunDeck(basicEncounterDeck), times);
            _friendlyStatusAssignerMock.Verify(library => library.AssignFriendlyStatus(basicEncounterDeck.FriendlyCombatantIDs, basicEncounterDeck.EnemyCombatantIDs), times);
        }

        private void VerifyMockCalls(Times times)
        {
            _combatStateServiceMock.Verify(library => library.FriendlyVictory, times);
            _combatStateServiceMock.Verify(library => library.Reset(), times);
            _combatantLoggerMock.Verify(library => library.ClearStateChanges(), times);
            _attackSchedulerMock.Verify(library => library.EnqueueInitial(0), times);
            _tearDownServiceMock.Verify(library => library.ResetCombatants(), times);
        }

        private void VerifyDispatchMessages(int count)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<BasicEncounterDeckResponse>>(collection => collection.Count == count)));
        }
        
        [Test]
        public void Positive_HandleMessages_SimulatesCombat_InvokesServices()
        {
            SetupGetStateChanges(_combatantStateChange);
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck]));

            VerifyRunDeck(_basicEncounterDeck, Times.Once());
            VerifyMockCalls(Times.Once());
            VerifyDispatchMessages(1);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleMessages_SimulatesCombat()
        {
            SetupGetStateChanges(_combatantStateChange);
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck, _basicEncounterDeck, _basicEncounterDeck]));

            VerifyRunDeck(_basicEncounterDeck, Times.Exactly(3));
            VerifyMockCalls(Times.Exactly(3));
            VerifyDispatchMessages(3);
            VerifyMocks();
        }

        [Test]
        public void Negative_RunEncounter_EmptyDeck_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([new BasicEncounterDeck { FriendlyCombatantIDs = [], EnemyCombatantIDs = [] }]));

            VerifyMocks();
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyFriendlyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { FriendlyCombatantIDs = [] }]));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyEnemyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { EnemyCombatantIDs = [] }]));
            
            VerifyMocks();
        }
        
        [Test]
        public void Negative_RunEncounter_BadInputCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([]));
            Assert.Throws<ArgumentNullException>(() => _basicEncounterDeckMediator.HandleMessages(null!));
            
            VerifyMocks();
        }
    }
}
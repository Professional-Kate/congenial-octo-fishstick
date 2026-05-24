using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
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
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<ICombatQueueRunner> _combatQueueRunnerMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;
        private CombatantStateChange _combatantStateChange;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyStatusAssignerMock = new Mock<IFriendlyStatusAssigner>();
            _attackSchedulerMock = new Mock<IInitialAbilityScheduler>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _combatQueueRunnerMock = new Mock<ICombatQueueRunner>();
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            _responseDispatcherMock = new Mock<IDispatchMany<BasicEncounterDeckResponse>>();
            
            _basicEncounterDeckMediator = new BasicEncounterDeckMediator(_friendlyStatusAssignerMock.Object, _combatantStoreServiceMock.Object, _attackSchedulerMock.Object, _combatQueueRunnerMock.Object, _combatStateServiceMock.Object, _combatantLoggerMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
            _basicEncounterDeck = new BasicEncounterDeck 
            {
                FriendlyCombatantIDs = [1],
                EnemyCombatantIDs = [2]
            };

            CombatantCreation combatantCreation = new()
            {
                CombatantType = CombatantType.WOLF, Information = new Information { Name = "A", Description = "B" },
                StatCard = new StatCard { Health = 10, Attack = 5 },
                AgilityCard = new AgilityCard { Speed = 3, Initiative = 1 }
            };

            AttackingCombatant attackingCombatant = new() { AbilityType = AbilityType.BASIC_ATTACK, CombatantID = 1, DamageDealt = 100 };
            _combatantStateChange = new CombatantStateChange { AttackingCombatant = attackingCombatant, CombatantID = 2, IsAlive = true, IsFriendly = true, CombatantCreation = combatantCreation, Tick = 1 };
        }

        [SetUp]
        public void Setup()
        {
            _friendlyStatusAssignerMock.Reset();
            _attackSchedulerMock.Reset();
            _combatantStoreServiceMock.Reset();
            _combatQueueRunnerMock.Reset();
            _combatStateServiceMock.Reset();
            _combatantLoggerMock.Reset();
            _responseDispatcherMock.Reset();
        }

        private void VerifyMocks()
        {
            _friendlyStatusAssignerMock.Verify();
            _friendlyStatusAssignerMock.VerifyNoOtherCalls();
            _attackSchedulerMock.Verify();
            _attackSchedulerMock.VerifyNoOtherCalls();
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
            _combatQueueRunnerMock.Verify();
            _combatQueueRunnerMock.VerifyNoOtherCalls();
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatantLoggerMock.Verify();
            _combatantLoggerMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupGetStateChanges(params CombatantStateChange[] combatantStateChanges)
        {
            _combatantLoggerMock.Setup(library => library.GetStateChanges()).Returns(combatantStateChanges).Verifiable();
        }

        private void VerifyAssignFriendlyStatus(BasicEncounterDeck basicEncounterDeck, Times times)
        {
            _friendlyStatusAssignerMock.Verify(library => library.AssignFriendlyStatus(basicEncounterDeck.FriendlyCombatantIDs, basicEncounterDeck.EnemyCombatantIDs), times);
        }

        private void VerifyRunDeck(BasicEncounterDeck basicEncounterDeck, Times times)
        {
            _combatQueueRunnerMock.Verify(library => library.RunDeck(basicEncounterDeck), times);
        }

        private void VerifyMockCalls(Times times)
        {
            _combatStateServiceMock.Verify(library => library.FriendlyVictory, times);
            _combatantLoggerMock.Verify(library => library.ClearStateChanges(), times);
            _combatantStoreServiceMock.Verify(library => library.RegisterInitialTargets(), times);
            _attackSchedulerMock.Verify(library => library.EnqueueInitial(0), times);
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

            VerifyAssignFriendlyStatus(_basicEncounterDeck, Times.Once());
            VerifyRunDeck(_basicEncounterDeck, Times.Once());
            VerifyMockCalls(Times.Once());
            VerifyDispatchMessages(1);
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
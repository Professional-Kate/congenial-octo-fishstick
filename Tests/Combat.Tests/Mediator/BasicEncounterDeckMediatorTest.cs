using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Core.Arena;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Incremental;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Mediator
{
    [TestFixture]
    public sealed class BasicEncounterDeckMediatorTest
    {
        private BasicEncounterDeckMediator _basicEncounterDeckMediator;
        private Mock<IIncrementalRepository<CombatantDefinition>> _combatantDefinitionRepositoryMock;
        private Mock<ICombatArena> _combatArenaMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;
        private CombatStage _combatStage;
        private readonly CombatantDefinition _combatantDefinition = new()
        {
            CombatantID = 1,
            StatCard = new StatCard { Health = 10 },
            AgilityCard = new AgilityCard { Speed = 3, Initiative = 1 },
            CombatantType = CombatantType.GOBLIN
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            _responseDispatcherMock = new Mock<IDispatchMany<BasicEncounterDeckResponse>>();
            _combatantDefinitionRepositoryMock = new Mock<IIncrementalRepository<CombatantDefinition>>();
            _combatArenaMock = new Mock<ICombatArena>();
            
            _basicEncounterDeckMediator = new BasicEncounterDeckMediator(_combatantDefinitionRepositoryMock.Object, _combatArenaMock.Object, _combatStateServiceMock.Object, _combatantLoggerMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
            _basicEncounterDeck = new BasicEncounterDeck 
            {
                FriendlyCombatantIDs = [1],
                EnemyCombatantIDs = [2]
            };

            ReadOnlyCombatant readOnlyCombatant = new()
            {
                InstanceID = 1,
                CombatantID = _combatantDefinition.CombatantID,
                StatCard = _combatantDefinition.StatCard,
                AgilityCard = _combatantDefinition.AgilityCard,
                TargetingType = TargetingType.FRIENDLY,
                IsAlive = true
            };
            
            CombatantStateChange combatantStateChange = new()
            {
                Tick = 10, 
                ReadOnlyAbilityStage = new ReadOnlyAbilityStage { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, Value = 10 },
                TargetCombatants = [ readOnlyCombatant with { InstanceID = 2 }]
            };

            _combatStage = new CombatStage
            {
                AbilityID = 1,
                InitiatingCombatant = readOnlyCombatant,
                CombatantStateChanges = [combatantStateChange]
            };
        }

        [SetUp]
        public void Setup()
        {
            _combatantDefinitionRepositoryMock.Reset();
            _combatArenaMock.Reset();
            _combatStateServiceMock.Reset();
            _combatantLoggerMock.Reset();
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantDefinitionRepositoryMock.Verify();
            _combatantDefinitionRepositoryMock.VerifyNoOtherCalls();
            _combatArenaMock.Verify();
            _combatArenaMock.VerifyNoOtherCalls();
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatantLoggerMock.Verify();
            _combatantLoggerMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
        }

        private void SetupGetStateChanges(params CombatStage[] combatStages)
        {
            _combatantLoggerMock.Setup(library => library.GetStateChanges()).Returns(combatStages).Verifiable();
        }

        private void SetupGetCombatantDefinition(params CombatantDefinition[] combatantDefinitions)
        {
            foreach (CombatantDefinition combatantDefinition in combatantDefinitions)
            {
                _combatantDefinitionRepositoryMock.Setup(library => library.Get(combatantDefinition.CombatantID)).Returns(combatantDefinition).Verifiable();
            }
        }

        private void VerifyRunCombatSimulation(IReadOnlyList<CombatantDefinition> friendlyDefinitions, IReadOnlyList<CombatantDefinition> enemyDefinitions, Times times)
        {
            _combatArenaMock.Verify(library => library.RunCombatSimulation(friendlyDefinitions, enemyDefinitions), times);
        }

        private void VerifyMockCalls(Times times)
        {
            _combatStateServiceMock.Verify(library => library.FriendlyVictory, times);
            _combatStateServiceMock.Verify(library => library.Reset(), times);
            _combatantLoggerMock.Verify(library => library.ClearStateChanges(), times);
        }

        private void VerifyDispatchMessages(int count)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<BasicEncounterDeckResponse>>(collection => collection.Count == count)));
        }
        
        [Test]
        public void Positive_HandleMessages_SimulatesCombat_InvokesServices()
        {
            SetupGetStateChanges(_combatStage);
            SetupGetCombatantDefinition(_combatantDefinition, _combatantDefinition with { CombatantID = 2 });
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck]));

            VerifyRunCombatSimulation([_combatantDefinition], [_combatantDefinition with { CombatantID = 2 }], Times.Once());
            VerifyMockCalls(Times.Once());
            VerifyDispatchMessages(1);
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleMessages_SimulatesCombat()
        {
            SetupGetStateChanges(_combatStage);
            SetupGetCombatantDefinition(_combatantDefinition, _combatantDefinition with { CombatantID = 2 });
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck, _basicEncounterDeck, _basicEncounterDeck]));

            VerifyRunCombatSimulation([_combatantDefinition], [_combatantDefinition with { CombatantID = 2 }], Times.Exactly(3));
            VerifyMockCalls(Times.Exactly(3));
            VerifyDispatchMessages(3);
        }

        [Test]
        public void Negative_RunEncounter_EmptyDeck_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([new BasicEncounterDeck { FriendlyCombatantIDs = [], EnemyCombatantIDs = [] }]));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyFriendlyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { FriendlyCombatantIDs = [] }]));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyEnemyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { EnemyCombatantIDs = [] }]));
        }
        
        [Test]
        public void Negative_RunEncounter_BadInputCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([]));
            Assert.Throws<ArgumentNullException>(() => _basicEncounterDeckMediator.HandleMessages(null!));
        }
    }
}
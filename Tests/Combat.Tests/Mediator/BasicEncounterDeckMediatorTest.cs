using IdelPog.Combat.Combatant.Contracts.Enum;
using IdelPog.Combat.Combatant.Model;
using IdelPog.Combat.Core.Arena;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Contracts.Response;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Mediator;
using IdelPog.Combat.Stat.Contracts.Enum;
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
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;
        private CombatStage _combatStage;
        private readonly CombatantDefinition _combatantDefinition = new()
        {
            CombatantID = 1,
            HealthCard = new HealthCard { Health = 10, BaseHealth = 10 },
            AgilityCard = new AgilityCard { Speed = 3, Initiative = 1 },
            CombatantType = CombatantType.GOBLIN
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _responseDispatcherMock = new Mock<IDispatchMany<BasicEncounterDeckResponse>>();
            _combatantDefinitionRepositoryMock = new Mock<IIncrementalRepository<CombatantDefinition>>();
            _combatArenaMock = new Mock<ICombatArena>();
            
            _basicEncounterDeckMediator = new BasicEncounterDeckMediator(_combatantDefinitionRepositoryMock.Object, _combatArenaMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
            _basicEncounterDeck = new BasicEncounterDeck 
            {
                FriendlyCombatantIDs = [1],
                EnemyCombatantIDs = [2]
            };

            ReadOnlyCombatant readOnlyCombatant = new()
            {
                InstanceID = 1,
                CombatantID = _combatantDefinition.CombatantID,
                HealthCard = _combatantDefinition.HealthCard,
                AgilityCard = _combatantDefinition.AgilityCard,
                TargetingType = TargetingType.FRIENDLY,
                IsAlive = true
            };

            CombatantStateChange combatantStateChange = new()
            {
                Tick = 10,
                ReadOnlyAbilityStage = new ReadOnlyAbilityStage
                {
                    AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.SLASH, Value = 10, CastTime = 1, MaxTargets = 1,
                    ReadOnlyStrategy = new ReadOnlyStrategy
                        { TargetingPreference = TargetingPreference.LOWEST, StatType = StatType.COOLDOWN, TargetingType = TargetingType.FRIENDLY }
                },
                TargetCombatants = [readOnlyCombatant with { InstanceID = 2 }]
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
            _responseDispatcherMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantDefinitionRepositoryMock.Verify();
            _combatantDefinitionRepositoryMock.VerifyNoOtherCalls();
            _combatArenaMock.Verify();
            _combatArenaMock.VerifyNoOtherCalls();
            _responseDispatcherMock.Verify();
            _responseDispatcherMock.VerifyNoOtherCalls();
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

        private void VerifyDispatchMessages(int count)
        {
            _responseDispatcherMock.Verify(library => library.Dispatch(It.Is<IReadOnlyList<BasicEncounterDeckResponse>>(collection => collection.Count == count)));
        }
        
        [Test]
        public void Positive_HandleMessages_SimulatesCombat_InvokesServices()
        {
            SetupGetCombatantDefinition(_combatantDefinition, _combatantDefinition with { CombatantID = 2 });
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck]));

            VerifyRunCombatSimulation([_combatantDefinition], [_combatantDefinition with { CombatantID = 2 }], Times.Once());
            VerifyDispatchMessages(1);
        }
        
        [Test]
        public void Positive_HandleMessages_MultipleMessages_SimulatesCombat()
        {
            SetupGetCombatantDefinition(_combatantDefinition, _combatantDefinition with { CombatantID = 2 });
            
            Assert.DoesNotThrow(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck, _basicEncounterDeck, _basicEncounterDeck]));

            VerifyRunCombatSimulation([_combatantDefinition], [_combatantDefinition with { CombatantID = 2 }], Times.Exactly(3));
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
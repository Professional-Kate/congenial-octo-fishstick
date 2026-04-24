using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class BasicEncounterDeckMediatorTest
    {
        private BasicEncounterDeckMediator _basicEncounterDeckMediator;
        private Mock<ICombatantEntityFactory> _combatantFactoryMock;
        private Mock<IBasicAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IAssetRepository<EventType, IEventResolver>> _repositoryMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFactoryMock = new Mock<ICombatantEntityFactory>();
            _attackSchedulerMock = new Mock<IBasicAttackScheduler>();
            _combatQueueMock = new Mock<ICombatQueue>();
            _repositoryMock = new Mock<IAssetRepository<EventType, IEventResolver>>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            _responseDispatcherMock = new Mock<IDispatchMany<BasicEncounterDeckResponse>>();
            
            _basicEncounterDeckMediator = new BasicEncounterDeckMediator(_combatantFactoryMock.Object, _combatantStoreServiceMock.Object, _attackSchedulerMock.Object, _combatStateServiceMock.Object, _combatQueueMock.Object, _repositoryMock.Object, _combatantLoggerMock.Object, _responseDispatcherMock.Object, new CollectionAssertion());
            _basicEncounterDeck = new BasicEncounterDeck
            {
                FriendlyCombatantCards = [CombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 5, Health = 10, Speed = 5})],
                EnemyCombatantCards = [CombatantCreationFactory.CreateCombatantCreation(CombatantType.WOLF, new StatCard { Attack = 3, Health = 10, Speed = 9 })]
            };
        }
        
        // TODO: test this :) 

        [Test]
        public void Negative_RunEncounter_EmptyDeck_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([new BasicEncounterDeck { FriendlyCombatantCards = [], EnemyCombatantCards = [] }]));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyFriendlyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { FriendlyCombatantCards = [] }]));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyEnemyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _basicEncounterDeckMediator.HandleMessages([_basicEncounterDeck with { EnemyCombatantCards = [] }]));
        }
    }
}
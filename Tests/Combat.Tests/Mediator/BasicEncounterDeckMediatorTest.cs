using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Response;
using IdelPog.Combat.Mediator;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;
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
        private Mock<IBasicAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<ICombatQueueRunner> _combatQueueRunnerMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        private Mock<IDispatchMany<BasicEncounterDeckResponse>> _responseDispatcherMock;
        
        private BasicEncounterDeck _basicEncounterDeck;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _friendlyStatusAssignerMock = new Mock<IFriendlyStatusAssigner>();
            _attackSchedulerMock = new Mock<IBasicAttackScheduler>();
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
        }
        
        // TODO: test this :) 

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
    }
}
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatServiceTest
    {
        private CombatService _combatService;
        private Mock<ICombatantFactory> _combatantFactoryMock;
        private Mock<IAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IEnqueueEvent> _enqueueEventMock;
        
        private BasicEncounterDeck _basicEncounterDeck;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFactoryMock = new Mock<ICombatantFactory>();
            _attackSchedulerMock = new Mock<IAttackScheduler>();
            _combatQueueMock = new Mock<ICombatQueue>();
            _enqueueEventMock = new Mock<IEnqueueEvent>();
            
            _combatService = new CombatService(new CollectionAssertion(), _combatantFactoryMock.Object, _attackSchedulerMock.Object, _combatQueueMock.Object);
            _basicEncounterDeck = new BasicEncounterDeck
            {
                FriendlyCombatantCards = [new CombatantCard { CombatantType = CombatantType.HUMAN, StatCard = new StatCard { Attack = 5, Health = 10, Speed = 5}, IsFriendly = false }],
                EnemyCombatantCards = [new CombatantCard { CombatantType = CombatantType.WOLF, StatCard = new StatCard { Attack = 3, Health = 10, Speed = 9 }, IsFriendly = true }]
            };
        }

        [Test]
        public void Negative_RunEncounter_EmptyDeck_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatService.RunEncounter(new BasicEncounterDeck { FriendlyCombatantCards = [], EnemyCombatantCards = [] }));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyFriendlyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatService.RunEncounter(_basicEncounterDeck with { FriendlyCombatantCards = [] }));
        }
        
        [Test]
        public void Negative_RunEncounter_EmptyEnemyCards_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _combatService.RunEncounter(_basicEncounterDeck with { EnemyCombatantCards = [] }));
        }
    }
}
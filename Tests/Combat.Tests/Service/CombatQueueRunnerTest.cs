using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver.Interface;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Repository.Asset;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatQueueRunnerTest
    {
        private CombatQueueRunner _combatQueueRunner;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IAssetRepository<EventType, IEventResolver>> _resolverRepositoryMock;
        private Mock<IEventResolver> _eventResolverMock;

        private CombatEvent _combatEvent;
        private BasicEncounterDeck _basicEncounterDeck;
        private const uint MAX_ITERATIONS = 3;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatQueueMock = new Mock<ICombatQueue>();
            _resolverRepositoryMock = new Mock<IAssetRepository<EventType, IEventResolver>>();
            _eventResolverMock = new Mock<IEventResolver>();

            _combatEvent = new CombatEvent
            {
                AbilityType = AbilityType.BASIC_ATTACK,
                Tick = 0, 
                AttackerID = 0,
                EventType = EventType.DIRECT_DAMAGE
            };
            
            _combatQueueRunner = new CombatQueueRunner(_combatStateServiceMock.Object, _combatQueueMock.Object, _resolverRepositoryMock.Object)
            {
                MaxIterations = MAX_ITERATIONS
            };

            _basicEncounterDeck = new BasicEncounterDeck { FriendlyCombatantIDs = [0], EnemyCombatantIDs = [1] };
        }

        [SetUp]
        public void Setup()
        {
            _combatStateServiceMock.Reset();
            _resolverRepositoryMock.Reset();
            _combatQueueMock.Reset();
            _eventResolverMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
            _resolverRepositoryMock.Verify();
            _resolverRepositoryMock.VerifyNoOtherCalls();
        }

        private void SetupIsCombatOverSequence()
        {
            _combatStateServiceMock.SetupSequence(library => library.IsCombatOver).Returns(false).Returns(true);
        }

        private void SetupQueueDequeue(CombatEvent combatEvent)
        {
            _combatQueueMock.Setup(library => library.Dequeue()).Returns(combatEvent).Verifiable();
        }

        private void SetupRepositoryGet(IEventResolver resolver, EventType eventType)
        {
            _resolverRepositoryMock.Setup(library => library.Get(eventType)).Returns(resolver).Verifiable();
        }

        private void VerifyIsCombatOverCalled(Times times)
        {
            _combatStateServiceMock.Verify(library => library.IsCombatOver, times);
        }

        private static void VerifyEventResolver(Mock<IEventResolver> eventResolver)
        {
            eventResolver.Verify(library => library.ResolveEvent(It.IsAny<double>(), It.IsAny<byte>(), AbilityType.BASIC_ATTACK), Times.Once);
            eventResolver.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_RunDeck_InvokesEvents_UntilCombatIsOver()
        {
            SetupIsCombatOverSequence();
            SetupQueueDequeue(_combatEvent);
            SetupRepositoryGet(_eventResolverMock.Object, EventType.DIRECT_DAMAGE);
            
            Assert.DoesNotThrow(() => _combatQueueRunner.RunDeck(_basicEncounterDeck));

            VerifyIsCombatOverCalled(Times.Exactly(2));
            VerifyEventResolver(_eventResolverMock);
            VerifyMocks();
        }

        [Test]
        public void Negative_RunDeck_GoesOverMaxIterations_Throws()
        {
            SetupQueueDequeue(_combatEvent);
            SetupRepositoryGet(_eventResolverMock.Object, EventType.DIRECT_DAMAGE);
            
            MaxIterationsException exception = Assert.Throws<MaxIterationsException>(() => _combatQueueRunner.RunDeck(_basicEncounterDeck));
            Assert.Multiple(() =>
            {
                Assert.That(exception.MaxIterations, Is.EqualTo(MAX_ITERATIONS));
                Assert.That(exception.BasicEncounterDeck, Is.EqualTo(_basicEncounterDeck));
            });
        }
    }
}
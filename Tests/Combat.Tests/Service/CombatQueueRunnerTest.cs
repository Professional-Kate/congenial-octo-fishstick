using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Queue;
using IdelPog.Combat.Service.Queue.Interface;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class CombatQueueRunnerTest
    {
        private CombatQueueRunner _combatQueueRunner;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<IAbilityEventHandler> _abilityTriggerHandlerMock;
        
        private ScheduledCombatEvent _abilityExecuteEvent;
        private BasicEncounterDeck _basicEncounterDeck;
        private const uint MAX_ITERATIONS = 3;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _combatQueueMock = new Mock<ICombatQueue>();
            _abilityTriggerHandlerMock = new Mock<IAbilityEventHandler>();
            
            _abilityExecuteEvent = new ScheduledCombatEvent
            {
                AbilityID = 1,
                Tick = 0, 
                CombatantID = 1,
                CombatEventType = CombatEventType.ABILITY_EXECUTE,
                AbilityStageIndex = 0,
                TargetingType = TargetingType.FRIENDLY
            };
            
            _combatQueueRunner = new CombatQueueRunner(_combatStateServiceMock.Object, _combatQueueMock.Object, _abilityTriggerHandlerMock.Object)
            {
                MaxIterations = MAX_ITERATIONS
            };

            _basicEncounterDeck = new BasicEncounterDeck { FriendlyCombatantIDs = [0], EnemyCombatantIDs = [1] };
        }

        [SetUp]
        public void Setup()
        {
            _combatStateServiceMock.Reset();
            _combatQueueMock.Reset();
            _abilityTriggerHandlerMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
            _abilityTriggerHandlerMock.Verify();
            _abilityTriggerHandlerMock.VerifyNoOtherCalls();
        }

        private void SetupIsCombatOverSequence()
        {
            _combatStateServiceMock.SetupSequence(library => library.IsCombatOver).Returns(false).Returns(true);
        }

        private void SetupQueueDequeue(ScheduledCombatEvent scheduledCombatEvent)
        {
            _combatQueueMock.Setup(library => library.Dequeue()).Returns(scheduledCombatEvent).Verifiable();
        }

        private void VerifyIsCombatOverCalled(Times times)
        {
            _combatStateServiceMock.Verify(library => library.IsCombatOver, times);
        }

        private void VerifyTriggerHandler(ScheduledCombatEvent scheduledCombatEvent, Times times)
        {
            _abilityTriggerHandlerMock.Verify(library => library.Handle(scheduledCombatEvent), times);
        }

        [Test]
        public void Positive_RunDeck_InvokesEvents_UntilCombatIsOver()
        {
            SetupIsCombatOverSequence();
            SetupQueueDequeue(_abilityExecuteEvent);
            
            Assert.DoesNotThrow(() => _combatQueueRunner.RunDeck(_basicEncounterDeck));

            VerifyIsCombatOverCalled(Times.Exactly(2));
            VerifyTriggerHandler(_abilityExecuteEvent, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Negative_RunDeck_GoesOverMaxIterations_Throws()
        {
            SetupQueueDequeue(_abilityExecuteEvent);
            
            MaxIterationsException exception = Assert.Throws<MaxIterationsException>(() => _combatQueueRunner.RunDeck(_basicEncounterDeck));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exception.MaxIterations, Is.EqualTo(MAX_ITERATIONS));
                Assert.That(exception.BasicEncounterDeck, Is.EqualTo(_basicEncounterDeck));
            }
            
            VerifyIsCombatOverCalled(Times.Exactly(4));
            VerifyTriggerHandler(_abilityExecuteEvent, Times.Exactly((int) MAX_ITERATIONS));
            VerifyMocks();
        }
    }
}
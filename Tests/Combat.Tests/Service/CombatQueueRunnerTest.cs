using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Exceptions;
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
                InstanceID = 1,
                CombatEventType = CombatEventType.ABILITY_EXECUTE,
                AbilityStageIndex = 0,
                TargetingType = TargetingType.FRIENDLY
            };
            
            _combatQueueRunner = new CombatQueueRunner(_combatStateServiceMock.Object, _combatQueueMock.Object, _abilityTriggerHandlerMock.Object)
            {
                MaxIterations = MAX_ITERATIONS
            };
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
            
            Assert.DoesNotThrow(() => _combatQueueRunner.RunCombat());

            VerifyIsCombatOverCalled(Times.Exactly(2));
            VerifyTriggerHandler(_abilityExecuteEvent, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Negative_RunDeck_GoesOverMaxIterations_Throws()
        {
            SetupQueueDequeue(_abilityExecuteEvent);
            
            MaxIterationsException exception = Assert.Throws<MaxIterationsException>(() => _combatQueueRunner.RunCombat());
            Assert.That(exception.MaxIterations, Is.EqualTo(MAX_ITERATIONS));
            
            VerifyIsCombatOverCalled(Times.Exactly(4));
            VerifyTriggerHandler(_abilityExecuteEvent, Times.Exactly((int) MAX_ITERATIONS));
            VerifyMocks();
        }
    }
}
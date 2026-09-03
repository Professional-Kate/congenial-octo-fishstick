using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Queue.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilityEventSchedulerTest
    {
        private AbilityEventScheduler _abilityEventScheduler;
        private Mock<IAbilityEntityRepository> _combatantAbilityEntityRepositoryMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<ICastingCalculator> _castingCalculatorMock;
        private Mock<IReadyTickSystem> _readyTimeSystemMock;
        
        private CombatantEntity _combatantEntity;
        private AbilityEntity _abilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityEntityRepositoryMock = new Mock<IAbilityEntityRepository>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _combatQueueMock = new Mock<ICombatQueue>();
            _castingCalculatorMock = new Mock<ICastingCalculator>();
            _readyTimeSystemMock = new Mock<IReadyTickSystem>();
            
            _abilityEventScheduler = new AbilityEventScheduler(_combatantAbilityEntityRepositoryMock.Object, _readyTimeSystemMock.Object, _combatantRepositoryMock.Object, _castingCalculatorMock.Object, _combatQueueMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.Create(15, TargetingType.FRIENDLY);
            _abilityEntity = TestAbilityEntityFactory.Create(_combatantEntity.InstanceID, 1);
            _abilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = 0 });
            
            _combatantAbilityEntityRepositoryMock.Reset();
            _combatantRepositoryMock.Reset();
            _combatQueueMock.Reset();
            _castingCalculatorMock.Reset();
            _readyTimeSystemMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatantAbilityEntityRepositoryMock.Verify();
            _combatantAbilityEntityRepositoryMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
            _castingCalculatorMock.Verify();
            _castingCalculatorMock.VerifyNoOtherCalls();
            _readyTimeSystemMock.Verify();
            _readyTimeSystemMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantAbilityEntityGet(AbilityEntity abilityEntity)
        { 
            _combatantAbilityEntityRepositoryMock.Setup(library => library.Get(abilityEntity.InstanceID, abilityEntity.AbilityID)).Returns(abilityEntity).Verifiable();
        }

        private void SetupCombatantEntityGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.InstanceID)).Returns(combatantEntity).Verifiable();
        }

        private static ScheduledCombatEvent CreateExpectedCombatEvent(AbilityEntity abilityEntity, double tick, CombatEventType combatEventType, byte abilityStageIndex)
        {
            return new ScheduledCombatEvent
            {
                AbilityID = abilityEntity.AbilityID, 
                InstanceID = abilityEntity.InstanceID, 
                Tick = tick, 
                CombatEventType = combatEventType,
                AbilityStageIndex = abilityStageIndex,
                TargetingType = TargetingType.FRIENDLY
            };
        }

        private void VerifyQueueEnqueue(ScheduledCombatEvent scheduledCombatEvent)
        { 
            _combatQueueMock.Verify(library => library.Enqueue(scheduledCombatEvent), Times.Once);
        }

        private void VerifyGetNextTick(uint combatantSpeed, uint castSpeed)
        {
            _castingCalculatorMock.Verify(library => library.GetCastDuration(combatantSpeed, castSpeed), Times.Once);
        }

        private void VerifySetNewReadyTime(double currentTick, AbilityEntity abilityEntity, uint combatantSpeed)
        {
            _readyTimeSystemMock.Verify(library => library.SetNextReadyTick(currentTick, abilityEntity, combatantSpeed), Times.Once);
        }

        private static uint GetCombatantSpeed(CombatantEntity combatantEntity) => combatantEntity.GetComponent<AgilityComponent>().Speed;
 
        [Test]
        public void Positive_ScheduleEvent_HasCastTime_EnqueuesCastingEvent()
        {
            const uint castTime = 120u;

            AbilityEntity castTimeEntity = TestAbilityEntityFactory.CreateWithCastTime(15, 1, castTime);
            
            SetupCombatantAbilityEntityGet(castTimeEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(0, castTimeEntity.AbilityID, 0, _combatantEntity.InstanceID));
            
            ScheduledCombatEvent expectedEvent = CreateExpectedCombatEvent(castTimeEntity, castTime, CombatEventType.ABILITY_CAST_COMPLETE, 0);
            _combatQueueMock.Verify(
                library => library.Enqueue(
                    It.Is<ScheduledCombatEvent>(combatEvent => combatEvent.AbilityID == expectedEvent.AbilityID && combatEvent.InstanceID == expectedEvent.InstanceID)), Times.Once);

            VerifyGetNextTick(_combatantEntity.GetComponent<AgilityComponent>().Speed, castTime);
            VerifySetNewReadyTime(0, castTimeEntity, GetCombatantSpeed(_combatantEntity));
            VerifyMocks();
        }

        [Test]
        public void Positive_ScheduleEvent_NoCastTime_EnqueuesAbilityEvent()
        {
            const double forTick = 400d;
            SetupCombatantAbilityEntityGet(_abilityEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(forTick, _abilityEntity.AbilityID, 0, _combatantEntity.InstanceID));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_abilityEntity, forTick, CombatEventType.ABILITY_EXECUTE, 0));
            VerifySetNewReadyTime(forTick, _abilityEntity, GetCombatantSpeed(_combatantEntity));
            VerifyMocks();
        }
        
        [Test]
        public void Positive_ScheduleEvent_SecondStage_NormalProcessing()
        {
            AbilityStage[] combatantStages = 
            [
                new()
                {
                    AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, MaxTargets = 1, Priority = 0, CastTime = 10, Value = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY }
                },
                new()
                {
                    AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, MaxTargets = 1, Priority = 0, CastTime = 0, Value = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, CombatantStatType = CombatantStatType.HEALTH, TargetingType = TargetingType.ENEMY }
                }
            ];
            
            AbilityEntity multipleStagesEntity = TestAbilityEntityFactory.Create(_combatantEntity.InstanceID, 12, combatantStages);
            
            const double forTick = 400d;
            SetupCombatantAbilityEntityGet(multipleStagesEntity);
            SetupCombatantEntityGet(_combatantEntity);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(forTick, multipleStagesEntity.AbilityID, 1, multipleStagesEntity.InstanceID));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(multipleStagesEntity, forTick, CombatEventType.ABILITY_EXECUTE, 1));
            _readyTimeSystemMock.Verify(library => library.SetNextReadyTick(It.IsAny<double>(), It.IsAny<AbilityEntity>(), It.IsAny<uint>()), Times.Never);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueAbilityEvent_EnqueuesAbilityEvent()
        {
            SetupCombatantEntityGet(_combatantEntity);
            const double currentTick = 2345.2242d;
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.EnqueueAbilityExecuteEvent(currentTick, _abilityEntity.AbilityID, 0, _combatantEntity.InstanceID));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_abilityEntity, currentTick, CombatEventType.ABILITY_EXECUTE, 0));
            VerifyMocks();
        }
    }
}
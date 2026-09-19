using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Ability.Service.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Service
{
    [TestFixture]
    public sealed class AbilityEventSchedulerTest
    {
        private AbilityEventScheduler _abilityEventScheduler;
        private Mock<ICombatQueue> _combatQueueMock;
        private Mock<ICastingCalculator> _castingCalculatorMock;
        private Mock<IReadyTickSystem> _readyTimeSystemMock;
        
        private CombatantEntity _combatantEntity;
        private AbilityEntity _abilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatQueueMock = new Mock<ICombatQueue>();
            _castingCalculatorMock = new Mock<ICastingCalculator>();
            _readyTimeSystemMock = new Mock<IReadyTickSystem>();
            
            _abilityEventScheduler = new AbilityEventScheduler(_readyTimeSystemMock.Object, _castingCalculatorMock.Object, _combatQueueMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _combatantEntity = TestCombatantEntityFactory.Create(15, TargetingType.FRIENDLY);
            _abilityEntity = TestAbilityEntityFactory.Create(_combatantEntity.InstanceID, 1);
            _abilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = 0 });
            
            _combatQueueMock.Reset();
            _castingCalculatorMock.Reset();
            _readyTimeSystemMock.Reset();
        }

        private void VerifyMocks()
        {
            _combatQueueMock.Verify();
            _combatQueueMock.VerifyNoOtherCalls();
            _castingCalculatorMock.Verify();
            _castingCalculatorMock.VerifyNoOtherCalls();
            _readyTimeSystemMock.Verify();
            _readyTimeSystemMock.VerifyNoOtherCalls();
        }

        private static ScheduledCombatEvent CreateExpectedCombatEvent(AbilityEntity abilityEntity, double tick, CombatEventType combatEventType, byte abilityStageIndex, CombatantEntity combatantEntity)
        {
            return new ScheduledCombatEvent
            {
                AbilityEntity = abilityEntity,
                CombatantEntity = combatantEntity,
                Tick = tick, 
                CombatEventType = combatEventType,
                AbilityStageIndex = abilityStageIndex,
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

        private static uint GetCombatantSpeed(CombatantEntity combatantEntity) => combatantEntity.GetStat(StatType.SPEED);
 
        [Test]
        public void Positive_ScheduleEvent_HasCastTime_EnqueuesCastingEvent()
        {
            const uint castTime = 120u;

            AbilityEntity castTimeEntity = TestAbilityEntityFactory.CreateWithCastTime(15, 1, castTime);
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(0, castTimeEntity, 0, _combatantEntity));
            
            ScheduledCombatEvent expectedEvent = CreateExpectedCombatEvent(castTimeEntity, castTime, CombatEventType.ABILITY_CAST_COMPLETE, 0, _combatantEntity);
            _combatQueueMock.Verify(
                library => library.Enqueue(
                    It.Is<ScheduledCombatEvent>(combatEvent => combatEvent.AbilityEntity.AbilityID == expectedEvent.AbilityEntity.AbilityID && combatEvent.CombatantEntity.InstanceID == expectedEvent.CombatantEntity.InstanceID)), Times.Once);

            VerifyGetNextTick(_combatantEntity.GetStat(StatType.SPEED), castTime);
            VerifySetNewReadyTime(0, castTimeEntity, GetCombatantSpeed(_combatantEntity));
            VerifyMocks();
        }

        [Test]
        public void Positive_ScheduleEvent_NoCastTime_EnqueuesAbilityEvent()
        {
            const double forTick = 400d;
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(forTick, _abilityEntity, 0, _combatantEntity));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_abilityEntity, forTick, CombatEventType.ABILITY_EXECUTE, 0, _combatantEntity));
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
                    AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.HOLY, MaxTargets = 1, Priority = 0, CastTime = 10, Value = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, StatType = StatType.HEALTH, TargetingType = TargetingType.ENEMY }
                },
                new()
                {
                    AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, MaxTargets = 1, Priority = 0, CastTime = 0, Value = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.HIGHEST, StatType = StatType.HEALTH, TargetingType = TargetingType.ENEMY }
                }
            ];
            
            AbilityEntity multipleStagesEntity = TestAbilityEntityFactory.Create(_combatantEntity.InstanceID, 12, combatantStages);
            
            const double forTick = 400d;
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.ScheduleEvent(forTick, multipleStagesEntity, 1, _combatantEntity));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(multipleStagesEntity, forTick, CombatEventType.ABILITY_EXECUTE, 1, _combatantEntity));
            _readyTimeSystemMock.Verify(library => library.SetNextReadyTick(It.IsAny<double>(), It.IsAny<AbilityEntity>(), It.IsAny<uint>()), Times.Never);
            VerifyMocks();
        }
        
        [Test]
        public void Positive_EnqueueAbilityEvent_EnqueuesAbilityEvent()
        {
            const double currentTick = 2345.2242d;
            
            Assert.DoesNotThrow(() => _abilityEventScheduler.EnqueueAbilityExecuteEvent(currentTick, _abilityEntity, 0, _combatantEntity));
            
            VerifyQueueEnqueue(CreateExpectedCombatEvent(_abilityEntity, currentTick, CombatEventType.ABILITY_EXECUTE, 0, _combatantEntity));
            VerifyMocks();
        }
    }
}
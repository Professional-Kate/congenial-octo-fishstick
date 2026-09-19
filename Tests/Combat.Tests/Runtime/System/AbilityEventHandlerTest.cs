using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Event.Resolver.Interface;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Asset;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityEventHandlerTest
    {
        private AbilityEventHandler _abilityEventHandler;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        private Mock<IAssetRepository<AbilityEffectType, IAbilityEffectResolver>> _resolverRepositoryMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<IAbilityEffectResolver> _abilityEffectResolverMock;
        private Mock<ITriggerAbilityHandler<CombatantCastCompleteData>> _combatantCastingTriggerMock;

        private const double READY_TIME = 100d;

        private ScheduledCombatEvent _executeEvent;
        private ScheduledCombatEvent _castCompleteEvent;
        private readonly CombatantCastCompleteData _friendlyCastCompleteData = new()
        {
            CombatantTargetingType = TargetingType.FRIENDLY
        };
        
        private AbilityEntity _abilityEntity;
        private CombatantEntity _combatantEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            _resolverRepositoryMock = new Mock<IAssetRepository<AbilityEffectType, IAbilityEffectResolver>>();
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _abilityEffectResolverMock = new Mock<IAbilityEffectResolver>();
            _combatantCastingTriggerMock =  new Mock<ITriggerAbilityHandler<CombatantCastCompleteData>>();

            _abilityEventHandler = new AbilityEventHandler(_combatantCastingTriggerMock.Object, _abilityEventSchedulerMock.Object, _resolverRepositoryMock.Object, _combatStateServiceMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _abilityEntity = TestAbilityEntityFactory.Create(1, 1);
            _abilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = READY_TIME });

            _combatantEntity = TestCombatantEntityFactory.Create(1, TargetingType.FRIENDLY);
            
            _executeEvent = new ScheduledCombatEvent
            {
                AbilityEntity = _abilityEntity,
                CombatantEntity = _combatantEntity,
                CombatEventType = CombatEventType.ABILITY_EXECUTE,
                Tick = 2,
                AbilityStageIndex = 0
            };
            
            _castCompleteEvent = new ScheduledCombatEvent
            {
                AbilityEntity = _abilityEntity,
                CombatantEntity = _combatantEntity,
                CombatEventType = CombatEventType.ABILITY_CAST_COMPLETE,
                Tick = 1,
                AbilityStageIndex = 0
            };

            _abilityEventSchedulerMock.Reset();
            _resolverRepositoryMock.Reset();
            _combatStateServiceMock.Reset();
            _combatantCastingTriggerMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
            _resolverRepositoryMock.Verify();
            _resolverRepositoryMock.VerifyNoOtherCalls();
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _combatantCastingTriggerMock.Verify();
            _combatantCastingTriggerMock.VerifyNoOtherCalls();
        }

        private void SetupResolverRepositoryGet(Mock<IAbilityEffectResolver> abilityEffectResolverMock, AbilityEffectType abilityEffectType)
        {
            _resolverRepositoryMock.Setup(library => library.Get(abilityEffectType)).Returns(abilityEffectResolverMock.Object).Verifiable();
        }
        
        private void SetupIsCombatOver(bool isCombatOver)
        {
            _combatStateServiceMock.Setup(library => library.IsCombatOver).Returns(isCombatOver).Verifiable();
        }
        
        private void VerifyEnqueueAbilityEvent(double currentTick, AbilityEntity abilityEntity, CombatantEntity combatantEntity, byte abilityStageIndex = 0)
        {
            _abilityEventSchedulerMock.Verify(library => library.EnqueueAbilityExecuteEvent(currentTick, abilityEntity, abilityStageIndex, combatantEntity), Times.Once);
        }
        
        private void VerifyScheduleEvent(double currentTick, AbilityEntity abilityEntity, CombatantEntity combatantEntity, byte abilityStageIndex = 0)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick + abilityEntity.GetStat(StatType.COOLDOWN), abilityEntity, abilityStageIndex, combatantEntity), Times.Once);
        }

        private void VerifyCombatantCastingHandler(double currentTick, CombatantCastCompleteData combatantCastCompleteData)
        {
            _combatantCastingTriggerMock.Verify(library => library.Handle(currentTick, combatantCastCompleteData), Times.Once);
        }
        
        private static void VerifyResolveEffect(Mock<IAbilityEffectResolver> abilityEffectResolverMock, double tick, AbilityEntity abilityEntity, AbilityStage abilityStage, CombatantEntity initiatingCombatant)
        {
            abilityEffectResolverMock.Verify(library => library.ResolveEffect(tick, abilityEntity, abilityStage, initiatingCombatant), Times.Once);
        }

        private static AbilityStage GetAbilityStage(AbilityEntity abilityEntity, int stage) => abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[stage];

        [Test]
        public void Positive_Handle_CastComplete_EnqueuesNewEvent()
        {
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_castCompleteEvent));
            
            VerifyCombatantCastingHandler(_castCompleteEvent.Tick, _friendlyCastCompleteData);
            VerifyEnqueueAbilityEvent(_castCompleteEvent.Tick, _abilityEntity, _combatantEntity);
        }

        [Test]
        public void Positive_Handle_AbilityExecute_ResolvesAbility()
        {
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _abilityEntity, GetAbilityStage(_abilityEntity, 0), _combatantEntity);
            VerifyScheduleEvent(_executeEvent.Tick, _abilityEntity, _combatantEntity);
        }

        [Test]
        public void Positive_Handle_MultipleStages_IsLastStage()
        {
            AbilityStage[] combatantStages =
                [
                    new()
                    {
                        AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { StatType = StatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    },
                    new()
                    {
                        AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { StatType = StatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    }
                ];

            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(1, 1, combatantStages);
            
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.HEALING);
            SetupIsCombatOver(false);
            
            ScheduledCombatEvent scheduledCombatEvent = _executeEvent with { AbilityEntity = abilityEntity };
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(scheduledCombatEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, scheduledCombatEvent.Tick, abilityEntity, GetAbilityStage(abilityEntity, 1), _combatantEntity);
            VerifyScheduleEvent(scheduledCombatEvent.Tick, abilityEntity, _combatantEntity);
        }
        
        [Test]
        public void Positive_Handle_MultipleStages_IsNotLastStage()
        {
            AbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { StatType = StatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { StatType = StatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStageCard = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 1, Value = 5, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { StatType = StatType.SPEED, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                }
            ];

            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(1, 1, combatantStages);
            
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            
            ScheduledCombatEvent scheduledCombatEvent = _executeEvent with { AbilityEntity = abilityEntity };
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(scheduledCombatEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, scheduledCombatEvent.Tick, abilityEntity, GetAbilityStage(abilityEntity, 1), _combatantEntity);
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(scheduledCombatEvent.Tick, abilityEntity, 2, _combatantEntity), Times.Once);
        }

        [Test]
        public void Positive_Handle_TriggerAbility_DoesNotReschedule()
        {
            TriggerComponent triggerComponent = new()
            {
                TargetingType = TargetingType.FRIENDLY,
                TriggerEventType = TriggerEventType.COMBATANT_DAMAGED,
                MinTriggerValue = 5,
                MaxTriggerValue = 10
            };
                
            _abilityEntity.ReplaceComponent(triggerComponent);
            
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _abilityEntity, GetAbilityStage(_abilityEntity, 0), _combatantEntity);
        }
    }
}
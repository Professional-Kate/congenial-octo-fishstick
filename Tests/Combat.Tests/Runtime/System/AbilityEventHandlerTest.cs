using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Core.Event.Resolver.Interface;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;
using IdelPog.Combat.Core.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Asset;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityEventHandlerTest
    {
        private AbilityEventHandler _abilityEventHandler;
        private Mock<IAbilityEntityRepository> _combatantAbilityEntityRepositoryMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        private Mock<IAssetRepository<AbilityEffectType, IAbilityEffectResolver>> _resolverRepositoryMock;
        private Mock<ICombatStateService> _combatStateServiceMock;
        private Mock<IAbilityEffectResolver> _abilityEffectResolverMock;
        private Mock<ITriggerAbilityHandler<CombatantCastCompleteData>> _combatantCastingTriggerMock;
        private Mock<IReadyTickSystem> _readyTickSystemMock;

        private const double READY_TIME = 100d;
        
        private readonly ScheduledCombatEvent _executeEvent = new()
        {
            AbilityID = 1,
            InstanceID = 1,
            CombatEventType = CombatEventType.ABILITY_EXECUTE,
            Tick = 2,
            AbilityStageIndex = 0,
            TargetingType = TargetingType.FRIENDLY
        };
        
        private readonly ScheduledCombatEvent _castCompleteEvent = new()
        {
            AbilityID = 1,
            InstanceID = 1,
            CombatEventType = CombatEventType.ABILITY_CAST_COMPLETE,
            Tick = 1,
            AbilityStageIndex = 0,
            TargetingType = TargetingType.FRIENDLY
        };

        private readonly CombatantCastCompleteData _friendlyCastCompleteData = new()
        {
            CastingCombatantID = 1, 
            CombatantTargetingType = TargetingType.FRIENDLY
        };
        
        private AbilityEntity _abilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityEntityRepositoryMock = new Mock<IAbilityEntityRepository>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            _resolverRepositoryMock = new Mock<IAssetRepository<AbilityEffectType, IAbilityEffectResolver>>();
            _combatStateServiceMock = new Mock<ICombatStateService>();
            _abilityEffectResolverMock = new Mock<IAbilityEffectResolver>();
            _combatantCastingTriggerMock =  new Mock<ITriggerAbilityHandler<CombatantCastCompleteData>>();
            _readyTickSystemMock = new Mock<IReadyTickSystem>();

            _abilityEventHandler = new AbilityEventHandler(_combatantAbilityEntityRepositoryMock.Object, _combatantCastingTriggerMock.Object, _abilityEventSchedulerMock.Object, _resolverRepositoryMock.Object, _combatStateServiceMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _abilityEntity = TestAbilityEntityFactory.Create(_executeEvent.InstanceID, _executeEvent.AbilityID);
            _abilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = READY_TIME });

            _combatantAbilityEntityRepositoryMock.Reset();
            _abilityEventSchedulerMock.Reset();
            _resolverRepositoryMock.Reset();
            _combatStateServiceMock.Reset();
            _abilityEffectResolverMock.Reset();
            _combatantCastingTriggerMock.Reset();
            _readyTickSystemMock.Reset();
        }

        [TearDown]
        public void TearDown()
        {
            _combatantAbilityEntityRepositoryMock.Verify();
            _combatantAbilityEntityRepositoryMock.VerifyNoOtherCalls();
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
            _resolverRepositoryMock.Verify();
            _resolverRepositoryMock.VerifyNoOtherCalls();
            _combatStateServiceMock.Verify();
            _combatStateServiceMock.VerifyNoOtherCalls();
            _abilityEffectResolverMock.Verify();
            _abilityEffectResolverMock.VerifyNoOtherCalls();
            _combatantCastingTriggerMock.Verify();
            _combatantCastingTriggerMock.VerifyNoOtherCalls();
            _readyTickSystemMock.Verify();
            _readyTickSystemMock.VerifyNoOtherCalls();
        }

        private void SetupCombatantAbilityEntityGet(AbilityEntity abilityEntity)
        {
            _combatantAbilityEntityRepositoryMock.Setup(library => library.Get(abilityEntity.InstanceID, abilityEntity.AbilityID)).Returns(abilityEntity).Verifiable();
        }

        private void SetupResolverRepositoryGet(Mock<IAbilityEffectResolver> abilityEffectResolverMock, AbilityEffectType abilityEffectType)
        {
            _resolverRepositoryMock.Setup(library => library.Get(abilityEffectType)).Returns(abilityEffectResolverMock.Object).Verifiable();
        }
        
        private void SetupIsCombatOver(bool isCombatOver)
        {
            _combatStateServiceMock.Setup(library => library.IsCombatOver).Returns(isCombatOver).Verifiable();
        }
        
        private void VerifyEnqueueAbilityEvent(double currentTick, AbilityEntity abilityEntity, byte abilityStageIndex = 0)
        {
            _abilityEventSchedulerMock.Verify(library => library.EnqueueAbilityExecuteEvent(currentTick, abilityEntity.AbilityID, abilityStageIndex, abilityEntity.InstanceID), Times.Once);
        }
        
        private void VerifyScheduleEvent(double currentTick, AbilityEntity abilityEntity, byte abilityStageIndex = 0)
        {
            CooldownComponent cooldownComponent = abilityEntity.GetComponent<CooldownComponent>();
            
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick + cooldownComponent.Cooldown, abilityEntity.AbilityID, abilityStageIndex, abilityEntity.InstanceID), Times.Once);
        }

        private void VerifyCombatantCastingHandler(double currentTick, CombatantCastCompleteData combatantCastCompleteData)
        {
            _combatantCastingTriggerMock.Verify(library => library.Handle(currentTick, combatantCastCompleteData), Times.Once);
        }
        
        private static void VerifyResolveEffect(Mock<IAbilityEffectResolver> abilityEffectResolverMock, double tick, AbilityEntity abilityEntity, AbilityStage abilityStage)
        {
            abilityEffectResolverMock.Verify(library => library.ResolveEffect(tick, abilityEntity, abilityStage), Times.Once);
        }

        private static AbilityStage GetAbilityStage(AbilityEntity abilityEntity, int stage) => abilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[stage];

        [Test]
        public void Positive_Handle_CastComplete_EnqueuesNewEvent()
        {
            SetupCombatantAbilityEntityGet(_abilityEntity);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_castCompleteEvent));
            
            VerifyCombatantCastingHandler(_castCompleteEvent.Tick, _friendlyCastCompleteData);
            VerifyEnqueueAbilityEvent(_castCompleteEvent.Tick, _abilityEntity);
        }

        [Test]
        public void Positive_Handle_AbilityExecute_ResolvesAbility()
        {
            SetupCombatantAbilityEntityGet(_abilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _abilityEntity, GetAbilityStage(_abilityEntity, 0));
            VerifyScheduleEvent(_executeEvent.Tick, _abilityEntity);
        }

        [Test]
        public void Positive_Handle_MultipleStages_IsLastStage()
        {
            AbilityStage[] combatantStages =
                [
                    new()
                    {
                        AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    },
                    new()
                    {
                        AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    }
                ];

            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(_executeEvent.InstanceID, _executeEvent.AbilityID, combatantStages);
            
            SetupCombatantAbilityEntityGet(abilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.HEALING);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, abilityEntity, GetAbilityStage(abilityEntity, 1));
            VerifyScheduleEvent(_executeEvent.Tick, abilityEntity);
        }
        
        [Test]
        public void Positive_Handle_MultipleStages_IsNotLastStage()
        {
            AbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStageCards = new AbilityStageCard { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 1, Value = 5, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.SPEED, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                }
            ];

            AbilityEntity abilityEntity = TestAbilityEntityFactory.Create(_executeEvent.InstanceID, _executeEvent.AbilityID, combatantStages);
            
            SetupCombatantAbilityEntityGet(abilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, abilityEntity, GetAbilityStage(abilityEntity, 1));
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(_executeEvent.Tick, abilityEntity.AbilityID, 2, abilityEntity.InstanceID), Times.Once);
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
            
            SetupCombatantAbilityEntityGet(_abilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _abilityEntity, GetAbilityStage(_abilityEntity, 0));
        }
    }
}
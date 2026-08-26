using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event;
using IdelPog.Combat.Runtime.Event.Resolver.Interface;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Repository.Asset;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class AbilityEventHandlerTest
    {
        private AbilityEventHandler _abilityEventHandler;
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityEntityRepositoryMock;
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
            CombatantID = 1,
            CombatEventType = CombatEventType.ABILITY_EXECUTE,
            Tick = 2,
            AbilityStageIndex = 0,
            TargetingType = TargetingType.FRIENDLY
        };
        
        private readonly ScheduledCombatEvent _castCompleteEvent = new()
        {
            AbilityID = 1,
            CombatantID = 1,
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
        
        private CombatantAbilityEntity _combatantAbilityEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
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
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(_executeEvent.CombatantID, _executeEvent.AbilityID);
            _combatantAbilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = READY_TIME });

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

        private void SetupCombatantAbilityEntityGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _combatantAbilityEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityID)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void SetupResolverRepositoryGet(Mock<IAbilityEffectResolver> abilityEffectResolverMock, AbilityEffectType abilityEffectType)
        {
            _resolverRepositoryMock.Setup(library => library.Get(abilityEffectType)).Returns(abilityEffectResolverMock.Object).Verifiable();
        }
        
        private void SetupIsCombatOver(bool isCombatOver)
        {
            _combatStateServiceMock.Setup(library => library.IsCombatOver).Returns(isCombatOver).Verifiable();
        }
        
        private void VerifyEnqueueAbilityEvent(double currentTick, CombatantAbilityEntity combatantAbilityEntity, byte abilityStageIndex = 0)
        {
            _abilityEventSchedulerMock.Verify(library => library.EnqueueAbilityExecuteEvent(currentTick, combatantAbilityEntity.AbilityID, abilityStageIndex, combatantAbilityEntity.CombatantID), Times.Once);
        }
        
        private void VerifyScheduleEvent(double currentTick, CombatantAbilityEntity combatantAbilityEntity, byte abilityStageIndex = 0)
        {
            CooldownComponent cooldownComponent = combatantAbilityEntity.GetComponent<CooldownComponent>();
            
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(currentTick + cooldownComponent.Cooldown, combatantAbilityEntity.AbilityID, abilityStageIndex, combatantAbilityEntity.CombatantID), Times.Once);
        }

        private void VerifyCombatantCastingHandler(double currentTick, CombatantCastCompleteData combatantCastCompleteData)
        {
            _combatantCastingTriggerMock.Verify(library => library.Handle(currentTick, combatantCastCompleteData), Times.Once);
        }
        
        private static void VerifyResolveEffect(Mock<IAbilityEffectResolver> abilityEffectResolverMock, double tick, CombatantAbilityEntity combatantAbilityEntity, CombatantAbilityStage combatantAbilityStage)
        {
            abilityEffectResolverMock.Verify(library => library.ResolveEffect(tick, combatantAbilityEntity, combatantAbilityStage), Times.Once);
        }

        private static CombatantAbilityStage GetAbilityStage(CombatantAbilityEntity combatantAbilityEntity, int stage) => combatantAbilityEntity.GetComponent<AbilityStagesComponent>().AbilityStages[stage];

        [Test]
        public void Positive_Handle_CastComplete_EnqueuesNewEvent()
        {
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_castCompleteEvent));
            
            VerifyCombatantCastingHandler(_castCompleteEvent.Tick, _friendlyCastCompleteData);
            VerifyEnqueueAbilityEvent(_castCompleteEvent.Tick, _combatantAbilityEntity);
        }

        [Test]
        public void Positive_Handle_AbilityExecute_ResolvesAbility()
        {
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _combatantAbilityEntity, GetAbilityStage(_combatantAbilityEntity, 0));
            VerifyScheduleEvent(_executeEvent.Tick, _combatantAbilityEntity);
        }

        [Test]
        public void Positive_Handle_MultipleStages_IsLastStage()
        {
            CombatantAbilityStage[] combatantStages =
                [
                    new()
                    {
                        AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    },
                    new()
                    {
                        AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                        TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                    }
                ];

            CombatantAbilityEntity combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(_executeEvent.CombatantID, _executeEvent.AbilityID, combatantStages);
            
            SetupCombatantAbilityEntityGet(combatantAbilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.HEALING);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, combatantAbilityEntity, GetAbilityStage(combatantAbilityEntity, 1));
            VerifyScheduleEvent(_executeEvent.Tick, combatantAbilityEntity);
        }
        
        [Test]
        public void Positive_Handle_MultipleStages_IsNotLastStage()
        {
            CombatantAbilityStage[] combatantStages =
            [
                new()
                {
                    AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.FIRE, CastTime = 3, MaxTargets = 1, Value = 2, Priority = 0 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE, AffinityType = AffinityType.LIGHTNING, CastTime = 0, MaxTargets = 1, Value = 2, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.ABILITY_DAMAGE, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                },
                new()
                {
                    AbilityStage = new AbilityStage { AbilityEffectType = AbilityEffectType.HEALING, AffinityType = AffinityType.STAB, CastTime = 0, MaxTargets = 1, Value = 5, Priority = 1 },
                    TargetingPreferenceComponent = new TargetingPreferenceComponent { CombatantStatType = CombatantStatType.SPEED, TargetingPreference = TargetingPreference.HIGHEST, TargetingType = TargetingType.FRIENDLY }
                }
            ];

            CombatantAbilityEntity combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(_executeEvent.CombatantID, _executeEvent.AbilityID, combatantStages);
            
            SetupCombatantAbilityEntityGet(combatantAbilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent with { AbilityStageIndex = 1 }));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, combatantAbilityEntity, GetAbilityStage(combatantAbilityEntity, 1));
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(_executeEvent.Tick, combatantAbilityEntity.AbilityID, 2, combatantAbilityEntity.CombatantID), Times.Once);
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
                
            _combatantAbilityEntity.ReplaceComponent(triggerComponent);
            
            SetupCombatantAbilityEntityGet(_combatantAbilityEntity);
            SetupResolverRepositoryGet(_abilityEffectResolverMock, AbilityEffectType.DIRECT_DAMAGE);
            SetupIsCombatOver(false);
            
            Assert.DoesNotThrow(() => _abilityEventHandler.Handle(_executeEvent));

            VerifyResolveEffect(_abilityEffectResolverMock, _executeEvent.Tick, _combatantAbilityEntity, GetAbilityStage(_combatantAbilityEntity, 0));
        }
    }
}
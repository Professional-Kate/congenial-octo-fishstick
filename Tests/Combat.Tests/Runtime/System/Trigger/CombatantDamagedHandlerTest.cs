using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Handler;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    [TestFixture]
    public sealed class CombatantDamagedHandlerTest : BaseTriggerAbilityHandler
    {
        private CombatantDamagedHandler _combatantDamagedHandler;

        private CombatantDamagedData _friendlyDamagedData;
        
        private AbilityEntity _validAbilityEntity;
        private AbilityEntity _enemyTriggerEntity;
        private CombatantEntity _initiatingCombatant;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantDamagedHandler = new CombatantDamagedHandler(TriggerReaderMock.Object, AbilityEventSchedulerMock.Object, CombatantRepositoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _validAbilityEntity = TestAbilityEntityFactory.Create(FriendlyCombatantEntity.InstanceID, 12);
            _validAbilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 1, MaxTriggerValue = 5 });
            _validAbilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK - 1 });
            
            _enemyTriggerEntity = TestAbilityEntityFactory.Create(EnemyCombatantEntity.InstanceID, 94);
            _enemyTriggerEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.ENEMY, MinTriggerValue = 1, MaxTriggerValue = 5 });
            _enemyTriggerEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK });

            _initiatingCombatant = TestCombatantEntityFactory.Create(1, TargetingType.ENEMY);
            _friendlyDamagedData = new CombatantDamagedData
            {
                DamagedCombatant = FriendlyCombatantEntity,
                DamageValue = 4,
                InitiatingCombatant = _initiatingCombatant
            };
        }

        [Test]
        public void Positive_Handle_FiltersEntities_NothingToSchedule()
        {
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = _friendlyDamagedData.DamageValue + 1 });   
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
        }

        [Test]
        public void Positive_Handle_SelfTargetingEntity_MatchesDamagedCombatant_SchedulesAbility()
        {
            CombatantEntity combatantEntity = FriendlyCombatantEntity with { InstanceID = SelfTargetingEntity.InstanceID };
            
            SetupGetCombatantEntity(combatantEntity);
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            // TargetingType does not matter for SELF targeting
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatant = combatantEntity });
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatant = combatantEntity });
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatant = combatantEntity });
            
            AbilityEventSchedulerMock.Verify(library => library.ScheduleEvent(TICK, SelfTargetingEntity, 0, combatantEntity), Times.Exactly(3));
        }

        [Test]
        public void Positive_Handle_SelfTargetingEntity_DoesNotSkipOtherValidation()
        {
            FriendlyCombatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            SetupGetCombatantEntity(FriendlyCombatantEntity with { InstanceID = SelfTargetingEntity.InstanceID });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
            
            AbilityEventSchedulerMock.Verify(library => library.ScheduleEvent(TICK, SelfTargetingEntity, 0, FriendlyCombatantEntity), Times.Never);
        }

        [Test]
        public void Positive_Handle_ContainsRetaliationComponent_EnqueuesOnComponent()
        {
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = _friendlyDamagedData.DamageValue + 1 });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            const byte capacity = 3;
            FriendlyCombatantEntity.AddComponent(new RetaliationComponent { Capacity = capacity });
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamaged _), Is.False);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
            
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamaged component), Is.True);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(component.InitiatingCombatant, Is.EqualTo(_initiatingCombatant));
                Assert.That(component.DamageValue, Is.EqualTo(_friendlyDamagedData.DamageValue));
            }
        }

        [Test]
        public void Positive_Handle_ContainsRetaliationComponent_ButInitiatingCombatant_IsSameAsDamagedCombatant_DoesNotAddComponent()
        {
            CombatantDamagedData combatantDamagedData = _friendlyDamagedData with { InitiatingCombatant = FriendlyCombatantEntity };
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = combatantDamagedData.DamageValue + 1 });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            const byte capacity = 3;
            FriendlyCombatantEntity.AddComponent(new RetaliationComponent { Capacity = capacity });
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamaged _), Is.False);
            
            _combatantDamagedHandler.Handle(TICK, combatantDamagedData);
            
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamaged _), Is.False);
        }

        [Test]
        public void Positive_Handle_MultipleCorrectEntity_FiltersEverythingElse()
        {
            AbilityEntity validAbility = TestAbilityEntityFactory.Create(EnemyCombatantEntity.InstanceID, 92);
            validAbility.ReplaceComponent(_validAbilityEntity.GetComponent<TriggerComponent>());
            validAbility.AddComponent(_validAbilityEntity.GetComponent<ReadyTickComponent>());
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [_validAbilityEntity, validAbility, NotReadyEntity, _enemyTriggerEntity]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
            
            VerifyScheduleEvent(FriendlyCombatantEntity, _validAbilityEntity);
            VerifyScheduleEvent(EnemyCombatantEntity, validAbility);
        }

        [Test]
        public void Positive_Handle_CombatantIsDead_FiltersAbility()
        {
            FriendlyCombatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [NotReadyEntity, _validAbilityEntity, _enemyTriggerEntity]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
        }
    }
}
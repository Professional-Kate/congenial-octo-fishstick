using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Component;
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

        private readonly CombatantDamagedData _friendlyDamagedData = new()
        {
            DamagedCombatantID = 12,
            DamagedCombatantTargetingType = TargetingType.FRIENDLY,
            DamageValue = 4,
            InitiatingCombatantID = 4
        };
        
        private AbilityEntity _validAbilityEntity;
        private AbilityEntity _enemyTriggerEntity;
        
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
        }

        [Test]
        public void Positive_Handle_FiltersEntities_NothingToSchedule()
        {
            SetupGetCombatantEntity(FriendlyCombatantEntity);
            
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = _friendlyDamagedData.DamageValue + 1 });   
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
        }

        [Test]
        public void Positive_Handle_SelfTargetingEntity_MatchesDamagedCombatant_SchedulesAbility()
        {
            SetupGetCombatantEntity(FriendlyCombatantEntity with { InstanceID = SelfTargetingEntity.InstanceID });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            // TargetingType does not matter for SELF targeting
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatantID = SelfTargetingEntity.InstanceID });
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatantID =  SelfTargetingEntity.InstanceID, DamagedCombatantTargetingType = TargetingType.ENEMY });
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatantID =  SelfTargetingEntity.InstanceID, DamagedCombatantTargetingType = TargetingType.SELF });
            
            AbilityEventSchedulerMock.Verify(library => library.ScheduleEvent(TICK, SelfTargetingEntity.AbilityID, 0, SelfTargetingEntity.InstanceID), Times.Exactly(3));
        }

        [Test]
        public void Positive_Handle_SelfTargetingEntity_DoesNotSkipOtherValidation()
        {
            SetupGetCombatantEntity(FriendlyCombatantEntity with { InstanceID = SelfTargetingEntity.InstanceID });
            SetupGetCombatantEntity(FriendlyCombatantEntity with { InstanceID = 23 });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData with { DamagedCombatantID = 23 });
            
            AbilityEventSchedulerMock.Verify(library => library.ScheduleEvent(TICK, SelfTargetingEntity.AbilityID, 0, SelfTargetingEntity.InstanceID), Times.Never);
        }

        [Test]
        public void Positive_Handle_ContainsRetaliationComponent_EnqueuesOnComponent()
        {
            SetupGetCombatantEntity(FriendlyCombatantEntity);
            
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = _friendlyDamagedData.DamageValue + 1 });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            const byte capacity = 3;
            FriendlyCombatantEntity.AddComponent(new RetaliationComponent { Capacity = capacity });
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamageComponent _), Is.False);
            
            _combatantDamagedHandler.Handle(TICK, _friendlyDamagedData);
            
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamageComponent component), Is.True);
            using (Assert.EnterMultipleScope())
            {
                Assert.That(component.CombatantID, Is.EqualTo(_friendlyDamagedData.InitiatingCombatantID));
                Assert.That(component.DamageValue, Is.EqualTo(_friendlyDamagedData.DamageValue));
            }
        }

        [Test]
        public void Positive_Handle_ContainsRetaliationComponent_ButInitiatingCombatant_IsSameAsDamagedCombatant_DoesNotAddComponent()
        {
            CombatantDamagedData combatantDamagedData = _friendlyDamagedData with { InitiatingCombatantID = FriendlyCombatantEntity.InstanceID };
            SetupGetCombatantEntity(FriendlyCombatantEntity);
            
            SelfTargetingEntity.ReplaceComponent(SelfTargetingEntity.GetComponent<TriggerComponent>() with { MinTriggerValue = combatantDamagedData.DamageValue + 1 });
            SetupTriggerReader(TriggerEventType.COMBATANT_DAMAGED, [SelfTargetingEntity, NotReadyEntity]);
            
            const byte capacity = 3;
            FriendlyCombatantEntity.AddComponent(new RetaliationComponent { Capacity = capacity });
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamageComponent _), Is.False);
            
            _combatantDamagedHandler.Handle(TICK, combatantDamagedData);
            
            Assert.That(FriendlyCombatantEntity.GetComponent<RetaliationComponent>().TryDequeue(out CombatantDamageComponent _), Is.False);
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
            
            VerifyScheduleEvent(_validAbilityEntity.InstanceID, _validAbilityEntity.AbilityID);
            VerifyScheduleEvent(validAbility.InstanceID, validAbility.AbilityID);
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
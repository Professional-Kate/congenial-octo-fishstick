using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Handler;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    [TestFixture]
    public sealed class CombatantCastingHandlerTest : BaseTriggerAbilityHandler
    {
        private CombatantCastingHandler _combatantCastingHandler;

        private readonly CombatantCastCompleteData _friendlyCombatantCastCompleteData = new()
        {
            CastingCombatantID = 12,
            CombatantTargetingType = TargetingType.FRIENDLY
        };
        
        private AbilityEntity _validAbilityEntity;
        private AbilityEntity _enemyTriggerEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantCastingHandler = new CombatantCastingHandler(TriggerReaderMock.Object, AbilityEventSchedulerMock.Object, CombatantRepositoryMock.Object);
        }

        [SetUp]
        public void Setup()
        {
            _validAbilityEntity = TestAbilityEntityFactory.Create(FriendlyCombatantEntity.InstanceID, 12);
            _validAbilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_CASTING_COMPLETE, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 1, MaxTriggerValue = 5 });
            _validAbilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK - 1 });
            
            _enemyTriggerEntity = TestAbilityEntityFactory.Create(EnemyCombatantEntity.InstanceID, 94);
            _enemyTriggerEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_CASTING_COMPLETE, TargetingType = TargetingType.ENEMY, MinTriggerValue = 1, MaxTriggerValue = 5 });
            _enemyTriggerEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK });
        }
        
        [Test]
        public void Positive_Handle_FiltersEntities_NothingToSchedule()
        {
            SetupTriggerReader(TriggerEventType.COMBATANT_CASTING_COMPLETE, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantCastingHandler.Handle(TICK, _friendlyCombatantCastCompleteData);
        }

        [Test]
        public void Positive_Handle_MultipleCorrectEntity_FiltersEverythingElse()
        {
            AbilityEntity validAbility = TestAbilityEntityFactory.Create(EnemyCombatantEntity.InstanceID, 92);
            validAbility.ReplaceComponent(_validAbilityEntity.GetComponent<TriggerComponent>());
            validAbility.AddComponent(_validAbilityEntity.GetComponent<ReadyTickComponent>());
            
            SetupTriggerReader(TriggerEventType.COMBATANT_CASTING_COMPLETE, [SelfTargetingEntity, NotReadyEntity, _validAbilityEntity, _enemyTriggerEntity, validAbility]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantCastingHandler.Handle(TICK, _friendlyCombatantCastCompleteData);
            
            VerifyScheduleEvent(_validAbilityEntity.InstanceID, _validAbilityEntity.AbilityID);
            VerifyScheduleEvent(validAbility.InstanceID, validAbility.AbilityID);
        }

        [Test]
        public void Positive_Handle_CombatantIsDead_FiltersAbility()
        {
            FriendlyCombatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupTriggerReader(TriggerEventType.COMBATANT_CASTING_COMPLETE, [SelfTargetingEntity, NotReadyEntity, _validAbilityEntity, _enemyTriggerEntity]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantCastingHandler.Handle(TICK, _friendlyCombatantCastCompleteData);
        }
    }
}
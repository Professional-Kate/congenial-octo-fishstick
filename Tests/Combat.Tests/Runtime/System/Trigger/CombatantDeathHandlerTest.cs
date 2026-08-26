using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Handler;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    public sealed class CombatantDeathHandlerTest : BaseTriggerAbilityHandler
    {
        private CombatantDeathHandler _combatantDeathHandler;

        private readonly CombatantDeathData _friendlyDeathData = new() { CombatantTargetingType = TargetingType.FRIENDLY, DeadCombatantID = 12 };

        private CombatantAbilityEntity _validAbilityEntity;
        private CombatantAbilityEntity _enemyTriggerEntity;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantDeathHandler = new CombatantDeathHandler(TriggerReaderMock.Object, AbilityEventSchedulerMock.Object, CombatantRepositoryMock.Object);
        }
        
        [SetUp]
        public void Setup()
        {
            _validAbilityEntity = TestCombatantAbilityEntityFactory.Create(FriendlyCombatantEntity.CombatantID, 12);
            _validAbilityEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.FRIENDLY, MinTriggerValue = 0, MaxTriggerValue = 0 });
            _validAbilityEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK - 1 });
            
            _enemyTriggerEntity = TestCombatantAbilityEntityFactory.Create(EnemyCombatantEntity.CombatantID, 94);
            _enemyTriggerEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DEATH, TargetingType = TargetingType.ENEMY, MinTriggerValue = 1, MaxTriggerValue = 5 });
            _enemyTriggerEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK });
        }
        
        [Test]
        public void Positive_Handle_FiltersEntities_NothingToSchedule()
        {
            SetupTriggerReader(TriggerEventType.COMBATANT_DEATH, [SelfTargetingEntity, NotReadyEntity]);
            
            _combatantDeathHandler.Handle(TICK, _friendlyDeathData);
        }

        [Test]
        public void Positive_Handle_MultipleCorrectEntity_FiltersEverythingElse()
        {
            CombatantAbilityEntity validAbility = TestCombatantAbilityEntityFactory.Create(EnemyCombatantEntity.CombatantID, 92);
            validAbility.ReplaceComponent(_validAbilityEntity.GetComponent<TriggerComponent>());
            validAbility.AddComponent(_validAbilityEntity.GetComponent<ReadyTickComponent>());
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DEATH, [SelfTargetingEntity, NotReadyEntity, _validAbilityEntity, _enemyTriggerEntity, validAbility]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantDeathHandler.Handle(TICK, _friendlyDeathData);
            
            VerifyScheduleEvent(_validAbilityEntity.CombatantID, _validAbilityEntity.AbilityID);
            VerifyScheduleEvent(validAbility.CombatantID, validAbility.AbilityID);
        }

        [Test]
        public void Positive_Handle_CombatantIsDead_FiltersAbility()
        {
            FriendlyCombatantEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupTriggerReader(TriggerEventType.COMBATANT_DEATH, [SelfTargetingEntity, NotReadyEntity, _validAbilityEntity, _enemyTriggerEntity]);
            SetupGetCombatantEntity(FriendlyCombatantEntity, EnemyCombatantEntity);
            
            _combatantDeathHandler.Handle(TICK, _friendlyDeathData);
        }
    }
}
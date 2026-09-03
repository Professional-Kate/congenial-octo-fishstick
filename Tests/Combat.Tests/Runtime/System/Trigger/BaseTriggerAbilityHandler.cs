using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System.Trigger
{
    [TestFixture]
    public abstract class BaseTriggerAbilityHandler
    {
        protected readonly Mock<ITriggerReader> TriggerReaderMock = new();
        protected readonly Mock<IAbilityEventScheduler> AbilityEventSchedulerMock = new();
        protected readonly Mock<ICombatantRepository> CombatantRepositoryMock = new();

        protected const double TICK = 100d;

        protected CombatantEntity FriendlyCombatantEntity;
        protected CombatantEntity EnemyCombatantEntity;
        
        protected AbilityEntity NotReadyEntity;
        protected AbilityEntity SelfTargetingEntity;
            
        [SetUp]
        public void BaseSetup()
        {
            FriendlyCombatantEntity = TestCombatantEntityFactory.Create(12, TargetingType.FRIENDLY);
            EnemyCombatantEntity = TestCombatantEntityFactory.Create(21, TargetingType.FRIENDLY);
            
            NotReadyEntity = TestAbilityEntityFactory.Create(54, 26);
            NotReadyEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK + TICK });
            
            SelfTargetingEntity = TestAbilityEntityFactory.Create(29, 10);
            SelfTargetingEntity.ReplaceComponent(new TriggerComponent { TriggerEventType = TriggerEventType.COMBATANT_DAMAGED, TargetingType = TargetingType.SELF, MinTriggerValue = 0, MaxTriggerValue = 10 });
            SelfTargetingEntity.AddComponent(new ReadyTickComponent { ReadyTick = TICK - 1 });
            
            TriggerReaderMock.Reset();
            AbilityEventSchedulerMock.Reset();
            CombatantRepositoryMock.Reset();
        }

        [TearDown]
        public void BaseTearDown()
        {
            TriggerReaderMock.Verify();
            TriggerReaderMock.VerifyNoOtherCalls();
            AbilityEventSchedulerMock.Verify();
            AbilityEventSchedulerMock.VerifyNoOtherCalls();
            CombatantRepositoryMock.Verify();
            CombatantRepositoryMock.VerifyNoOtherCalls();
        }

        protected void SetupTriggerReader(TriggerEventType triggerEventType, ImmutableArray<AbilityEntity> abilityEntities)
        {
            TriggerReaderMock.Setup(library => library.GetAbilities(triggerEventType)).Returns(abilityEntities).Verifiable();
        }

        protected void SetupGetCombatantEntity(params CombatantEntity[] combatantEntities)
        {
            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                CombatantRepositoryMock.Setup(library => library.Get(combatantEntity.InstanceID)).Returns(combatantEntity).Verifiable();
            }
        }

        protected void VerifyScheduleEvent(byte combatantID, byte abilityID)
        {
            AbilityEventSchedulerMock.Verify(library => library.ScheduleEvent(TICK, abilityID, 0, combatantID), Times.Once);
        }

    }
}
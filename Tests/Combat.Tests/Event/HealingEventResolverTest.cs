using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class HealingEventResolverTest
    {
        private HealingEventResolver _healingEventResolver;
        private Mock<ICombatantRepository> _combatantRepositoryMock;
        private Mock<ICombatantAbilityEntityRepository> _combatantAbilityRepositoryMock;
        private Mock<ICombatantTargetFinder> _targetFinderMock;
        private Mock<IEntityHealingService> _entityHealingServiceMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;

        private CombatEvent _healingEvent;
        private const double COOLDOWN = 1d;

        private CombatantEntity _targetCombatant;
        private CombatantEntity _healingCombatant;
        private CombatantAbilityEntity _healingCombatantAbility;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            _combatantAbilityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _targetFinderMock = new Mock<ICombatantTargetFinder>();
            _entityHealingServiceMock = new Mock<IEntityHealingService>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            
            _healingEventResolver = new HealingEventResolver(_combatantRepositoryMock.Object, _combatantAbilityRepositoryMock.Object, _targetFinderMock.Object, _abilityEventSchedulerMock.Object, _entityHealingServiceMock.Object);
            
            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _healingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _healingCombatantAbility = TestCombatantAbilityEntityFactory.Create(_healingCombatant.CombatantID, AbilityType.MINOR_HEAL);
            _healingCombatantAbility.AddComponent(new CooldownComponent { Cooldown = COOLDOWN });
            _healingCombatantAbility.AddComponent(new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH });
            
            _healingEvent = new CombatEvent { CombatantID = _healingCombatant.CombatantID, Tick = 256, AbilityType = _healingCombatantAbility.AbilityType, EventType = EventType.HEALING };
        }
        
        [SetUp]
        public void Setup()
        {
            _targetFinderMock.Reset();
            _combatantAbilityRepositoryMock.Reset();
            _entityHealingServiceMock.Reset();
            _abilityEventSchedulerMock.Reset();
            _combatantRepositoryMock.Reset();
        }
        
        private void VerifyMocks()
        {
            _targetFinderMock.Verify();
            _targetFinderMock.VerifyNoOtherCalls();
            _combatantAbilityRepositoryMock.Verify();
            _combatantAbilityRepositoryMock.VerifyNoOtherCalls();
            _entityHealingServiceMock.Verify();
            _entityHealingServiceMock.VerifyNoOtherCalls();
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
        }
        
        private void SetupTargetFinder(CombatantEntity[] targets, TargetingPreference targetingPreference, CombatantStatType combatantStatType)
        {
            _targetFinderMock.Setup(library => library.SelectPreferredTargets(targetingPreference, combatantStatType, true, 1)).Returns(targets).Verifiable();
        }

        private void SetupAbilityEntityRepositoryGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _combatantAbilityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void VerifyHealingApplied(CombatantEntity[] targetCombatants, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingAbility, double tick)
        {
            _entityHealingServiceMock.Verify(library => library.ApplyHealing(targetCombatants, attackingCombatant, attackingAbility, tick), Times.Once);
        }

        private void VerifyEventEnqueued(double tick, AbilityType abilityType)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(tick, _healingEvent.CombatantID, abilityType), Times.Once);
        }

        private void SetupRepositoryGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        [Test]
        public void Positive_HandleEvent_CombatantNotAlive_DoesNothing()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.CreateCombatantEntity(25);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _healingEventResolver.ResolveEvent(_healingEvent.Tick, deadEntity.CombatantID, _healingEvent.AbilityType));
            
            VerifyMocks();
        }

        [Test]
        public void Positive_HandleEvent_HealsEntity_EnqueuesNewHealingEvent()
        {
            SetupTargetFinder([_targetCombatant], TargetingPreference.LOWEST, CombatantStatType.HEALTH);
            SetupAbilityEntityRepositoryGet(_healingCombatantAbility);
            SetupRepositoryGet(_healingCombatant);
            
            Assert.DoesNotThrow(() => _healingEventResolver.ResolveEvent(_healingEvent.Tick, _healingEvent.CombatantID, _healingEvent.AbilityType));
            
            VerifyHealingApplied([_targetCombatant], _healingCombatant, _healingCombatantAbility, _healingEvent.Tick);
            VerifyEventEnqueued(COOLDOWN + _healingEvent.Tick, _healingEvent.AbilityType);
            VerifyMocks();
        }
    }
}
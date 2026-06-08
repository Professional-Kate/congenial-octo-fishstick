using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Tests.TestFactory;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class DirectDamageEventResolverTest
    {
        private DirectDamageEventResolver _directDamageEventResolver;
        private Mock<ICombatantTargetFinder> _targetFinderMock;
        private Mock<ICombatantAbilityEntityRepository> _combatantEntityRepositoryMock;
        private Mock<IEntityDamageService> _damageMediatorMock;
        private Mock<IAbilityEventScheduler> _abilityEventSchedulerMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private CombatEvent _directDamageEvent;
        private const double COOLDOWN = 1d;

        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _targetFinderMock = new Mock<ICombatantTargetFinder>();
            _combatantEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _damageMediatorMock = new Mock<IEntityDamageService>();
            _abilityEventSchedulerMock = new Mock<IAbilityEventScheduler>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _directDamageEventResolver = new DirectDamageEventResolver(_combatantRepositoryMock.Object, _combatantEntityRepositoryMock.Object, _targetFinderMock.Object, _abilityEventSchedulerMock.Object, _damageMediatorMock.Object);

            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(_attackingCombatant.CombatantID, AbilityType.STAB);
            _attackingCombatantAbility.AddComponent(new CooldownComponent { Cooldown = COOLDOWN });
            _attackingCombatantAbility.AddComponent(new TargetingPreferenceComponent { TargetingPreference = TargetingPreference.LOWEST, CombatantStatType = CombatantStatType.HEALTH });
            
            _directDamageEvent = new CombatEvent { CombatantID = _attackingCombatant.CombatantID, Tick = 0, AbilityType = _attackingCombatantAbility.AbilityType, EventType = EventType.DIRECT_DAMAGE };
        }

        [SetUp]
        public void Setup()
        {
            _targetFinderMock.Reset();
            _combatantEntityRepositoryMock.Reset();
            _damageMediatorMock.Reset();
            _abilityEventSchedulerMock.Reset();
            _combatantRepositoryMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity target, TargetingPreference targetingPreference, CombatantStatType combatantStatType)
        {
            _targetFinderMock.Setup(library => library.SelectPreferredTargets(targetingPreference, combatantStatType, false, 1)).Returns([target]).Verifiable();
        }

        private void SetupAbilityEntityRepositoryGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _combatantEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void VerifyDamageApplied(CombatantEntity[] targetCombatants, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingAbility, double tick)
        {
            _damageMediatorMock.Verify(library => library.ApplyDamage(targetCombatants, attackingCombatant, attackingAbility, tick), Times.Once);
        }

        private void VerifyEventEnqueued(double tick, AbilityType abilityType)
        {
            _abilityEventSchedulerMock.Verify(library => library.ScheduleEvent(tick, _directDamageEvent.CombatantID, abilityType), Times.Once);
        }

        private void SetupRepositoryGet(CombatantEntity combatantEntity)
        {
            _combatantRepositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private void VerifyMocks()
        {
            _targetFinderMock.Verify();
            _targetFinderMock.VerifyNoOtherCalls();
            _combatantEntityRepositoryMock.Verify();
            _combatantEntityRepositoryMock.VerifyNoOtherCalls();
            _damageMediatorMock.Verify();
            _damageMediatorMock.VerifyNoOtherCalls();
            _abilityEventSchedulerMock.Verify();
            _abilityEventSchedulerMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage_EnqueuesAttack()
        {
            SetupTargetFinder(_targetCombatant, TargetingPreference.LOWEST, CombatantStatType.HEALTH);
            SetupAbilityEntityRepositoryGet(_attackingCombatantAbility);
            SetupRepositoryGet(_attackingCombatant);
            
            Assert.DoesNotThrow(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, _directDamageEvent.CombatantID, _directDamageEvent.AbilityType));

            VerifyDamageApplied([_targetCombatant], _attackingCombatant, _attackingCombatantAbility, _directDamageEvent.Tick);
            VerifyEventEnqueued(COOLDOWN, _directDamageEvent.AbilityType);
            VerifyMocks();
        }

        [Test]
        public void Positive_ResolveEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.CreateCombatantEntity(3);
            deadEntity.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, deadEntity.CombatantID, _directDamageEvent.AbilityType));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            _combatantRepositoryMock.Setup(library => library.Get(_attackingCombatant.CombatantID))
                .Throws(new NotFoundException<byte>(_attackingCombatant.CombatantID));
            
            Assert.Throws<NotFoundException<byte>>(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, _directDamageEvent.CombatantID, _directDamageEvent.AbilityType));
            
            _combatantRepositoryMock.Verify(library => library.Get(_attackingCombatant.CombatantID), Times.Once);
            VerifyMocks();
        }
    }
}
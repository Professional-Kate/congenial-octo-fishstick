using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event;
using IdelPog.Combat.Event.Resolver;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Event
{
    [TestFixture]
    public sealed class DirectDamageEventResolverTest
    {
        private DirectDamageEventResolver _directDamageEventResolver;
        private Mock<ITargetFinder> _targetFinderMock;
        private Mock< ICombatantAbilityEntityRepository> _combatantEntityRepositoryMock;
        private Mock<IEntityDamageMediator> _damageMediatorMock;
        private Mock<IBasicAttackScheduler> _attackSchedulerMock;
        private Mock<ICombatantRepository> _combatantRepositoryMock;

        private DirectDamageEvent _directDamageEvent;

        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _targetFinderMock = new Mock<ITargetFinder>();
            _combatantEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _damageMediatorMock = new Mock<IEntityDamageMediator>();
            _attackSchedulerMock = new Mock<IBasicAttackScheduler>();
            _combatantRepositoryMock = new Mock<ICombatantRepository>();
            
            _directDamageEventResolver = new DirectDamageEventResolver(_targetFinderMock.Object, _combatantEntityRepositoryMock.Object, _damageMediatorMock.Object, _attackSchedulerMock.Object, _combatantRepositoryMock.Object);

            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(_attackingCombatant.CombatantID, AbilityType.STRONG_ATTACK);
            
            _directDamageEvent = new DirectDamageEvent { AttackerID = _attackingCombatant.CombatantID, Tick = 0, AbilityType = _attackingCombatantAbility.AbilityType };
        }

        [SetUp]
        public void Setup()
        {
            _targetFinderMock.Reset();
            _combatantEntityRepositoryMock.Reset();
            _damageMediatorMock.Reset();
            _attackSchedulerMock.Reset();
            _combatantRepositoryMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity target, CombatantEntity attacker, CombatantAbilityEntity attackerAbility)
        {
            _targetFinderMock.Setup(library => library.FindBestTarget(attacker, attackerAbility.AbilityType)).Returns(target).Verifiable();
        }

        private void SetupAbilityEntityRepositoryGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _combatantEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void VerifyDamageApplied(CombatantEntity targetCombatant, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingAbility)
        {
            _damageMediatorMock.Verify(library => library.ApplyDamage(targetCombatant, attackingCombatant, attackingAbility), Times.Once);
        }

        private void VerifyEventEnqueued(AbilityType abilityType)
        {
            _attackSchedulerMock.Verify(library => library.EnqueueAttack(_directDamageEvent.Tick, _directDamageEvent.AttackerID, abilityType), Times.Once);
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
            _attackSchedulerMock.Verify();
            _attackSchedulerMock.VerifyNoOtherCalls();
            _combatantRepositoryMock.Verify();
            _combatantRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ResolveEvent_AppliesDamage_EnqueuesAttack()
        {
            SetupTargetFinder(_targetCombatant, _attackingCombatant, _attackingCombatantAbility);
            SetupAbilityEntityRepositoryGet(_attackingCombatantAbility);
            SetupRepositoryGet(_attackingCombatant);
            
            Assert.DoesNotThrow(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, _directDamageEvent.AttackerID, _directDamageEvent.AbilityType));

            VerifyDamageApplied(_targetCombatant, _attackingCombatant, _attackingCombatantAbility);
            VerifyEventEnqueued(_directDamageEvent.AbilityType);
            VerifyMocks();
        }

        [Test]
        public void Positive_ResolveEvent_CombatantNotAlive_Returns()
        {
            CombatantEntity deadEntity = TestCombatantEntityFactory.CreateCombatantEntity(3);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepositoryGet(deadEntity);
            
            Assert.DoesNotThrow(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, deadEntity.CombatantID, _directDamageEvent.AbilityType));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ResolveEvent_CombatantNotFound_Throws()
        {
            _combatantRepositoryMock.Setup(library => library.Get(_attackingCombatant.CombatantID))
                .Throws(new NotFoundException<byte>(_attackingCombatant.CombatantID));
            
            Assert.Throws<NotFoundException<byte>>(() => _directDamageEventResolver.ResolveEvent(_directDamageEvent.Tick, _directDamageEvent.AttackerID, _directDamageEvent.AbilityType));
            
            _combatantRepositoryMock.Verify(library => library.Get(_attackingCombatant.CombatantID), Times.Once);
            VerifyMocks();
        }
    }
}
using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class EntityDamageMediatorTest
    {
        private EntityDamageMediator _entityDamageMediator;
        private Mock<ICombatantRepository> _repositoryMock;
        private Mock<ITargetFinder> _targetFinderMock;
        private Mock<IDamageSystem> _damageSystemMock;
        private Mock<ICombatantAbilityEntityRepository> _abilityEntityRepositoryMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantStatsComponent _attackerStatsComponent;
        private CombatantAbilityEntity _attackingCombatantAbility;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            _damageSystemMock = new Mock<IDamageSystem>();
            _abilityEntityRepositoryMock = new Mock<ICombatantAbilityEntityRepository>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            
            _entityDamageMediator = new EntityDamageMediator(_repositoryMock.Object, _targetFinderMock.Object, _damageSystemMock.Object, _abilityEntityRepositoryMock.Object, _deathSystemMock.Object, _combatantStoreServiceMock.Object, _combatantLoggerMock.Object, new CombatantAssertion());
            _attackingCombatantAbility = TestCombatantAbilityEntityFactory.Create(2, AbilityType.BASIC_ATTACK);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = TestCombatantEntityFactory.CreateCombatantEntity(1);
            
            _attackingCombatant = TestCombatantEntityFactory.CreateCombatantEntity(2);
            _attackerStatsComponent = _attackingCombatant.GetComponent<CombatantStatsComponent>();
            
            _repositoryMock.Reset();
            _targetFinderMock.Reset();
            _combatantStoreServiceMock.Reset();
            _damageSystemMock.Reset();
            _deathSystemMock.Reset();
            _abilityEntityRepositoryMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity attacker, CombatantEntity target)
        {
            _targetFinderMock.Setup(library => library.FindBestTarget(attacker, AbilityType.BASIC_ATTACK)).Returns(target).Verifiable();
        }

        private void SetupRepository(CombatantEntity combatantEntity)
        {
            _repositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private void SetupDamageSystem(CombatantEntity targetCombatant, CombatantStatsComponent attackerStats, uint newHealth, CombatantAbilityEntity combatantAbilityEntity)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, attackerStats.Attack, combatantAbilityEntity)).Returns(newHealth).Verifiable();
        }

        private void SetupAbilityEntityRepositoryGet(CombatantAbilityEntity combatantAbilityEntity)
        {
            _abilityEntityRepositoryMock.Setup(library => library.Get(combatantAbilityEntity.CombatantID, combatantAbilityEntity.AbilityType)).Returns(combatantAbilityEntity).Verifiable();
        }

        private void VerifyStoreRegisterCombatantChange(CombatantEntity combatantEntity, Times times)
        {
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantChange(combatantEntity), times);
        }

        private void VerifyGetCalculatedDamage(uint attackingCombatantAttack, CombatantAbilityEntity attackingAbility)
        {
            _damageSystemMock.Verify(library => library.GetCalculatedDamage(attackingCombatantAttack, attackingAbility), Times.Once);
        }
        
        private void VerifyMocks()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            
            _targetFinderMock.Verify();
            _targetFinderMock.VerifyNoOtherCalls();
            
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
            
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
            
            _deathSystemMock.Verify();
            _deathSystemMock.VerifyNoOtherCalls();
            
            _abilityEntityRepositoryMock.Verify();
            _abilityEntityRepositoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 1, _attackingCombatantAbility);
            SetupTargetFinder(_attackingCombatant, _targetCombatant);
            SetupRepository(_attackingCombatant);
            SetupAbilityEntityRepositoryGet(_attackingCombatantAbility);
            
            _entityDamageMediator.ApplyDamage(_attackingCombatant.CombatantID, AbilityType.BASIC_ATTACK);

            VerifyStoreRegisterCombatantChange(_targetCombatant, Times.Once());
            VerifyGetCalculatedDamage(_attackerStatsComponent.Attack, _attackingCombatantAbility);
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 0, _attackingCombatantAbility);
            SetupTargetFinder(_attackingCombatant, _targetCombatant);
            SetupRepository(_attackingCombatant);
            SetupAbilityEntityRepositoryGet(_attackingCombatantAbility);
            
            _entityDamageMediator.ApplyDamage(_attackingCombatant.CombatantID, AbilityType.BASIC_ATTACK);
            
            _deathSystemMock.Verify(library => library.KillEntity(_targetCombatant), Times.Once);
            VerifyGetCalculatedDamage(_attackerStatsComponent.Attack, _attackingCombatantAbility);
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            _repositoryMock.Setup(library => library.Get(_targetCombatant.CombatantID))
                .Throws(new NotFoundException<byte>(_targetCombatant.CombatantID)).Verifiable();
            
            Assert.Throws<NotFoundException<byte>>(() => _entityDamageMediator.ApplyDamage(_targetCombatant.CombatantID, AbilityType.BASIC_ATTACK));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_AttackingCombatantNotAlive_Throws()
        { 
            CombatantEntity deadEntity = TestCombatantEntityFactory.CreateCombatantEntity(1);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepository(deadEntity);
            
            CombatantDeadException exception = Assert.Throws<CombatantDeadException>(() => _entityDamageMediator.ApplyDamage(deadEntity.CombatantID, AbilityType.BASIC_ATTACK));
            
            Assert.That(exception.CombatantID, Is.EqualTo(deadEntity.CombatantID));
            VerifyMocks();
        }
    }
}
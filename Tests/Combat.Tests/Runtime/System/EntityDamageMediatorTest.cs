using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Validation.Assertion;
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
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<IDeathSystem> _deathSystemMock;
        private Mock<ICombatantLogger> _combatantLoggerMock;
        
        private CombatantEntity _targetCombatant;
        private CombatantEntity _attackingCombatant;
        private CombatantStatsComponent _attackerStatsComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            _damageSystemMock = new Mock<IDamageSystem>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _deathSystemMock = new Mock<IDeathSystem>();
            _combatantLoggerMock = new Mock<ICombatantLogger>();
            
            _entityDamageMediator = new EntityDamageMediator(_repositoryMock.Object, _targetFinderMock.Object, _damageSystemMock.Object, _deathSystemMock.Object, _combatantStoreServiceMock.Object, new FoundAssertion(), new CombatantAssertion(), new NumberAssertion(), _combatantLoggerMock.Object);
        }

        [SetUp]
        public void Setup()
        { 
            _targetCombatant = CombatantEntityFactory.CreateCombatantEntity(1);

            _attackingCombatant = CombatantEntityFactory.CreateCombatantEntity(2);
            _attackerStatsComponent = _attackingCombatant.GetComponent<CombatantStatsComponent>();
            
            _repositoryMock.Reset();
            _targetFinderMock.Reset();
            _combatantStoreServiceMock.Reset();
            _damageSystemMock.Reset();
            _deathSystemMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity attacker, CombatantEntity target)
        {
            _targetFinderMock.Setup(library => library.FindBestTarget(attacker, SkillType.BASIC_ATTACK)).Returns(target).Verifiable();
        }

        private void SetupRepository(CombatantEntity combatantEntity)
        {
            _repositoryMock.Setup(library => library.Contains(combatantEntity.CombatantID)).Returns(true).Verifiable();
            _repositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
        }

        private void SetupDamageSystem(CombatantEntity targetCombatant, CombatantStatsComponent attackerStats, uint newHealth)
        {
            _damageSystemMock.Setup(library => library.DealDamage(targetCombatant, attackerStats.Attack)).Returns(newHealth).Verifiable();
        }

        private void VerifyStoreRegisterCombatantChange(CombatantEntity combatantEntity, Times times)
        {
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantChange(combatantEntity), times);
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
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 1);
            SetupTargetFinder(_attackingCombatant, _targetCombatant);
            SetupRepository(_attackingCombatant);
            
            _entityDamageMediator.ApplyDamage(_attackingCombatant.CombatantID, SkillType.BASIC_ATTACK);
            
            VerifyStoreRegisterCombatantChange(_targetCombatant, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath()
        {
            SetupDamageSystem(_targetCombatant, _attackerStatsComponent, 0);
            SetupTargetFinder(_attackingCombatant, _targetCombatant);
            SetupRepository(_attackingCombatant);
            
            _entityDamageMediator.ApplyDamage(_attackingCombatant.CombatantID, SkillType.BASIC_ATTACK);
            
            _deathSystemMock.Verify(library => library.KillEntity(_targetCombatant), Times.Once);
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _entityDamageMediator.ApplyDamage(_targetCombatant.CombatantID, SkillType.BASIC_ATTACK));
            
            _repositoryMock.Verify(library => library.Contains(_targetCombatant.CombatantID), Times.Once);
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            StatCard zeroAttackCard = new() { Attack = 0, Health = 10, Speed = 10 };
            CombatantEntity zeroAttackEntity = CombatantEntityFactory.CreateCombatantEntity(1, true, zeroAttackCard);
            
            SetupRepository(zeroAttackEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _entityDamageMediator.ApplyDamage(zeroAttackEntity.CombatantID, SkillType.BASIC_ATTACK));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackEntity.GetComponent<CombatantStatsComponent>().ToString()));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_AttackingCombatantNotAlive_Throws()
        { 
            CombatantEntity deadEntity = CombatantEntityFactory.CreateCombatantEntity(1);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepository(deadEntity);
            
            CombatantDeadException exception = Assert.Throws<CombatantDeadException>(() => _entityDamageMediator.ApplyDamage(deadEntity.CombatantID, SkillType.BASIC_ATTACK));
            
            Assert.That(exception.CombatantID, Is.EqualTo(deadEntity.CombatantID));
            VerifyMocks();
        }
    }
}
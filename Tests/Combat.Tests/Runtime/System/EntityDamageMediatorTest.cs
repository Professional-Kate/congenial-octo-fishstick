using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
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
        private Mock<ICombatLog> _combatLogMock;
        private Mock<ICombatStateService> _combatantStateServiceMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        
        private CombatantEntity _combatantEntity;
        private CombatantStatsComponent _combatantStatsComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            _damageSystemMock = new Mock<IDamageSystem>();
            _combatLogMock = new Mock<ICombatLog>();
            _combatantStateServiceMock = new Mock<ICombatStateService>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            
            _entityDamageMediator = new EntityDamageMediator(_repositoryMock.Object, _targetFinderMock.Object, _damageSystemMock.Object, _combatLogMock.Object, _combatantStateServiceMock.Object, _combatantStoreServiceMock.Object, new FoundAssertion(), new CombatantAssertion(), new NumberAssertion());
        }

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = CombatantEntityFactory.CreateCombatantEntity(1);
            _combatantStatsComponent = _combatantEntity.GetComponent<CombatantStatsComponent>();
            
            _repositoryMock.Reset();
            _targetFinderMock.Reset();
            _combatantStateServiceMock.Reset();
            _combatantStoreServiceMock.Reset();
            _damageSystemMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity combatantEntity)
        {
            _targetFinderMock.Setup(library => library.FindBestTarget(combatantEntity)).Returns(combatantEntity).Verifiable();
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

        private void SetupIsCombatOver(bool isCombatOver)
        {
            _combatantStateServiceMock.Setup(library => library.IsCombatOver).Returns(isCombatOver).Verifiable();
        }

        private void VerifyStoreRegisterCombatantChange(CombatantEntity combatantEntity, Times times)
        {
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantChange(combatantEntity), times);
        }

        private void VerifyStateServiceEvaluate(CombatantEntity combatantEntity)
        {
            _combatantStateServiceMock.Verify(library => library.Evaluate(combatantEntity), Times.Once);
        }
        
        private void VerifyStateServiceIsCombatOver()
        {
            _combatantStateServiceMock.Verify(library => library.IsCombatOver, Times.Once);
        }

        private void VerifyMocks()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
            
            _targetFinderMock.Verify();
            _targetFinderMock.VerifyNoOtherCalls();
            
            _combatantStateServiceMock.Verify();
            _combatantStateServiceMock.VerifyNoOtherCalls();
            
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
            
            _damageSystemMock.Verify();
            _damageSystemMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupDamageSystem(_combatantEntity, _combatantStatsComponent, 1);
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageMediator.ApplyDamage(_combatantEntity.CombatantID);
            
            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath_CombatNotOver()
        {
            SetupIsCombatOver(false);
            SetupDamageSystem(_combatantEntity, _combatantStatsComponent, 0);
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageMediator.ApplyDamage(_combatantEntity.CombatantID);
            
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantDeath(_combatantEntity), Times.Once);
            VerifyStateServiceEvaluate(_combatantEntity);
            VerifyStateServiceIsCombatOver();
            VerifyMocks();
        }
        
        [Test]
        public void Positive_ApplyDamage_CausesDeath_CombatIsOver()
        {
            SetupIsCombatOver(true);
            SetupDamageSystem(_combatantEntity, _combatantStatsComponent, 0);
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageMediator.ApplyDamage(_combatantEntity.CombatantID);
            
            VerifyStateServiceEvaluate(_combatantEntity);
            VerifyStateServiceIsCombatOver();
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _entityDamageMediator.ApplyDamage(_combatantEntity.CombatantID));
            
            _repositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once);
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            StatCard zeroAttackCard = new() { Attack = 0, Health = 10, Speed = 10 };
            CombatantEntity zeroAttackEntity = CombatantEntityFactory.CreateCombatantEntity(1, zeroAttackCard);
            
            SetupRepository(zeroAttackEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _entityDamageMediator.ApplyDamage(zeroAttackEntity.CombatantID));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackEntity.GetComponent<CombatantStatsComponent>().ToString()));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_AttackingCombatantNotAlive_Throws()
        { 
            CombatantEntity deadEntity = CombatantEntityFactory.CreateCombatantEntity(1);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepository(deadEntity);
            
            CombatantDeadException exception = Assert.Throws<CombatantDeadException>(() => _entityDamageMediator.ApplyDamage(deadEntity.CombatantID));
            
            Assert.That(exception.CombatantID, Is.EqualTo(deadEntity.CombatantID));
            VerifyMocks();
        }
    }
}
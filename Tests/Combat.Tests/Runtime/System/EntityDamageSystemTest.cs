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
    public sealed class EntityDamageSystemTest
    {
        private EntityDamageSystem _entityDamageSystem;
        private Mock<ICombatantRepository> _repositoryMock;
        private Mock<ITargetFinder> _targetFinderMock;
        private Mock<ICombatStateService> _combatantStateServiceMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private Mock<ICombatLog> _combatLogMock;
        
        private CombatantEntity _combatantEntity;
        private StatCard _statCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            _combatantStateServiceMock = new Mock<ICombatStateService>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            _combatLogMock = new Mock<ICombatLog>();
            
            _entityDamageSystem = new EntityDamageSystem(_repositoryMock.Object, _targetFinderMock.Object, _combatantStateServiceMock.Object, _combatantStoreServiceMock.Object, new FoundAssertion(), new NumberAssertion(), _combatLogMock.Object, new CombatantAssertion());

            _statCard = new StatCard { Health = 11, Attack = 5, Speed = 3 };
        }

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = CombatantEntityFactory.CreateCombatantEntity(1, _statCard);
            _repositoryMock.Reset();
            _targetFinderMock.Reset();
            _combatantStateServiceMock.Reset();
            _combatantStoreServiceMock.Reset();
        }

        private void SetupTargetFinder(CombatantEntity combatantEntity)
        {
            _targetFinderMock.Setup(library => library.FindBestTarget(combatantEntity)).Returns(combatantEntity).Verifiable();
        }
        
        private CombatantStatsComponent GetComponent()
        { 
            return _combatantEntity.GetComponent<CombatantStatsComponent>();
        }

        private static void VerifyComponent(StatCard expectedStatCard, CombatantStatsComponent component)
        {
            Assert.That(component.StatCard, Is.EqualTo(expectedStatCard));
        }

        private void SetupRepository(CombatantEntity combatantEntity)
        {
            _repositoryMock.Setup(library => library.Contains(combatantEntity.CombatantID)).Returns(true).Verifiable();
            _repositoryMock.Setup(library => library.Get(combatantEntity.CombatantID)).Returns(combatantEntity).Verifiable();
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
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());

            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Once());
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_MultipleTimes_ReducesHealth()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 1 }, GetComponent());

            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Exactly(2));
            VerifyMocks();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath_SignalsDeath_EndsCombat()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 1 }, GetComponent());
            
            _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 0 }, GetComponent());

            
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantDeath(_combatantEntity), Times.Once);
            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Exactly(2));
            VerifyStateServiceEvaluate(_combatantEntity);
            VerifyStateServiceIsCombatOver();
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _entityDamageSystem.ApplyDamage(_combatantEntity.CombatantID));
            
            _repositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once);
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            StatCard zeroAttackCard = _statCard with { Attack = 0 };
            CombatantEntity zeroAttackEntity = CombatantEntityFactory.CreateCombatantEntity(0, zeroAttackCard);
            
            SetupRepository(zeroAttackEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _entityDamageSystem.ApplyDamage(zeroAttackEntity.CombatantID));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackCard.ToString()));
            
            VerifyMocks();
        }

        [Test]
        public void Negative_ApplyDamage_AttackingCombatantNotAlive_Throws()
        { 
            CombatantEntity deadEntity = CombatantEntityFactory.CreateCombatantEntity(1);
            deadEntity.UpdateLifeStatus(false);
            
            SetupRepository(deadEntity);
            
            Assert.Throws<Exception>(() => _entityDamageSystem.ApplyDamage(deadEntity.CombatantID));
            
            VerifyMocks();
        }
    }
}
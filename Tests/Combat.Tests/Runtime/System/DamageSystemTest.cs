using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        private Mock<ICombatantRepository> _repositoryMock;
        private Mock<ITargetFinder> _targetFinderMock;
        private Mock<ICombatStateService> _combatantStateServiceMock;
        private Mock<ICombatantStoreService> _combatantStoreServiceMock;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _combatantEntity;
        private StatCard _statCard;
        private CombatantCard _combatantCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            _combatantStateServiceMock = new Mock<ICombatStateService>();
            _combatantStoreServiceMock = new Mock<ICombatantStoreService>();
            
            _damageSystem = new DamageSystem(_repositoryMock.Object, _targetFinderMock.Object, _combatantStateServiceMock.Object, _combatantStoreServiceMock.Object, new FoundAssertion(), new NumberAssertion());

            _statCard = new StatCard { Health = 11, Attack = 5, Speed = 3 };
            _combatantCard = new CombatantCard { StatCard = _statCard, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.GOBLIN };
        }

        [SetUp]
        public void Setup()
        { 
            _combatantEntity = new CombatantEntity(_repositoryAsserter, _combatantCard) { CombatantID = 1 };
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

        private void VerifyStateServiceEvaluate()
        {
            _combatantStateServiceMock.Verify(library => library.Evaluate(), Times.Once);
        }
        
        private void VerifyStateServiceIsCombatOver()
        {
            _combatantStateServiceMock.Verify(library => library.IsCombatOver, Times.Once);
        }

        private void VerifyRepository()
        {
            _repositoryMock.Verify();
            _repositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyTargetFinder()
        {
            _targetFinderMock.Verify();
            _targetFinderMock.VerifyNoOtherCalls();
        }

        private void VerifyStateService()
        {
            _combatantStateServiceMock.Verify();
            _combatantStateServiceMock.VerifyNoOtherCalls();
        }

        private void VerifyCombatantStoreService()
        {
            _combatantStoreServiceMock.Verify();
            _combatantStoreServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());

            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Once());
            VerifyRepository();
            VerifyTargetFinder();
            VerifyStateService();
            VerifyCombatantStoreService();
        }

        [Test]
        public void Positive_ApplyDamage_MultipleTimes_ReducesHealth()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 1 }, GetComponent());

            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Exactly(2));
            VerifyRepository();
            VerifyTargetFinder();
            VerifyStateService();
            VerifyCombatantStoreService();
        }

        [Test]
        public void Positive_ApplyDamage_CausesDeath_SignalsDeath_EndsCombat()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 6 }, GetComponent());
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 1 }, GetComponent());
            
            _damageSystem.ApplyDamage(_combatantEntity.CombatantID);
            VerifyComponent(_statCard with { Health = 0 }, GetComponent());

            
            _combatantStoreServiceMock.Verify(library => library.RegisterCombatantDeath(_combatantEntity), Times.Once);
            VerifyStoreRegisterCombatantChange(_combatantEntity, Times.Exactly(2));
            VerifyStateServiceEvaluate();
            VerifyStateServiceIsCombatOver();
            VerifyRepository();
            VerifyTargetFinder();
            VerifyStateService();
            VerifyCombatantStoreService();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _damageSystem.ApplyDamage(_combatantEntity.CombatantID));
            
            _repositoryMock.Verify(library => library.Contains(_combatantEntity.CombatantID), Times.Once);
            VerifyRepository();
            VerifyTargetFinder();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            CombatantCard zeroAttackCard = new() { StatCard = _statCard with { Attack = 0 }, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.GOBLIN };
            CombatantEntity zeroAttackEntity = new(_repositoryAsserter, zeroAttackCard) { CombatantID = 2 };
            
            SetupRepository(zeroAttackEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _damageSystem.ApplyDamage(zeroAttackEntity.CombatantID));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackCard.StatCard.ToString()));
            
            VerifyRepository();
            VerifyTargetFinder();
        }
    }
}
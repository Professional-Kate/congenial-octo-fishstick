using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
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
        private RepositoryAsserter _repositoryAsserter;
        private Mock<ITargetFinder> _targetFinderMock;
        
        private CombatantEntity _combatantEntity;
        private StatCard _statCard;
        private CombatantCard _combatantCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _repositoryMock = new Mock<ICombatantRepository>();
            _targetFinderMock = new Mock<ITargetFinder>();
            
            _damageSystem = new DamageSystem(_repositoryMock.Object, new FoundAssertion(), new NumberAssertion(), _targetFinderMock.Object);

            _statCard = new StatCard { Health = 10, Attack = 5, Speed = 3 };
            _combatantCard = new CombatantCard { StatCard = _statCard, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.GOBLIN };
        }

        [SetUp]
        public void SetUp()
        { 
            _combatantEntity = new CombatantEntity(_repositoryAsserter, _combatantCard) { CombatantID = 0 };
            _repositoryMock.Reset();
            _targetFinderMock.Reset();
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
            _repositoryMock.Setup(library => library.Contains(0)).Returns(true).Verifiable();
            _repositoryMock.Setup(library => library.Get(0)).Returns(combatantEntity).Verifiable();
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

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(0);
            
            VerifyComponent(_statCard with { Health = 5 }, GetComponent());

            VerifyRepository();
            VerifyTargetFinder();
        }

        [Test]
        public void Positive_ApplyDamage_MultipleTimes_ReducesHealth()
        {
            SetupTargetFinder(_combatantEntity);
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(0);
            VerifyComponent(_statCard with { Health = 5 }, GetComponent());

            _damageSystem.ApplyDamage(0);
            VerifyComponent(_statCard with { Health = 0 }, GetComponent());
            
            VerifyRepository();
            VerifyTargetFinder();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _damageSystem.ApplyDamage(0));
            
            _repositoryMock.Verify(library => library.Contains(0), Times.Once);
            VerifyRepository();
            VerifyTargetFinder();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            CombatantCard zeroAttackCard = new() { StatCard = _statCard with { Attack = 0 }, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.GOBLIN };
            CombatantEntity zeroAttackEntity = new(_repositoryAsserter, zeroAttackCard) { CombatantID = 0 };
            
            SetupRepository(zeroAttackEntity);
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _damageSystem.ApplyDamage(0));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackCard.StatCard.ToString()));
            
            VerifyRepository();
            VerifyTargetFinder();
        }
    }
}
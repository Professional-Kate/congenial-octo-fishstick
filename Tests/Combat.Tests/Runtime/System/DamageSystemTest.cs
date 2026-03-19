using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        private Mock<IAssetRepository<byte, CombatantEntity>> _repositoryMock;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _combatantEntity;
        private StatCard _statCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            _repositoryMock = new Mock<IAssetRepository<byte, CombatantEntity>>();
            
            _damageSystem = new DamageSystem(_repositoryMock.Object, new FoundAssertion(), new NumberAssertion());

            _statCard = new StatCard { Health = 10, Attack = 5, Speed = 3 };
        }

        [SetUp]
        public void SetUp()
        { 
            _combatantEntity = new CombatantEntity(_repositoryAsserter, _statCard) { IsFriendly = true, CombatantID = 0 };
            _repositoryMock.Reset();
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

        [Test]
        public void Positive_ApplyDamage_RemovesHealthFromTarget()
        {
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(0);
            
            VerifyComponent(_statCard with { Health = 5 }, GetComponent());

            VerifyRepository();
        }

        [Test]
        public void Positive_ApplyDamage_MultipleTimes_ReducesHealth()
        {
            SetupRepository(_combatantEntity);
            
            _damageSystem.ApplyDamage(0);
            VerifyComponent(_statCard with { Health = 5 }, GetComponent());

            _damageSystem.ApplyDamage(0);
            VerifyComponent(_statCard with { Health = 0 }, GetComponent());
            
            VerifyRepository();
        }

        [Test]
        public void Negative_ApplyDamage_InstanceIDUnknown_Throws()
        {
            Assert.Throws<NotFoundException<byte>>(() => _damageSystem.ApplyDamage(0));
            
            _repositoryMock.Verify(library => library.Contains(0), Times.Once);
            VerifyRepository();
        }

        [Test]
        public void Negative_ApplyDamage_ZeroAttack_Throws()
        {
            SetupRepository(_combatantEntity);
            StatCard zeroAttackCard = _statCard with { Attack = 0 };
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _damageSystem.ApplyDamage(0));
            
            Assert.That(exception.Source, Is.EqualTo(zeroAttackCard.ToString()));
            
            VerifyRepository();
        }
    }
}
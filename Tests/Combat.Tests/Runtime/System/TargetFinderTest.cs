using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using Moq;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class TargetFinderTest
    {
        private TargetFinder _targetFinder;
        private Mock<ICombatantFilters> _combatantFiltersMock;
        private Mock<ICombatantStore> _combatantStoreMock;
        private RepositoryAsserter _repositoryAsserter;

        private CombatantEntity _friendlyEntity;
        private CombatantEntity _enemyEntity;

        private StatCard _friendlyStats;
        private StatCard _enemyStats;
        private CombatantCard _friendlyCard;
        private CombatantCard _enemyCard;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            _combatantStoreMock = new Mock<ICombatantStore>();
            
            _targetFinder = new TargetFinder(_combatantFiltersMock.Object, new Random(1),  _combatantStoreMock.Object);

            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _friendlyStats = new StatCard { Health = 10, Attack = 5, Speed = 10 };
            _friendlyCard = new CombatantCard { StatCard = _friendlyStats, TargetingType = TargetingType.LOW_HEALTH, IsFriendly = true, CombatantType = CombatantType.BEAR };
            
            _enemyStats = new StatCard { Health = 10, Attack = 5, Speed = 10 };
            _enemyCard = new CombatantCard { StatCard = _enemyStats, TargetingType = TargetingType.LOW_HEALTH, IsFriendly = false, CombatantType = CombatantType.GOBLIN };
            
            _friendlyEntity = new CombatantEntity(_repositoryAsserter, _friendlyCard) { CombatantID = 0 };
            _enemyEntity = new CombatantEntity(_repositoryAsserter, _enemyCard) { CombatantID = 1 };
        }

        private void SetupReturnEnemies(params CombatantEntity[] enemies)
        {
            _combatantFiltersMock.Setup(library => library.GetEnemies()).Returns(enemies).Verifiable();
        }
        
        private void SetupReturnFriendlies(params CombatantEntity[] friends)
        {
            _combatantFiltersMock.Setup(library => library.GetFriendlies()).Returns(friends).Verifiable();
        }

        [Test]
        public void Positive_FindBestTarget_FriendlyAttack_ReturnsOnlyTarget()
        {
            SetupReturnEnemies(_enemyEntity);
            
            CombatantEntity target = _targetFinder.FindBestTarget(_friendlyEntity);
            
            Assert.That(target, Is.EqualTo(_enemyEntity));
        }

        [Test]
        public void Positive_FindBestTarget_FriendlyAttack_ReturnsKillableTarget()
        {
            CombatantEntity lowHealthEnemy = new(_repositoryAsserter, _enemyCard with { StatCard = _enemyStats with { Health = 5 }}) { CombatantID = 10 };
            
            SetupReturnEnemies(_enemyEntity, lowHealthEnemy);
            
            CombatantEntity target = _targetFinder.FindBestTarget(_friendlyEntity);
            
            Assert.That(target, Is.EqualTo(lowHealthEnemy));
        }
    }
}
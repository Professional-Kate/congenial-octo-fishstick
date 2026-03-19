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
        private RepositoryAsserter _repositoryAsserter;

        private CombatantEntity _friendlyEntity;
        private CombatantEntity _enemyEntity;

        private StatCard _friendlyStats;
        private StatCard _enemyStats;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantFiltersMock = new Mock<ICombatantFilters>();
            
            _targetFinder = new TargetFinder(_combatantFiltersMock.Object, new Random(1));

            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _friendlyStats = new StatCard { Health = 10, Attack = 5, Speed = 10 };
            _enemyStats = new StatCard { Health = 10, Attack = 5, Speed = 10 };
            
            _friendlyEntity = new CombatantEntity(_repositoryAsserter, _friendlyStats) { IsFriendly = true, CombatantID = 0 };
            _enemyEntity = new CombatantEntity(_repositoryAsserter, _enemyStats) { IsFriendly = false, CombatantID = 1 };
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
            CombatantEntity lowHealthEnemy = new(_repositoryAsserter, _enemyStats with { Health = 5 }) { IsFriendly = false, CombatantID = 10 };
            
            SetupReturnEnemies(_enemyEntity, lowHealthEnemy);
            
            CombatantEntity target = _targetFinder.FindBestTarget(_friendlyEntity);
            
            Assert.That(target, Is.EqualTo(lowHealthEnemy));
        }
    }
}
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class LowestHealthFilterTest
    {
        private LowestHealthFilter _lowestHealthFilter;
        private RepositoryAsserter _repositoryAsserter;

        private CombatantEntity _highHealthEntity;
        private CombatantEntity _lowHealthEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lowestHealthFilter = new LowestHealthFilter(new CollectionAssertion());
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _highHealthEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 5, Health = 10, Speed = 5 }) { IsFriendly = true, CombatantID = 0 };
            _lowHealthEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 5, Health = 5, Speed = 5 }) { IsFriendly = true, CombatantID = 15 };
        }

        [Test]
        public void Positive_GetEntity_FindsLowestHealthEntity()
        {
            byte combatantID = _lowestHealthFilter.GetEntity([_highHealthEntity, _lowHealthEntity, _highHealthEntity, _highHealthEntity]);
            
            Assert.That(combatantID, Is.EqualTo(_lowHealthEntity.CombatantID));
        }

        [Test]
        public void Positive_GetEntity_OneHP_EarlyReturn()
        {
            CombatantEntity oneHealth = new(_repositoryAsserter, new StatCard { Attack = 5, Health = 1, Speed = 5 }) { IsFriendly = true, CombatantID = 12 };
            
            byte combatantID = _lowestHealthFilter.GetEntity([_lowHealthEntity, oneHealth, _lowHealthEntity, _highHealthEntity]);
            
            Assert.That(combatantID, Is.EqualTo(oneHealth.CombatantID));
        }

        [Test]
        public void Negative_GetEntity_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _lowestHealthFilter.GetEntity([]));
        }
    }
}
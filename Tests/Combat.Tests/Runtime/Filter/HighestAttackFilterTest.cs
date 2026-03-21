using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class HighestAttackFilterTest
    {
        private HighestAttackFilter _highestAttackFilter;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _highAttackEntity;
        private CombatantEntity _lowAttackEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _highestAttackFilter = new HighestAttackFilter(new CollectionAssertion());
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _highAttackEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 8, Health = 7, Speed = 5 }) { IsFriendly = true, CombatantID = 12 };
            _lowAttackEntity = new CombatantEntity(_repositoryAsserter, new StatCard { Attack = 2, Health = 6, Speed = 5 }) { IsFriendly = true, CombatantID = 27 };
        }
        
        [Test]
        public void Positive_GetEntity_FindsHighestAttackEntity()
        {
            CombatantEntity combatant = _highestAttackFilter.GetEntity([_lowAttackEntity, _lowAttackEntity, _highAttackEntity, _lowAttackEntity]);
            
            Assert.That(combatant, Is.EqualTo(_highAttackEntity));
        }
        
        [Test]
        public void Positive_GetEntity_MaxAttack_ReturnsExpected()
        {
            CombatantEntity maxAttack = new(_repositoryAsserter, new StatCard { Attack = uint.MaxValue, Health = 1, Speed = 5 }) { IsFriendly = true, CombatantID = 25 };
            
            CombatantEntity combatantID = _highestAttackFilter.GetEntity([_highAttackEntity, maxAttack, _highAttackEntity, _lowAttackEntity]);
            
            Assert.That(combatantID, Is.EqualTo(maxAttack));
        }

        [Test]
        public void Negative_GetEntity_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _highestAttackFilter.GetEntity([]));
        }
    }
}
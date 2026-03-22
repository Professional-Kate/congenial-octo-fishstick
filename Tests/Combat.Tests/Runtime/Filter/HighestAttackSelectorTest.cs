using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class HighestAttackSelectorTest
    {
        private HighestAttackSelector _highestAttackSelector;
        private RepositoryAsserter _repositoryAsserter;
        
        private CombatantEntity _highAttackEntity;
        private CombatantEntity _lowAttackEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _highestAttackSelector = new HighestAttackSelector(new CollectionAssertion());
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());

            _highAttackEntity = new CombatantEntity(_repositoryAsserter, new CombatantCard { StatCard = new StatCard { Attack = 8, Health = 7, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.HUMAN }) { CombatantID = 12, IsAlive = true };
            _lowAttackEntity = new CombatantEntity(_repositoryAsserter, new CombatantCard { StatCard = new StatCard { Attack = 2, Health = 6, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.HUMAN }) { CombatantID = 27, IsAlive = true };
        }
        
        [Test]
        public void Positive_GetEntity_FindsHighestAttackEntity()
        {
            CombatantEntity combatant = _highestAttackSelector.GetEntity([_lowAttackEntity, _lowAttackEntity, _highAttackEntity, _lowAttackEntity]);
            
            Assert.That(combatant, Is.EqualTo(_highAttackEntity));
        }
        
        [Test]
        public void Positive_GetEntity_MaxAttack_ReturnsExpected()
        {
            CombatantEntity maxAttack = new(_repositoryAsserter, new CombatantCard { StatCard = new StatCard { Attack = uint.MaxValue, Health = 1, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, IsFriendly = true, CombatantType = CombatantType.HUMAN }) { CombatantID = 25, IsAlive = true };
            
            CombatantEntity combatantID = _highestAttackSelector.GetEntity([_highAttackEntity, maxAttack, _highAttackEntity, _lowAttackEntity]);
            
            Assert.That(combatantID, Is.EqualTo(maxAttack));
        }

        [Test]
        public void Negative_GetEntity_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _highestAttackSelector.GetEntity([]));
        }
    }
}
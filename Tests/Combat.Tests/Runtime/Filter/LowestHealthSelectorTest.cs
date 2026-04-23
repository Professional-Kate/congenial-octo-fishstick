using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class LowestHealthSelectorTest
    {
        private LowestHealthSelector _lowestHealthSelector;
        private RepositoryAsserter _repositoryAsserter;

        private CombatantEntity _highHealthEntity;
        private CombatantEntity _lowHealthEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lowestHealthSelector = new LowestHealthSelector(new CollectionAssertion());
            _repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            
            _highHealthEntity = CombatantEntityFactory.CreateCombatantEntity(1, true,
                new CombatantCard
                {
                    StatCard = new StatCard { Attack = 5, Health = 10, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, CombatantType = CombatantType.HUMAN, Information = new Information { Name = "", Description = "" }
                });

            _lowHealthEntity = CombatantEntityFactory.CreateCombatantEntity(15, true,
                new CombatantCard
                {
                    StatCard = new StatCard { Attack = 4, Health = 5, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, CombatantType = CombatantType.HUMAN, Information = new Information { Name = "", Description = "" }
                });
        }

        [Test]
        public void Positive_GetEntity_FindsLowestHealthEntity()
        {
            CombatantEntity combatant = _lowestHealthSelector.GetEntity([_highHealthEntity, _lowHealthEntity, _highHealthEntity, _highHealthEntity]);
            
            Assert.That(combatant, Is.EqualTo(_lowHealthEntity));
        }

        [Test]
        public void Positive_GetEntity_OneHP_EarlyReturn()
        {
            CombatantEntity oneHealth = CombatantEntityFactory.CreateCombatantEntity(12, true,
                new CombatantCard
                {
                    StatCard = new StatCard { Attack = 4, Health = 1, Speed = 5 }, TargetingType = TargetingType.HIGH_ATTACK, CombatantType = CombatantType.HUMAN, Information = new Information { Name = "", Description = "" }
                });
            
            CombatantEntity combatant = _lowestHealthSelector.GetEntity([_lowHealthEntity, oneHealth, _lowHealthEntity, _highHealthEntity]);
            
            Assert.That(combatant, Is.EqualTo(oneHealth));
        }

        [Test]
        public void Negative_GetEntity_EmptyCollection_Throws()
        { 
            Assert.Throws<EmptyCollectionException>(() => _lowestHealthSelector.GetEntity([]));
        }
    }
}
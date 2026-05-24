using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class LowestHealthSelectorTest
    {
        private LowestHealthSelector _lowestHealthSelector;

        private CombatantEntity _highHealthEntity;
        private CombatantEntity _lowHealthEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lowestHealthSelector = new LowestHealthSelector(new CollectionAssertion());
            
            _highHealthEntity = TestCombatantEntityFactory.CreateCombatantEntity(1, true, 
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 5, Health = 10 }));

            _lowHealthEntity = TestCombatantEntityFactory.CreateCombatantEntity(15, true, 
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 4, Health = 5 }));
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
            CombatantEntity oneHealth = TestCombatantEntityFactory.CreateCombatantEntity(12, true, 
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 4, Health = 1 }));
            
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
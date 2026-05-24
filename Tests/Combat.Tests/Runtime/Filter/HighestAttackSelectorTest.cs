using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Runtime.Filter
{
    [TestFixture]
    public sealed class HighestAttackSelectorTest
    {
        private HighestAttackSelector _highestAttackSelector;

        private CombatantEntity _highAttackEntity;
        private CombatantEntity _lowAttackEntity;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _highestAttackSelector = new HighestAttackSelector(new CollectionAssertion());

            _highAttackEntity = TestCombatantEntityFactory.CreateCombatantEntity(12, true,
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 8, Health = 7 }));
            
            _lowAttackEntity = TestCombatantEntityFactory.CreateCombatantEntity(27, true,
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = 2, Health = 6 }));
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
            CombatantEntity maxAttack = TestCombatantEntityFactory.CreateCombatantEntity(25, true,
                TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN, new StatCard { Attack = uint.MaxValue, Health = 1 }));
            
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
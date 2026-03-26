using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class CombatantAssertionTest
    {
        private CombatantAssertion _combatantAssertion;

        private CombatantEntity _combatant;

        [SetUp]
        public void Setup()
        {
            _combatantAssertion = new CombatantAssertion();

            _combatant = CombatantEntityFactory.CreateCombatantEntity(0);
        }

        [Test]
        public void Positive_AssertCombatantAlive_CombatantIsAlive_NoThrow()
        { 
            Assert.DoesNotThrow(() => _combatantAssertion.AssertCombatantAlive(_combatant));
        }
        
        [Test]
        public void Negative_AssertCombatantAlive_CombatantNotAlive_Throws()
        { 
            _combatant.UpdateLifeStatus(false);
            
            CombatantDeadException exception = Assert.Throws<CombatantDeadException>(() => _combatantAssertion.AssertCombatantAlive(_combatant));
            
            Assert.That(exception.CombatantID, Is.EqualTo(_combatant.CombatantID));
        }
    }
}
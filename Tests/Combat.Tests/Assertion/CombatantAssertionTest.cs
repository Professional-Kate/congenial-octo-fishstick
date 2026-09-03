using IdelPog.Combat.Assertion;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Tests.TestFactory;

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

            _combatant = TestCombatantEntityFactory.Create(0, TargetingType.FRIENDLY);
        }

        [Test]
        public void Positive_AssertCombatantAlive_CombatantIsAlive_NoThrow()
        { 
            Assert.DoesNotThrow(() => _combatantAssertion.AssertCombatantAlive(_combatant));
        }
        
        [Test]
        public void Negative_AssertCombatantAlive_CombatantNotAlive_Throws()
        { 
            _combatant.ReplaceComponent(new LifeStatusComponent { IsAlive = false });
            
            CombatantDeadException exception = Assert.Throws<CombatantDeadException>(() => _combatantAssertion.AssertCombatantAlive(_combatant));
            
            Assert.That(exception.CombatantID, Is.EqualTo(_combatant.InstanceID));
        }
    }
}
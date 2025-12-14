using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class CombatantStatsAssertionTest
    {
        private CombatantStatsAssertion _combatantStatsAssertion;
        
        private CombatantStats _combatantStats;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantStatsAssertion = new CombatantStatsAssertion(new AmountAssertion());
            
            _combatantStats = new CombatantStats { Attack = 1, Health = 1, Speed = 1 };
        }

        [Test]
        public void Positive_AssertStats_GoodStats_NoThrow()
        {
            Assert.DoesNotThrow(() => _combatantStatsAssertion.AssertStats(_combatantStats));
        }

        [Test]
        public void Negative_AssertStats_ZeroAttack_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _combatantStatsAssertion.AssertStats(_combatantStats with { Attack = 0 }));
        }
        
        [Test]
        public void Negative_AssertStats_ZeroHealth_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _combatantStatsAssertion.AssertStats(_combatantStats with { Health = 0 }));
        }
        
        [Test]
        public void Negative_AssertStats_ZeroSpeed_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _combatantStatsAssertion.AssertStats(_combatantStats with { Speed = 0 }));
        }
    }
}
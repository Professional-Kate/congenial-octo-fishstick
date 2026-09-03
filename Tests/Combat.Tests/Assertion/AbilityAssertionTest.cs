using IdelPog.Combat.Assertion;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class AbilityAssertionTest
    {
        private AbilityAssertion _abilityAssertion;
        
        private const byte MAX_ABILITIES = 2;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _abilityAssertion = new AbilityAssertion { MaxAbilitiesSlots = MAX_ABILITIES };
        }

        [Test]
        public void Positive_AssertAbilityCount_AbilityCountUnderMax_NoThrow()
        { 
            Assert.DoesNotThrow(() => _abilityAssertion.AssertAbilityCount(1));
        }
        
        [Test]
        public void Positive_AssertAbilityCount_AbilityCountEqualMax_NoThrow()
        { 
            Assert.DoesNotThrow(() => _abilityAssertion.AssertAbilityCount(2));
        }

        [Test]
        public void Negative_AssertAbilityCount_AbilityCountOverMax_Throws()
        {
            Assert.Throws<TooManyAbilitiesException>(() => _abilityAssertion.AssertAbilityCount(3));
        }
    }
}
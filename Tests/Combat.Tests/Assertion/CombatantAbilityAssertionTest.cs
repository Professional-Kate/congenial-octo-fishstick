using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class CombatantAbilityAssertionTest
    {
        private CombatantAbilityAssertion _combatantAbilityAssertion;
        
        private CombatantAbilityEquip _combatantAbilityEquip;
        private AbilityCard _abilityCard;

        private const byte MAX_ABILITIES = 2;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityAssertion = new CombatantAbilityAssertion { MaxAbilities = MAX_ABILITIES };
            
            _abilityCard = new AbilityCard { AbilityType = AbilityType.BASIC_ATTACK, StrategyCard = new StrategyCard { TargetingType = TargetingType.HIGH_ATTACK }};
            _combatantAbilityEquip = CreateAbilityEquip(0, _abilityCard);
        }

        private static CombatantAbilityEquip CreateAbilityEquip(byte combatantID , params AbilityCard[] abilityCards) => new() { CombatantID = combatantID, AbilityCards = abilityCards };

        [Test]
        public void Positive_AssertAbilityCount_AbilityCountUnderMax_NoThrow()
        { 
            Assert.DoesNotThrow(() => _combatantAbilityAssertion.AssertAbilityCount(_combatantAbilityEquip));
        }
        
        [Test]
        public void Positive_AssertAbilityCount_AbilityCountEqualMax_NoThrow()
        { 
            CombatantAbilityEquip combatantAbilityEquip = CreateAbilityEquip(0, _abilityCard, _abilityCard);
            Assert.DoesNotThrow(() => _combatantAbilityAssertion.AssertAbilityCount(combatantAbilityEquip));
        }

        [Test]
        public void Negative_AssertAbilityCount_AbilityCountOverMax_Throws()
        {
            CombatantAbilityEquip combatantAbilityEquip = CreateAbilityEquip(0, _abilityCard, _abilityCard, _abilityCard);
            TooManyAbilitiesException exception = Assert.Throws<TooManyAbilitiesException>(() => _combatantAbilityAssertion.AssertAbilityCount(combatantAbilityEquip));
            
            Assert.That(exception.CombatantAbilityEquip, Is.EqualTo(combatantAbilityEquip));
        }
    }
}
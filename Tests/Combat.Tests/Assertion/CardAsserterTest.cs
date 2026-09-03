using IdelPog.Combat.Assertion;
using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class CardAsserterTest
    {
        private CardAsserter _cardAsserter;

        private readonly CombatantCreation _combatantCreation = TestCombatantCreationFactory.CreateCombatantCreation(CombatantType.HUMAN);

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _cardAsserter = new CardAsserter(new NumberAssertion());
        }

        [Test]
        public void Positive_AssertCombatantCards_ValidCards_NoThrow()
        { 
            Assert.DoesNotThrow(() => _cardAsserter.AssertCombatantCards(_combatantCreation));
        }

        [Test]
        public void Negative_AssertCombatantCards_BadStatCard_Throws()
        {
            StatCard badStatCard = new() { Health = 0 };
            
            NumberZeroException exception = Assert.Throws<NumberZeroException>(() => _cardAsserter.AssertCombatantCards(_combatantCreation with { StatCard = badStatCard }));
            
            Assert.That(exception.Source, Is.EqualTo(nameof(badStatCard.Health)));
        }

        [Test]
        public void Negative_AssertCombatantCards_BadAgilityCard_Throws()
        {
            AgilityCard zeroSpeed = new() { Speed = 0, Initiative = 1 };
            AgilityCard zeroInitiative = new() { Speed = 1, Initiative = 0 };
            
            NumberZeroException zeroSpeedException = Assert.Throws<NumberZeroException>(() => _cardAsserter.AssertCombatantCards(_combatantCreation with { AgilityCard = zeroSpeed }));
            Assert.That(zeroSpeedException.Source, Is.EqualTo(nameof(zeroSpeed.Speed)));
            
            NumberZeroException zeroInitiativeException = Assert.Throws<NumberZeroException>(() => _cardAsserter.AssertCombatantCards(_combatantCreation with { AgilityCard = zeroInitiative }));
            Assert.That(zeroInitiativeException.Source, Is.EqualTo(nameof(zeroInitiative.Initiative)));
        }
    }
}
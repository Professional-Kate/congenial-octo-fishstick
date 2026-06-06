using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class AbilityEntityFactoryTest
    {
        private AbilityEntityFactory _abilityEntityFactory;
        
        private AbilityCreation _basicAttackCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _abilityEntityFactory = new AbilityEntityFactory();
            _basicAttackCreation = TestAbilityCreationFactory.Create(AbilityType.SLASH);
        }

        private static void AssertSkillEntity(AbilityEntity abilityEntity, AbilityCreation abilityCreation)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity, Is.Not.Null);
                Assert.That(abilityEntity.AbilityType, Is.EqualTo(abilityCreation.AbilityCard.AbilityType));
                Assert.That(abilityEntity.Information, Is.EqualTo(abilityCreation.Information));
                Assert.That(abilityEntity.GetComponent<CooldownComponent>().Cooldown, Is.EqualTo(abilityCreation.AbilityCard.Cooldown));
                CardAssertions.AssertElementalDamageCard(abilityEntity, abilityCreation.ElementalDamageCard);
                CardAssertions.AssertPhysicalDamageCard(abilityEntity, abilityCreation.PhysicalDamageCard);
            }
        }

        private static void AssertCastTimeComponent(AbilityEntity abilityEntity, uint castTime)
        {
            if (castTime == 0)
            { 
                Assert.That(abilityEntity.ContainsComponent<CastTimeComponent>(), Is.False);
                return;
            }
            
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity.ContainsComponent<CastTimeComponent>(), Is.True);
                Assert.That(abilityEntity.GetComponent<CastTimeComponent>().CastTime, Is.EqualTo(castTime));
            }
        }
        
        [TestCase(0u)]
        [TestCase(100u)]
        public void Positive_CreateAbilityEntity_CreatesEntity_OptionalCastTimeComponent(uint castTime)
        {
            AbilityCard abilityCard = _basicAttackCreation.AbilityCard with { CastTime = castTime };
            AbilityCreation castTimeCreation = _basicAttackCreation with { AbilityCard = abilityCard };
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(castTimeCreation);
            
            AssertSkillEntity(abilityEntity, castTimeCreation);
            AssertCastTimeComponent(abilityEntity, castTime);
        }
    }
}
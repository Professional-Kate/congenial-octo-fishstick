using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory;

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
            _basicAttackCreation = TestAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
        }

        private static void AssertSkillEntity(AbilityEntity abilityEntity, AbilityCreation abilityCreation)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(abilityEntity, Is.Not.Null);
                Assert.That(abilityEntity.AbilityType, Is.EqualTo(abilityCreation.AbilityType));
                Assert.That(abilityEntity.Information, Is.EqualTo(abilityCreation.Information));
                Assert.That(abilityEntity.GetComponent<CooldownComponent>().Cooldown, Is.EqualTo(abilityCreation.Cooldown));
                Assert.That(abilityEntity.GetComponent<DamageComponent>().PhysicalDamage, Is.EqualTo(abilityCreation.DamageCard.PhysicalDamage));
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
            AbilityCreation castTimeCreation = _basicAttackCreation with { CastTime = castTime };
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(castTimeCreation);
            
            AssertSkillEntity(abilityEntity, castTimeCreation);
            AssertCastTimeComponent(abilityEntity, castTime);
        }
    }
}
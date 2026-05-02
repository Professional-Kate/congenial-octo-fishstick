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
        
        private AbilityCreation _abilityCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _abilityEntityFactory = new AbilityEntityFactory();
            _abilityCreation = TestAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
        }

        private static void AssertSkillEntity(AbilityEntity abilityEntity, AbilityCreation abilityCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(abilityEntity, Is.Not.Null);
                Assert.That(abilityEntity.AbilityType, Is.EqualTo(abilityCreation.AbilityType));
                Assert.That(abilityEntity.Information, Is.EqualTo(abilityCreation.Information));
                Assert.That(abilityEntity.GetComponent<CooldownComponent>().Cooldown, Is.EqualTo(abilityCreation.Cooldown));
                Assert.That(abilityEntity.GetComponent<DamageComponent>().PhysicalDamage, Is.EqualTo(abilityCreation.DamageCard.PhysicalDamage));
            });
        }

        [Test]
        public void Positive_CreateSkillEntity_CreatesSkillEntity()
        {
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(_abilityCreation);

            AssertSkillEntity(abilityEntity, _abilityCreation);
        }
    }
}
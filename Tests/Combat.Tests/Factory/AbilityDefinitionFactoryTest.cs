using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Factory;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Tests.Factory
{
    [TestFixture]
    public sealed class AbilityDefinitionFactoryTest
    {
        private AbilityDefinitionFactory _abilityDefinitionFactory;
        
        private AbilityDefinitionCreation _abilityDefinitionCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _abilityDefinitionFactory = new AbilityDefinitionFactory();

            _abilityDefinitionCreation = new AbilityDefinitionCreation
            {
                AbilityType = AbilityType.STAB,
                TargetingInformation = new TargetingInformation { TargetingType = TargetingType.SINGLE, MaxTargets = 1 },
                Information = new Information { Name = "", Description = "" },
                Cooldown = 0,
                Damage = 1
            };
        }

        private static void AssertAbilityDefinition(AbilityDefinition abilityDefinition, AbilityDefinitionCreation abilityDefinitionCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(abilityDefinition.AbilityType, Is.EqualTo(abilityDefinitionCreation.AbilityType));
                Assert.That(abilityDefinition.TargetingInformation, Is.EqualTo(abilityDefinitionCreation.TargetingInformation));
                Assert.That(abilityDefinition.Information, Is.EqualTo(abilityDefinitionCreation.Information));
                Assert.That(abilityDefinition.Cooldown, Is.EqualTo(abilityDefinitionCreation.Cooldown));
                Assert.That(abilityDefinition.Damage, Is.EqualTo(abilityDefinitionCreation.Damage));
            });
        }

        [Test]
        public void Positive_Create_CreatesAbilityDefinition()
        {
            AbilityDefinition definition = _abilityDefinitionFactory.Create(_abilityDefinitionCreation);
            
            AssertAbilityDefinition(definition, _abilityDefinitionCreation);
        }
    }
}
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class AbilityEntityFactoryTest
    {
        private AbilityEntityFactory _abilityEntityFactory;
        
        private CombatantAbilityCreation _combatantAbilityCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _abilityEntityFactory = new AbilityEntityFactory(new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));
            _combatantAbilityCreation = TestCombatantAbilityCreationFactory.Create(AbilityType.BASIC_ATTACK);
        }

        private static void AssertSkillEntity(AbilityEntity abilityEntity, CombatantAbilityCreation combatantAbilityCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(abilityEntity, Is.Not.Null);
                Assert.That(abilityEntity.AbilityType, Is.EqualTo(combatantAbilityCreation.AbilityType));
                Assert.That(abilityEntity.Information, Is.EqualTo(combatantAbilityCreation.Information));
                Assert.That(abilityEntity.GetComponent<SpeedComponent>().Speed, Is.EqualTo(combatantAbilityCreation.Speed));
                Assert.That(abilityEntity.GetComponent<DamageComponent>().Damage, Is.EqualTo(combatantAbilityCreation.Damage));
            });
        }

        [Test]
        public void Positive_CreateSkillEntity_CreatesSkillEntity()
        {
            AbilityEntity abilityEntity = _abilityEntityFactory.CreateAbilityEntity(_combatantAbilityCreation);

            AssertSkillEntity(abilityEntity, _combatantAbilityCreation);
        }
    }
}
using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Skill;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Runtime.System.Factory;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class SkillEntityFactoryTest
    {
        private SkillEntityFactory _skillEntityFactory;
        
        private CombatantSkillCreation _combatantSkillCreation;

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            _skillEntityFactory = new SkillEntityFactory(new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion()));
            _combatantSkillCreation = CombatantSkillCreationFactory.Create(SkillType.BASIC_ATTACK);
        }

        private static void AssertSkillEntity(SkillEntity skillEntity, CombatantSkillCreation combatantSkillCreation)
        {
            Assert.Multiple(() =>
            {
                Assert.That(skillEntity, Is.Not.Null);
                Assert.That(skillEntity.SkillType, Is.EqualTo(combatantSkillCreation.SkillType));
                Assert.That(skillEntity.Information, Is.EqualTo(combatantSkillCreation.Information));
                Assert.That(skillEntity.GetComponent<SpeedComponent>().Speed, Is.EqualTo(combatantSkillCreation.Speed));
                Assert.That(skillEntity.GetComponent<DamageComponent>().Damage, Is.EqualTo(combatantSkillCreation.Damage));
            });
        }

        [Test]
        public void Positive_CreateSkillEntity_CreatesSkillEntity()
        {
            SkillEntity skillEntity = _skillEntityFactory.CreateSkillEntity(_combatantSkillCreation);

            AssertSkillEntity(skillEntity, _combatantSkillCreation);
        }
    }
}
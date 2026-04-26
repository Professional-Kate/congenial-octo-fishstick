using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantAbilityFactoryTest
    {
        private CombatantAbilityFactory _combatantAbilityFactory;

        private CombatantAbilityEntity _combatantAbilityEntity;
        private DamageComponent _damageComponent;
        private CooldownComponent _cooldownComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityFactory = new CombatantAbilityFactory();
        }

        [SetUp]
        public void Setup()
        {
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, AbilityType.BASIC_ATTACK);

            _damageComponent = new DamageComponent { Damage = 10 };
            _cooldownComponent = new CooldownComponent { Cooldown = 5 };
            _combatantAbilityEntity.AddComponent(_damageComponent);
            _combatantAbilityEntity.AddComponent(_cooldownComponent);
        }

        private void AssertCombatantAbility(CombatantAbility combatantAbility, CombatantAbilityEntity sourceEntity)
        {
            Assert.Multiple(() =>
            {
                Assert.That(combatantAbility.AbilityType, Is.EqualTo(sourceEntity.AbilityType));
                Assert.That(combatantAbility.Damage, Is.EqualTo(_damageComponent.Damage));
                Assert.That(combatantAbility.Cooldown, Is.EqualTo(_cooldownComponent.Cooldown));
            });
        }

        [Test]
        public void Positive_CreateCombatantAbility_ConvertsEntity()
        {
            CombatantAbility combatantAbility = _combatantAbilityFactory.CreateCombatantAbility(_combatantAbilityEntity);

            AssertCombatantAbility(combatantAbility, _combatantAbilityEntity);
        }

        [Test]
        public void Positive_CreateCombatantAbilities_ConvertsAll()
        {
            CombatantAbility[] combatantAbilities = _combatantAbilityFactory.CreateCombatantAbilities([_combatantAbilityEntity, _combatantAbilityEntity]);

            foreach (CombatantAbility combatantAbility in combatantAbilities)
            {
                AssertCombatantAbility(combatantAbility, _combatantAbilityEntity);
            }
        }
        
        [Test]
        public void Positive_CreateCombatantAbilities_EmptyInput_ReturnsNothing()
        {
            CombatantAbility[] combatantAbilities = _combatantAbilityFactory.CreateCombatantAbilities([]);
            
            Assert.That(combatantAbilities, Is.Empty);
        }
    }
}
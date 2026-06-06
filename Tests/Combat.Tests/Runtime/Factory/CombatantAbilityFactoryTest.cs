using IdelPog.Combat.Contracts;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Factory;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.Factory
{
    [TestFixture]
    public sealed class CombatantAbilityFactoryTest
    {
        private CombatantAbilityFactory _combatantAbilityFactory;

        private CombatantAbilityEntity _combatantAbilityEntity;
        private ElementalDamageComponent _elementalDamageComponent;
        private PhysicalDamageComponent _physicalDamageComponent;
        private CooldownComponent _cooldownComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _combatantAbilityFactory = new CombatantAbilityFactory();
        }

        [SetUp]
        public void Setup()
        {
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(1, AbilityType.SLASH);

            _elementalDamageComponent = new ElementalDamageComponent { ColdDamage = 1, LightningDamage = 1, FireDamage = 1 };
            _physicalDamageComponent = new PhysicalDamageComponent { SlashDamage = 1, StrikeDamage = 1, ThrustDamage = 1 };
            _cooldownComponent = new CooldownComponent { Cooldown = 5 };
            _combatantAbilityEntity.AddComponent(_elementalDamageComponent);
            _combatantAbilityEntity.AddComponent(_physicalDamageComponent);
            _combatantAbilityEntity.AddComponent(_cooldownComponent);
        }

        private void AssertCombatantAbility(CombatantAbility combatantAbility, CombatantAbilityEntity sourceEntity)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(combatantAbility.AbilityType, Is.EqualTo(sourceEntity.AbilityType));
                Assert.That(combatantAbility.Cooldown, Is.EqualTo(_cooldownComponent.Cooldown));
                CardAssertions.AssertElementalDamageCard(sourceEntity, combatantAbility.ElementalDamageCard);
                CardAssertions.AssertPhysicalDamageCard(sourceEntity, combatantAbility.PhysicalDamageCard);
            }
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
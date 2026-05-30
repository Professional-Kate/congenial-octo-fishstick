using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        
        private CombatantEntity _targetEntity;
        private StatsComponent _statsComponent;
        private CombatantAbilityEntity _combatantAbilityEntity;
        private ElementalDamageComponent _elementalDamageComponent;
        private PhysicalDamageComponent _physicalDamageComponent;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystem = new DamageSystem();

            _elementalDamageComponent = new ElementalDamageComponent { LightningDamage = 1, ColdDamage = 1, FireDamage = 1 };
            _physicalDamageComponent = new PhysicalDamageComponent { SlashDamage = 1, StrikeDamage = 1, ThrustDamage = 1 };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = TestCombatantEntityFactory.CreateCombatantEntity(0);
            _statsComponent = _targetEntity.GetComponent<StatsComponent>();
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(0, AbilityType.BASIC_ATTACK);
            _combatantAbilityEntity.AddComponent(_elementalDamageComponent);
            _combatantAbilityEntity.AddComponent(_physicalDamageComponent);
        }

        private static void AssertNewHealth(uint newHealth, uint expectedHealth)
        { 
            Assert.That(newHealth, Is.EqualTo(expectedHealth));
        }

        private static void AssertEntityHealth(CombatantEntity combatantEntity, uint expectedHealth)
        {
            Assert.That(combatantEntity.GetComponent<StatsComponent>().Health, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _combatantAbilityEntity);
            
            AssertNewHealth(newHealth,_statsComponent.Health - _elementalDamageComponent.TotalDamage - _physicalDamageComponent.TotalDamage);
            AssertEntityHealth(_targetEntity, newHealth);
        }
        
        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            _combatantAbilityEntity.RemoveComponent<ElementalDamageComponent>();
            _combatantAbilityEntity.AddComponent(new ElementalDamageComponent { LightningDamage = 14, ColdDamage = 255, FireDamage = 153 });
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _combatantAbilityEntity);
            
            AssertNewHealth(newHealth, 0);
            AssertEntityHealth(_targetEntity, newHealth);
        }

        [Test]
        public void Positive_GetCalculatedDamage_ReturnsCalculatedDamage()
        {
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(_combatantAbilityEntity);
            
            Assert.That(calculatedDamage, Is.EqualTo(_elementalDamageComponent.TotalDamage + _physicalDamageComponent.TotalDamage));
        }

        [Test]
        public void Positive_ZeroDamageFromEverything_ReturnsZero()
        {
            ElementalDamageComponent weakElementalDamage = new() { LightningDamage = 0, ColdDamage = 0, FireDamage = 0 };
            PhysicalDamageComponent weakPhysicalDamage = new() { SlashDamage = 0, StrikeDamage = 0, ThrustDamage = 0 };
            
            CombatantAbilityEntity weakAbility = TestCombatantAbilityEntityFactory.Create(0, AbilityType.BASIC_ATTACK);
            weakAbility.AddComponent(weakElementalDamage);
            weakAbility.AddComponent(weakPhysicalDamage);
            
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(weakAbility);
            Assert.That(calculatedDamage, Is.Zero);

            uint newHealth = _damageSystem.DealDamage(_targetEntity, weakAbility);
            Assert.That(newHealth, Is.EqualTo(_statsComponent.Health));
        }
    }
}
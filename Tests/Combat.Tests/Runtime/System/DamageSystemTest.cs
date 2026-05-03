using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System;

namespace IdelPog.Combat.Tests.Runtime.System
{
    [TestFixture]
    public sealed class DamageSystemTest
    {
        private DamageSystem _damageSystem;
        
        private CombatantEntity _targetEntity;
        private StatCard _attackerStats;
        private CombatantStatsComponent _combatantStatsComponent;
        private CombatantAbilityEntity _combatantAbilityEntity;
        private DamageComponent _damageComponent;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystem = new DamageSystem();

            _attackerStats = new StatCard { Attack = 1, Health = 10, Speed = 5 };
            _damageComponent = new DamageComponent { PhysicalDamage = 1, LightningDamage = 1, ColdDamage = 1, FireDamage = 1 };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = TestCombatantEntityFactory.CreateCombatantEntity(0);
            _combatantStatsComponent = _targetEntity.GetComponent<CombatantStatsComponent>();
            _combatantAbilityEntity = TestCombatantAbilityEntityFactory.Create(0, AbilityType.BASIC_ATTACK);
            _combatantAbilityEntity.AddComponent(_damageComponent);
        }

        private static void AssertNewHealth(uint newHealth, uint expectedHealth)
        { 
            Assert.That(newHealth, Is.EqualTo(expectedHealth));
        }

        private static void AssertEntityHealth(CombatantEntity combatantEntity, uint expectedHealth)
        {
            Assert.That(combatantEntity.GetComponent<CombatantStatsComponent>().Health, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _attackerStats.Attack, _combatantAbilityEntity);
            
            AssertNewHealth(newHealth,_combatantStatsComponent.Health - _attackerStats.Attack - _damageComponent.TotalDamage);
            AssertEntityHealth(_targetEntity, newHealth);
        }
        
        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, 200, _combatantAbilityEntity);
            
            AssertNewHealth(newHealth, 0);
            AssertEntityHealth(_targetEntity, newHealth);
        }

        [Test]
        public void Positive_GetCalculatedDamage_ReturnsCalculatedDamage()
        {
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(_attackerStats.Attack, _combatantAbilityEntity);
            
            Assert.That(calculatedDamage, Is.EqualTo(_damageComponent.TotalDamage + _attackerStats.Attack));
        }

        [Test]
        public void Positive_ZeroDamageFromEverything_ReturnsZero()
        {
            StatCard weakAttacker = _attackerStats with { Attack = 0 };
            DamageComponent weakDamage = new() { PhysicalDamage = 0, LightningDamage = 0, ColdDamage = 0, FireDamage = 0 };
            CombatantAbilityEntity weakAbility = TestCombatantAbilityEntityFactory.Create(0, AbilityType.BASIC_ATTACK);
            weakAbility.AddComponent(weakDamage);
            
            uint calculatedDamage = _damageSystem.GetCalculatedDamage(weakAttacker.Attack, weakAbility);
            Assert.That(calculatedDamage, Is.Zero);

            uint newHealth = _damageSystem.DealDamage(_targetEntity, weakAttacker.Attack, weakAbility);
            Assert.That(newHealth, Is.EqualTo(_combatantStatsComponent.Health));
        }
    }
}
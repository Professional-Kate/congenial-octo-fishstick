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
            _damageComponent = new DamageComponent { Damage = 1 };
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
            
            AssertNewHealth(newHealth,_combatantStatsComponent.Health - _attackerStats.Attack - _damageComponent.Damage);
            AssertEntityHealth(_targetEntity, newHealth);
        }
        
        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, 200, _combatantAbilityEntity);
            
            AssertNewHealth(newHealth, 0);
            AssertEntityHealth(_targetEntity, newHealth);
        }
    }
}
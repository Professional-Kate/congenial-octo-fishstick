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
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _damageSystem = new DamageSystem();

            _attackerStats = new StatCard { Attack = 1, Health = 10, Speed = 5 };
        }

        [SetUp]
        public void Setup()
        {
            _targetEntity = CombatantEntityFactory.CreateCombatantEntity(0);
            _combatantStatsComponent = _targetEntity.GetComponent<CombatantStatsComponent>();
        }

        private static void AssertNewHealth(uint newHealth, uint expectedHealth)
        { 
            Assert.That(newHealth, Is.EqualTo(expectedHealth));
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, _attackerStats.Attack);
            
            AssertNewHealth(newHealth,_combatantStatsComponent.Health - _attackerStats.Attack);
        }

        [Test]
        public void Positive_DealDamage_DamagesEntity_MoreAttackThanHealth_ReturnsZero()
        {
            uint newHealth = _damageSystem.DealDamage(_targetEntity, 200);
            
            AssertNewHealth(newHealth, 0);
        }
    }
}